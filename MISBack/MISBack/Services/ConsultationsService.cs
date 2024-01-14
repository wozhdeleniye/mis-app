using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Services
{
    public class ConsultationsService : IConsultationsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ConsultationsService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<Guid> AddComment(Guid id, CommentCreateModel commentModel, Guid docId)
        {
            if(await _context.Consultations.FirstOrDefaultAsync(x=>x.id == id) == null)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    $"Нет косультации с id: {id}"
                );
                throw ex;
            }
            Guid newId = Guid.NewGuid();
            await _context.Comments.AddAsync(new Comment
            {
                id = newId,
                authorId = docId,
                createTime = DateTime.Now,
                consultationId = id,
                parentId = commentModel.parentId,
                content = commentModel.content
            });
            await _context.SaveChangesAsync();
            return newId;
        }

        public async Task EditComment(Guid id, InspectionCommentCreateModel commentModel)
        {
            var commEntity = await _context
                .Comments
                .FirstOrDefaultAsync(x => x.id == id);
            commEntity.content = commentModel.content;
            commEntity.modifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();
        }

        public async Task<ConsultationModel> GetConsultation(Guid id)
        {
            var conEntity = await _context
                .Consultations
                .FirstOrDefaultAsync(x => x.id == id);
            ConsultationModel conResult = new ConsultationModel();
            if (conEntity == null)
            {
                return conResult;
            }
            conResult.id = id;
            conResult.createTime = conEntity.createTime;
            conResult.inspectionId = conEntity.inspectionId;

            var specEntity = await _context
                .Specialities
                .FirstOrDefaultAsync(x => x.id == conEntity.specialityId);
            conResult.speciality = new SpecialityModel();
            conResult.speciality.id = specEntity.id;
            conResult.speciality.createTime = specEntity.createTime;
            conResult.speciality.name = specEntity.name; 
            conResult.comments = new List<CommentModel>();
            var commsEntity = await _context
                .Comments
                .Where(x=>x.consultationId == id).ToListAsync();
            foreach(var comms in commsEntity)
            {
                var author = await _context
                    .Doctors
                    .FirstOrDefaultAsync(x => x.id == comms.authorId);
                CommentModel comment = _mapper.Map<CommentModel>(comms);
                comment.author = author.name;
                conResult.comments.Add(comment);
            }
            return conResult;
        }

        public async Task<InspectionPagedListModel> GetInspections(List<int>? icdRoots, bool grouped, int page, int size, Guid docId)
        {
            var inspecsEntity = new List<Inspection>();
            if (grouped)inspecsEntity = await _context
                    .Inspections
                    .Where(x => x.baseInspectionId == x.id).ToListAsync();
            else inspecsEntity = await _context
                    .Inspections.ToListAsync();
            var inspecsList = new List<Inspection>();
            foreach(var inspec in inspecsEntity)
            {
                var consEntity = await _context
                    .Consultations
                    .Where(x=> x.inspectionId == inspec.id).ToListAsync();

                var docEntity = await _context
                        .Doctors
                        .FirstOrDefaultAsync(x => x.id == docId);
                consEntity = consEntity.Where(x => x.specialityId == docEntity.speciality).ToList();
                if(consEntity != null)
                {
                    inspecsList.Add(inspec);
                }
            }
            if(icdRoots != null && icdRoots.Count > 0)
            {
                var icdSortedInspections = new List<Inspection>();
                foreach (var inspec in inspecsList)
                {
                    var diagEntity = await _context
                        .Diagnoses
                        .FirstOrDefaultAsync(x => x.type == DiagnosisType.Main && x.InspectionId == inspec.id);
                    var icdEntity = await _context
                        .Icd10s
                        .FirstOrDefaultAsync(x => x.id == diagEntity.icd10Id);

                    while (icdEntity.parentId != null)
                    {
                        icdEntity = await _context
                        .Icd10s
                        .FirstOrDefaultAsync(x => x.id == icdEntity.parentId);
                    }
                    if (icdRoots.Contains(icdEntity.id)) icdSortedInspections.Add(inspec);
                }
                inspecsList = icdSortedInspections;
            }
            

            //pagintaion
            int start = (page - 1) * size;
            int end = Math.Min(size, inspecsList.Count - start);
            double count = Double.Ceiling((double)inspecsList.Count / size);
            if (page > count)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    "Out of page list");
                throw ex;
            }

            InspectionPagedListModel inspecResult = new InspectionPagedListModel();

            inspecResult.pagination = new PageInfoModel();
            inspecResult.inspections = new List<InspectionPreviewModel>();


            inspecResult.pagination.size = size;
            inspecResult.pagination.count = ((int)count);
            inspecResult.pagination.current = page;

            foreach (var inspection in inspecsList.GetRange(start, end))
            {
                bool hasChain = false;
                bool hasNested = false;
                var nestedInspectionEntity = await _context
                    .Inspections
                    .FirstOrDefaultAsync(x => x.previousInspectionId == inspection.id);
                if (nestedInspectionEntity != null) hasNested = true;
                if (inspection.id == inspection.baseInspectionId && hasNested) hasChain = true;


                var diagEntity = await _context
                    .Diagnoses
                    .FirstOrDefaultAsync(x => x.InspectionId == inspection.id && x.type == DiagnosisType.Main);
                var icdEntity = await _context
                    .Icd10s
                    .FirstOrDefaultAsync(x => x.id == diagEntity.icd10Id);
                var docEntity = await _context
                    .Doctors
                    .FirstOrDefaultAsync(x => x.id == inspection.docId);
                var patientEntity = await _context
                    .Patients
                    .FirstOrDefaultAsync(x => x.id == inspection.patientId);
                DiagnosisModel diagnosis = _mapper.Map<DiagnosisModel>(diagEntity);
                diagnosis.code = icdEntity.code;
                diagnosis.name = icdEntity.name;
                inspecResult.inspections.Add(new InspectionPreviewModel
                {
                    id = inspection.id,
                    createTime = inspection.createTime,
                    previousId = inspection.previousInspectionId,
                    date = inspection.date,
                    conclusion = inspection.conclusion,
                    doctorID = inspection.docId,
                    doctor = docEntity.name,
                    patientId = inspection.patientId,
                    patient = patientEntity.name,
                    diagnosis = diagnosis,
                    hasChain = hasChain,
                    hasNested = hasNested
                });
            }
            return inspecResult;
        }
    }
}

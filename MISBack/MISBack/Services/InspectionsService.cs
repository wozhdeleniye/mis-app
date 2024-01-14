using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;
using System.Security.Principal;
using System.Xml.Linq;

namespace MISBack.Services
{
    public class InspectionsService : IInspectionsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public InspectionsService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<InspectionModel> GetInspection(Guid id)
        {
            var inspecEntity = await _context
            .Inspections
            .FirstOrDefaultAsync(x => x.id == id);
            var ex = new Exception();
            if (inspecEntity != null)
            {
                if (inspecEntity.baseInspectionId == inspecEntity.id) inspecEntity.baseInspectionId = Guid.Empty;

                var patientEntity = await _context
                .Patients
                .FirstOrDefaultAsync(x => x.id == inspecEntity.patientId);

                var docEntity = await _context
                .Doctors
                .FirstOrDefaultAsync(x => x.id == inspecEntity.docId);

                List<Diagnosis> diags = await _context
                    .Diagnoses
                    .Where(x => x.InspectionId == id).ToListAsync();

                List<DiagnosisModel> diagsList = new List<DiagnosisModel>();
                foreach (var diagnosis in diags)
                {
                    var ICDEntity = await _context
                        .Icd10s
                        .FirstOrDefaultAsync(x => x.id == diagnosis.icd10Id);

                    DiagnosisModel diagnosisModel = _mapper.Map<DiagnosisModel>(diagnosis);
                    diagnosisModel.code = ICDEntity.code;
                    diagnosisModel.name = ICDEntity.name;

                    diagsList.Add(diagnosisModel);
                }
                var consultationsEntity = await _context
                    .Consultations
                    .Where(x => x.inspectionId == id).ToListAsync();
                List<InspectionConsultationModel> consultations = new List<InspectionConsultationModel>();
                
                foreach (var con in consultationsEntity)
                {
                    var specialityEntity = await _context
                        .Specialities
                        .FirstOrDefaultAsync(x => x.id == con.specialityId);
                    SpecialityModel spec = _mapper.Map<SpecialityModel>(specialityEntity);

                    var rootCommentEntity = await _context
                        .Comments
                        .FirstOrDefaultAsync(x => x.consultationId == con.id && x.parentId == null);
                    InspectionCommentModel rootComment = _mapper.Map<InspectionCommentModel>(rootCommentEntity);
                    if (rootCommentEntity != null)
                    {
                        if (rootComment.modifyTime != null)
                        {
                            rootComment.modifyTime = rootComment.modifyTime;
                        }
                        var commentAuthorEntity = await _context
                        .Doctors
                        .FirstOrDefaultAsync(x => x.id == rootCommentEntity.authorId);
                        rootComment.author = _mapper.Map<DoctorModel>(commentAuthorEntity);
                        
                    }

                    var allConComments = await _context
                            .Comments
                            .Where(x => x.consultationId == con.id).ToListAsync();

                    consultations.Add(new InspectionConsultationModel
                    {
                        id = con.id,
                        createTime = con.createTime,
                        inspectionId = con.inspectionId,
                        speciality = spec,
                        rootComment = rootComment,
                        commentNumber = allConComments.Count()
                    });
                }
                if (inspecEntity.id == inspecEntity.baseInspectionId) inspecEntity.baseInspectionId = Guid.Empty;
                return new InspectionModel
                {
                    id = inspecEntity.id,
                    createTime = inspecEntity.createTime,
                    date = inspecEntity.date,
                    anamnesis = inspecEntity.anamnesis,
                    complaints = inspecEntity.complaints,
                    treatment = inspecEntity.treatment,
                    conclusion = inspecEntity.conclusion,
                    nextVisitDate = inspecEntity.nextVisitDate,
                    deathDate = inspecEntity.deathDate,
                    baseInspectionId = inspecEntity.baseInspectionId,
                    previousInspectionId = inspecEntity.previousInspectionId,
                    patient = _mapper.Map<PatientModel>(patientEntity),
                    doctor = _mapper.Map<DoctorModel>(docEntity),
                    diagnoses = diagsList,
                    consultations = consultations
                };
            }
                

            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "Inspection not exists"
            );
            throw ex;
        }
        public async Task EditInspection(Guid id, InspectionEditModel inspectionModel, Guid docId)
        {
            if(inspectionModel.diagnoses.Where(x=>x.type == DiagnosisType.Main).Count() == 0)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    "Must be one diagnosis with type Main"
                );
                throw ex;
            }
            var inspecEntity = await _context
                .Inspections
                .FirstOrDefaultAsync(x => x.id == id);

            if(inspecEntity.docId != docId)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status403Forbidden.ToString(),
                    "User doesn't have editing rights (not the inspection author)"
                );
                throw ex;
            }

            inspecEntity.anamnesis = inspectionModel.anamnesis;
            inspecEntity.complaints = inspectionModel.complaints;
            inspecEntity.treatment = inspectionModel.treatment;
            inspecEntity.conclusion = inspectionModel.conclusion;
            inspecEntity.nextVisitDate = inspectionModel.nextVisitDate;
            inspecEntity.deathDate = inspectionModel.deathDate;

            var exDiagnosesEntity = await _context
                .Diagnoses
                .Where(x => x.InspectionId == id).ToListAsync();

            foreach (DiagnosisCreateModel diag in inspectionModel.diagnoses)
            {
                Diagnosis diagnosis = _mapper.Map<Diagnosis>(diag);
                diagnosis.icd10Id = diag.icdDiagnosisId;
                diagnosis.InspectionId = id;
                diagnosis.id = Guid.NewGuid();
                diagnosis.createTime = DateTime.Now;
                await _context.Diagnoses.AddAsync(diagnosis);
            }
            foreach(var exDiag in exDiagnosesEntity)
            {
                _context.Diagnoses.Remove(exDiag);
            }
            await _context.SaveChangesAsync();
        }

        public async Task<List<InspectionPreviewModel>> GetInspectionChain(Guid id)
        {
            List<InspectionPreviewModel> chain = new List<InspectionPreviewModel>();
            Guid prevId = id;
            var firstInspectionEntity = await _context
                .Inspections
                .FirstOrDefaultAsync(x => x.previousInspectionId == prevId);
            if (firstInspectionEntity == null) return chain;

            bool hasNext = true;

            while (hasNext)
            {
                
                var nextInspecEntity = await _context
                    .Inspections
                    .FirstOrDefaultAsync(x => x.previousInspectionId == prevId);
                var docEntity = await _context
                    .Doctors
                    .FirstOrDefaultAsync(x=> x.id == nextInspecEntity.docId);
                var patientEntity = await _context
                    .Patients
                    .FirstOrDefaultAsync(x => x.id == nextInspecEntity.patientId);
                var diagEntity = await _context
                    .Diagnoses
                    .FirstOrDefaultAsync(x => x.InspectionId == nextInspecEntity.id && x.type == DiagnosisType.Main);
                var icdEntity = await _context
                    .Icd10s
                    .FirstOrDefaultAsync(x => x.id == diagEntity.icd10Id);

                bool hasNested = false;
                var next1InspecEntity = await _context
                    .Inspections
                    .FirstOrDefaultAsync(x => x.previousInspectionId == nextInspecEntity.id);
                if(next1InspecEntity != null) hasNested = true;
                else hasNext = false;

                DiagnosisModel diagnosisModel = _mapper.Map<DiagnosisModel>(diagEntity);
                diagnosisModel.code = icdEntity.code;
                diagnosisModel.name = icdEntity.name;

                chain.Add(new InspectionPreviewModel
                {
                    id = nextInspecEntity.id,
                    createTime = nextInspecEntity.createTime,
                    previousId = nextInspecEntity.previousInspectionId,
                    date = nextInspecEntity.date,
                    conclusion = nextInspecEntity.conclusion,
                    doctorID = nextInspecEntity.docId,
                    doctor = docEntity.name,
                    patientId = nextInspecEntity.patientId,
                    patient = patientEntity.name,
                    diagnosis = diagnosisModel,
                    hasChain = false,
                    hasNested = hasNested
                });
                prevId = nextInspecEntity.id;
            }
            return chain;
        }
    }
}

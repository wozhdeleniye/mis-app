using AutoMapper;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using MISBack.Data;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace MISBack.Services
{
    public class PatientsService : IPatientsService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public PatientsService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Guid> CreatePatient(PatientCreateModel patientModel)
        {
            var ex = new Exception();

            if (patientModel.gender == null)
            {
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    $"Possible Gender values: Male, Female");
                throw ex;
            }

            CheckBirthDate(patientModel.birthday);

            var id = Guid.NewGuid();

            if(patientModel != null)
            {
                await _context.Patients.AddAsync(new Patient
                {
                    id = id,
                    createTime = DateTime.Now,
                    name = patientModel.name,
                    birthday = patientModel.birthday,
                    gender = patientModel.gender
                });
                await _context.SaveChangesAsync();
                return id;
            }
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    $"Invalid arguments");
            throw ex;
        }

        public async Task<Guid> CreateInspection(Guid id, InspectionCreateModel model, Guid docId)
        {
            var ex = new Exception();
            if (model == null) {
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    $"Model is empty");
                throw ex;
            }
            
            if (id == Guid.Empty || id == null)
            {
                ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                        $"Unauthorized");
                throw ex;
            }

            if (model.conclusion == Conclusion.Disease)
            {
                if (model.nextVisitDate == null)
                {
                    ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                        $"Next visit date please");
                    throw ex;
                }
            }

            if (model.conclusion == Conclusion.Death)
            {
                if(model.deathDate == null)
                {
                    ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                        $"Death date please");
                    throw ex;
                }
                var patientInspections = await _context
                    .Inspections
                    .Where(x => x.patientId == id && x.conclusion == Conclusion.Death).ToListAsync();
                if(patientInspections.Count == 0)
                {
                    ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                        $"Already dead. Omae wa mou shindeiru");
                    throw ex;
                }
            }

            if(model.diagnoses.Where(x=> x.type == DiagnosisType.Main).Count() == 0)
            {
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                        $"You need one main dianose if you want to create inspection");
                throw ex;
            }

            CheckNextVisitDate(model.nextVisitDate);

            var inspectionId = Guid.NewGuid();
            
            if(model.conclusion != Data.Enums.Conclusion.Disease)
            {
                model.nextVisitDate = null;
            }
            if (model.conclusion != Data.Enums.Conclusion.Death)
            {
                model.deathDate = null;
            }

            var prevInspectionEntity = await _context
                .Inspections
                .FirstOrDefaultAsync(x => x.id == model.previousInspectionId);

            Guid baseInspection = Guid.Empty;

            if (prevInspectionEntity == null)
            {
                baseInspection = inspectionId;
            }
            else baseInspection = prevInspectionEntity.baseInspectionId;

            await _context.Inspections.AddAsync(new Inspection
            {
                id = inspectionId,
                docId = docId,
                patientId = id,
                createTime = DateTime.Now,
                date = model.date,
                anamnesis = model.anamnesis,
                complaints = model.complaints,
                treatment = model.treatment,
                conclusion = model.conclusion,
                nextVisitDate = model.nextVisitDate,
                deathDate = model.deathDate,
                baseInspectionId = baseInspection,
                previousInspectionId = model.previousInspectionId
            });

            foreach (DiagnosisCreateModel diag in model.diagnoses)
            {
                var icdEntity = await _context
                    .Icd10s
                    .FirstOrDefaultAsync(x => x.id == diag.icdDiagnosisId);
                CheckIcd(icdEntity);

                await _context.Diagnoses.AddAsync(new Diagnosis
                {
                    id = Guid.NewGuid(),
                    createTime = DateTime.Now,
                    description = diag.description,
                    type = diag.type,
                    icd10Id = diag.icdDiagnosisId,
                    InspectionId = inspectionId
                });
            }

            var docEntity = await _context
                .Doctors
                .FirstOrDefaultAsync(x => x.id == docId);

            foreach (ConsultationCreateModel con in model.consultations)
            {
                var specEntity = await _context
                    .Specialities
                    .FirstOrDefaultAsync(x => x.id == con.specialityId);
                CheckSpec(specEntity);

                var ConsEntity = await _context
                    .Consultations
                    .Where(x => x.inspectionId == inspectionId).ToListAsync();

                foreach (var cons in ConsEntity)
                {
                    if (cons.specialityId == con.specialityId)
                    {
                        ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                        $"More than one consultation have the same speciality");
                        throw ex;
                    }
                }
                Guid conId = Guid.NewGuid();
                await _context.Consultations.AddAsync(new Consultation
                {
                    id = conId,
                    createTime = DateTime.Now,
                    inspectionId = inspectionId,
                    specialityId = con.specialityId,
                    authorId = docId,
                    comment = con.comment.content
                });

                await _context.Comments.AddAsync(new Comment
                {
                    id = Guid.NewGuid(),
                    authorId = docId,
                    createTime = DateTime.Now,
                    consultationId = conId,
                    content = con.comment.content
                });
                
            }
            await _context.SaveChangesAsync();
            return inspectionId;
        }

        public async Task<InspectionPagedListModel> GetInspectionPagedList(Guid id, bool grouped, List<int> icdRoots, int page, int size)
        {
            List<Inspection> inspecsEntity = new List<Inspection>();
            if(grouped) 
                inspecsEntity = await _context
                    .Inspections
                    .Where(x => x.patientId == id && x.id == x.baseInspectionId).ToListAsync();
            else inspecsEntity = await _context
                    .Inspections
                    .Where(x => x.patientId == id).ToListAsync();
            if(icdRoots.Count > 0)
            {
                var icdSortedInspections = new List<Inspection>();
                foreach (var inspec in inspecsEntity)
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
                inspecsEntity = icdSortedInspections;
            }
            



            if (inspecsEntity.Count == 0) {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status404NotFound.ToString(),
                    "No inspections found"
                );
                throw ex;
            }

            int start = (page - 1) * size;
            int end = Math.Min(size, inspecsEntity.Count - start);
            double count = Double.Ceiling((double)inspecsEntity.Count / size);
            if (page > count)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    "Out of page list");
                throw ex;
            }

            InspectionPagedListModel inspectionsResponeModel = new InspectionPagedListModel();
            inspectionsResponeModel.pagination = new PageInfoModel()
            {
                size = end - start,
                count = ((int)count),
                current = page
            };
            inspectionsResponeModel.inspections = new List<InspectionPreviewModel>();

            foreach (Inspection inspection in inspecsEntity.GetRange(start, end))
            {
                bool hasChain = false;
                bool hasNested = false;
                var nestedInspectionEntity = await _context
                    .Inspections
                    .FirstOrDefaultAsync(x => x.previousInspectionId == inspection.id);
                if(nestedInspectionEntity!=null)  hasNested = true; 
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

                DiagnosisModel diagnosisModel = _mapper.Map<DiagnosisModel>(diagEntity);
                diagnosisModel.code = icdEntity.code;
                diagnosisModel.name = icdEntity.name;

                inspectionsResponeModel.inspections.Add(new InspectionPreviewModel
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
                    diagnosis = diagnosisModel,
                    hasChain = hasChain,
                    hasNested = hasNested
                });
            }

            return inspectionsResponeModel;
        }

        public async Task<PatientModel> GetPatientInfo(Guid id)
        {
            var patientEntity = await _context
                    .Patients
                    .FirstOrDefaultAsync(x => x.id == id);

            if (patientEntity != null)
                return _mapper.Map<PatientModel>(patientEntity);

            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status401Unauthorized.ToString(),
                "Patient doesn't exist"
            );
            throw ex;
        }

        public async Task<PatientPagedListModel> GetPatientPagedList(Guid docId,
            string? name, Conclusion? conclusions, PatientSorting? sorting, bool sheduledVisits,
            bool onlyMine, int page, int size)
        {
            List<Patient> patientsEntity = new List<Patient>(); 

            if (name == null)
            {
                patientsEntity = await _context
                    .Patients.ToListAsync();
            }
            else
            {
                patientsEntity = await _context
                    .Patients
                    .Where(x => x.name.Contains(name)).ToListAsync();
            }

            List<Patient> patients = new List<Patient>();

            if (conclusions != null)
            {
                foreach (Patient p in patientsEntity)
                {
                    var inspectionEntity = await _context
                        .Inspections
                        .Where(x => x.patientId == p.id && x.conclusion == conclusions).ToListAsync();
                    if (inspectionEntity.Count() != 0)
                    {
                        patients.Add(p);
                    }
                }
            }
            else patients = patientsEntity;

            if (sheduledVisits == true || onlyMine == true)
            {
                List<Guid> toDelete = new List<Guid>();
                foreach (Patient p in patients)
                {
                    
                    if (sheduledVisits)
                    {
                        bool noDate = true;
                        foreach (InspectionShortModel pShort in GetChildleddInpections(p.id, null).Result)
                        {
                            var inspecEntity = await _context
                                .Inspections
                                .FirstOrDefaultAsync(x => x.nextVisitDate != null && x.id == pShort.id);
                            if (inspecEntity != null)
                            {
                                noDate = false;
                            }
                        }
                        if (noDate) toDelete.Add(p.id);
                    }
                    if (onlyMine == true)
                    {
                        var inspectionEntity = await _context
                        .Inspections
                        .Where(x => x.docId == docId && x.patientId == p.id).ToListAsync();
                        if(inspectionEntity.Count() == 0)
                        {
                            toDelete.Add(p.id);
                        }
                    }
                }
                foreach (Guid id in toDelete)
                {
                    patients.RemoveAll(x => x.id == id);
                }
            }

            if(sorting == PatientSorting.CreateAsc)
            {
                patients = patients.OrderBy(x => x.createTime).ToList();
            }
            else if (sorting == PatientSorting.CreateDesc)
            {
                patients = patients.OrderByDescending(x => x.createTime).ToList();
            }
            else if (sorting == PatientSorting.NameAsc)
            {
                patients = patients.OrderBy(x => x.name).ToList();
            }
            else if (sorting == PatientSorting.NameDesc)
            {
                patients = patients.OrderByDescending(x => x.name).ToList();
            }
            else if (sorting == PatientSorting.InspectionAsc)
            {
                List<LastInspectionSortingModel> lastInspectionModel = new List<LastInspectionSortingModel>();
                foreach(Patient patient in patients)
                {
                    List<InspectionShortModel> pShort = GetChildleddInpections(patient.id, null).Result.ToList();
                    
                    if (pShort.Count != 0)
                    {
                        DateTime lastInspection = pShort.OrderByDescending(x => x.createTime).First().createTime;
                        lastInspectionModel.Add(new LastInspectionSortingModel
                        {
                            patient = patient,
                            lastInspection = lastInspection
                        });
                    }
                    else lastInspectionModel.Add(new LastInspectionSortingModel
                    {
                        patient = patient,
                        lastInspection = DateTime.MinValue
                    });
                }
                lastInspectionModel = lastInspectionModel.OrderBy(x => x.lastInspection).ToList();
                patients.Clear();
                foreach (LastInspectionSortingModel sortedPatients in lastInspectionModel)
                {
                    patients.Add(sortedPatients.patient);
                }
            }
            else if (sorting == PatientSorting.InspectionDesc)
            {
                List<LastInspectionSortingModel> lastInspectionModel = new List<LastInspectionSortingModel>();
                foreach (Patient patient in patients)
                {
                    List<InspectionShortModel> pShort = GetChildleddInpections(patient.id, null).Result.ToList();

                    if (pShort.Count != 0)
                    {
                        DateTime lastInspection = pShort.OrderByDescending(x => x.createTime).First().createTime;
                        lastInspectionModel.Add(new LastInspectionSortingModel
                        {
                            patient = patient,
                            lastInspection = lastInspection
                        });
                    }
                    else lastInspectionModel.Add(new LastInspectionSortingModel
                    {
                        patient = patient,
                        lastInspection = DateTime.MinValue
                    });
                }
                lastInspectionModel = lastInspectionModel.OrderByDescending(x => x.lastInspection).ToList();
                patients.Clear();
                foreach (LastInspectionSortingModel sortedPatients in lastInspectionModel)
                {
                    patients.Add(sortedPatients.patient);
                }
            }

            if (patients.Count == 0)
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                    "Invalid value for attribute page");
                throw ex;
            }

            int start = (page - 1) * size;
            int end = Math.Min(size, patients.Count - start);
            double count = Double.Ceiling((double)patients.Count / size);
            if(page > count) 
            {
                var ex = new Exception();
                ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                    "Out of page list");
                throw ex;
            }

            PatientPagedListModel patientPagedList = new PatientPagedListModel();

            List<PatientModel> pats = new List<PatientModel>();
            PageInfoModel pagination = new PageInfoModel();
            patientPagedList.patients = pats.ToList();
            patientPagedList.pagination = pagination;

            patientPagedList.pagination.size = size;
            patientPagedList.pagination.count = ((int)count);
            patientPagedList.pagination.current = page;

            foreach(Patient p in patients.GetRange(start, end))
            {
                patientPagedList.patients.Add(_mapper.Map<PatientModel>(p));
            }
            return patientPagedList;
        }
        private static void CheckBirthDate(DateTime? birthDate)
        {
            if (birthDate == null || birthDate <= DateTime.Now) return;

            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                "Birth date can't be later than today");
            throw ex;
        }
        private static void CheckNextVisitDate(DateTime? visitDate)
        {
            if (visitDate == null || visitDate >= DateTime.Now) return;

            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status409Conflict.ToString(),
                "Visit date can't be earlier than now");
            throw ex;
        }
        private static void CheckIcd(Icd10 icd)
        {
            if (icd != null) return;

            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                "There is no icd with this id");
            throw ex;
        }
        private static void CheckSpec(Speciality spec)
        {
            if (spec != null) return;

            var ex = new Exception();
            ex.Data.Add(StatusCodes.Status400BadRequest.ToString(),
                $"There is no speciality with such id: {spec.id}");
            throw ex;
        }

        public async Task<List<InspectionShortModel>> GetChildleddInpections(Guid id, string? request)
        {
            var inspectionsEntity = await _context
                    .Inspections
                    .Where(x => x.patientId == id).ToListAsync();
            if (inspectionsEntity.Count == 0)
            {
                return new List<InspectionShortModel>();
            }
            var baseInspectionsEntity = await _context
                    .Inspections
                    .Where(x => x.id == x.baseInspectionId && x.patientId == id).ToListAsync();

            List<Inspection> insList = new List<Inspection>();
            foreach (Inspection inspection in baseInspectionsEntity)
            {
                var inspectionEntity = await _context
                    .Inspections
                    .Where(x => x.baseInspectionId == inspection.id).ToListAsync();
                inspectionEntity = inspectionEntity.OrderByDescending(x => x.createTime).ToList();
                insList.Add(inspectionEntity.First());
            }
            List<InspectionShortModel> result = new List<InspectionShortModel>();
            foreach(Inspection ins in insList)
            {
                InspectionShortModel partResult = new InspectionShortModel();
                partResult.id = ins.id;
                partResult.createTime = ins.createTime;
                partResult.date = ins.date;
                var diagEntity = await _context
                    .Diagnoses
                    .FirstOrDefaultAsync(x => x.InspectionId == ins.id);
                var icdEntity = await _context
                    .Icd10s
                    .FirstOrDefaultAsync(x => x.id == diagEntity.icd10Id);

                DiagnosisModel diagnosisModel = _mapper.Map<DiagnosisModel>(diagEntity);
                diagnosisModel.code = icdEntity.code;
                diagnosisModel.name = icdEntity.name;

                partResult.diagnosis = diagnosisModel;
                result.Add(partResult);
            }
            if(request != null)
            {
                result = result.Where(x => x.diagnosis.code.ToString().StartsWith(request) || x.diagnosis.name.StartsWith(request)).ToList();
            }
            return result;
        }
    }
}

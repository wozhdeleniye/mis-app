using AutoMapper;
using MISBack.Data.Entities;
using MISBack.Data.Models;

namespace MISBack.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Doctor, DoctorModel>();
            CreateMap<Diagnosis, DiagnosisModel>();
            CreateMap<DiagnosisCreateModel, Diagnosis>();
            CreateMap<Patient, PatientModel>();
            CreateMap<Icd10, Icd10RecordModel>();
            CreateMap<Speciality, SpecialityModel>();
            CreateMap<Comment, CommentModel>();
            CreateMap<Comment, InspectionCommentModel>();
        }
    }
}

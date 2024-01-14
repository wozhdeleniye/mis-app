using MISBack.Data.Enums;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces
{
    public interface IPatientsService
    {
        Task<Guid> CreatePatient(PatientCreateModel patientModel);
        Task<PatientPagedListModel> GetPatientPagedList(Guid docId, string? name, Conclusion? conclusions, PatientSorting? sorting, bool sheduledVisits, bool onlyMine, int page, int size);
        Task<Guid> CreateInspection(Guid id, InspectionCreateModel model, Guid docId);
        Task<InspectionPagedListModel> GetInspectionPagedList(Guid id, bool grouped, List<int> icdRoots, int page, int size);
        Task<PatientModel> GetPatientInfo(Guid id);
        Task<List<InspectionShortModel>> GetChildleddInpections(Guid id, string? request);
    }
}

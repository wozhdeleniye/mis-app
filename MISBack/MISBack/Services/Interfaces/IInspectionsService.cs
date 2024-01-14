using MISBack.Data.Models;

namespace MISBack.Services.Interfaces
{
    public interface IInspectionsService
    {
        Task<InspectionModel> GetInspection(Guid id);
        Task EditInspection(Guid id, InspectionEditModel inspectionModel, Guid docId);
        Task<List<InspectionPreviewModel>> GetInspectionChain(Guid id);
    }
}

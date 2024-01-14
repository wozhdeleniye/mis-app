using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces
{
    public interface IConsultationsService
    {
        Task<InspectionPagedListModel> GetInspections(List<int>? icdRoots, bool grouped, int page, int size, Guid docId);
        Task<ConsultationModel> GetConsultation(Guid id);
        Task<Guid> AddComment(Guid id, CommentCreateModel commentModel, Guid docId);
        Task EditComment(Guid id, InspectionCommentCreateModel commentModel);
    }
}

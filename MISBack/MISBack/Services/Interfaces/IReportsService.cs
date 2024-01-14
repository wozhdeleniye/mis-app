using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;

namespace MISBack.Services.Interfaces
{
    public interface IReportsService
    {
        Task<IcdRootsRepotModel> GetInspections(DateTime start, DateTime end, List<int> icdRoots);
    }
}

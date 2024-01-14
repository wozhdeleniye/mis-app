using MISBack.Data.Models;

namespace MISBack.Services.Interfaces
{
    public interface IDictionariesService
    {
        Task<SpecialtiesPagedListModel> GetSpecialities(string name, int page, int size);
        Task<Icd10SearchModel> SearchIcd10(string request, int page, int size);
        Task<List<Icd10RecordModel>> GetRootIcd();
    }
}

using MISBack.Data.Models;

namespace MISBack.Services.Interfaces
{
    public interface IDoctorsService
    {
        Task<TokenResponseModel> RegisterDoc(DoctorRegisterModel docRegisterDto);
        Task<TokenResponseModel> LoginDoc(LoginCredentialsModel credentials);
        Task Logout(string token);
        Task<DoctorModel> GetDocProfile(Guid docId);
        Task EditDocProfile(Guid docId, DoctorEditModel docEditModel);
    }
}

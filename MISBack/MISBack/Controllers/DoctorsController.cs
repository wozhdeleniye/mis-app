using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MISBack.Controllers
{
    [ApiController]
    [Route("api/doctor")]
    public class DoctorsController : ControllerBase
    {
        private readonly IDoctorsService _docsService;

        public DoctorsController(IDoctorsService docsService)
        {
            _docsService = docsService;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterDoctor([FromBody] DoctorRegisterModel docRegisterForm)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var newDoctor = await _docsService.RegisterDoc(docRegisterForm);
            return Ok(newDoctor);
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginDoctor([FromBody] LoginCredentialsModel docLoginForm)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var doctor = await _docsService.LoginDoc(docLoginForm);
            return Ok(doctor);
        }

        [HttpPost]
        [Route("logout")]
        public async Task LogoutDoctor()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            if (token == null)
            {
                throw new Exception("Token not found");
            }

            await _docsService.Logout(token);
        }

        [HttpGet]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        [Route("profile")]
        [SwaggerOperation(Summary = "Get user profile")]
        public async Task<DoctorModel> GetDoctorProfile()
        {
            return await _docsService.GetDocProfile(
                Guid.Parse(User.Identity.Name));
        }

        [HttpPut]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        [Route("profile")]
        [SwaggerOperation(Summary = "Edit user Profile")]
        public async Task<IActionResult> EditDoctorProfile([FromBody] DoctorEditModel docEditModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _docsService.EditDocProfile(
                Guid.Parse(User.Identity.Name), docEditModel);
            return Ok();
        }
    }
}

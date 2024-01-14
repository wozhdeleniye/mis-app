using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Entities;
using MISBack.Data.Enums;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers
{
    [ApiController]
    [Route("api/patient")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientsService _patsService;

        public PatientsController(IPatientsService patsService)
        {
            _patsService = patsService;
        }

        [HttpPost]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> CreatePatient([FromBody] PatientCreateModel patientModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var patient = await _patsService.CreatePatient(patientModel);
            return Ok(patient);

        }

        [HttpGet]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> GetPatients(
            [FromQuery] Conclusion? conclusions, PatientSorting? sorting, string name = "", bool sheduledVisits = false, bool onlyMine = false, int page = 1, int size = 5)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var patient = await _patsService.GetPatientPagedList(Guid.Parse(User.Identity.Name), name, conclusions, sorting, sheduledVisits, onlyMine, page, size);
            return Ok(patient);
        }

        [HttpPost]
        [Route("{id}/inspections")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> CreateInspection([FromRoute] Guid id, [FromBody] InspectionCreateModel InspectionModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inspec = await _patsService.CreateInspection(id, InspectionModel, Guid.Parse(User.Identity.Name));
            return Ok(inspec);
        }

        [HttpGet]
        [Route("{id}/inspections")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> GetInspections([FromRoute] Guid id, [FromQuery] List<int>? icdRoots, [FromQuery] bool grouped = false,  int page = 1, int size = 5)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState); 
            var inspec = await _patsService.GetInspectionPagedList(id, grouped, icdRoots, page, size);
            return Ok(inspec);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> GetPatientInfo([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var patient = await _patsService.GetPatientInfo(id);
            return Ok(patient);
        }

        [HttpGet]
        [Route("{id}/inspections/search")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> GetChildleddInpections([FromRoute] Guid id, [FromQuery] string? request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inspecs = await _patsService.GetChildleddInpections(id, request);
            return Ok(inspecs);
        }
    }
}

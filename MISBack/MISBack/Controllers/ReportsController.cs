using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers
{
    [ApiController]
    [Route("api/report/icdrootsreport")]
    public class ReportsController : ControllerBase
    {
        private readonly IReportsService _repsService;

        public ReportsController(IReportsService repsService)
        {
            _repsService = repsService;
        }

        [HttpGet]
        public async Task<IActionResult> CreatePatient([FromQuery] DateTime start, DateTime end,[FromQuery] List<int> icdRoots)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var patient = await _repsService.GetInspections(start, end, icdRoots);
            return Ok(patient);
        }
    }
}

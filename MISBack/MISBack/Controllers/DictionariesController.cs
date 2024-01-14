using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;
using System.Xml.Linq;

namespace MISBack.Controllers
{
    [ApiController]
    [Route("api/dictionary")]
    public class DictionariesController : ControllerBase
    {
        private readonly IDictionariesService _dicsService;

        public DictionariesController(IDictionariesService dicsService)
        {
            _dicsService = dicsService; 
        }
        [HttpGet]
        [Route("speciality")]
        public async Task<IActionResult> GetSpecialities([FromQuery] string name = "", int page = 1, int size = 5)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var specs = await _dicsService.GetSpecialities(name, page, size);
            return Ok(specs);
        }

        [HttpGet]
        [Route("icd10")]
        public async Task<IActionResult> SearchIcd10([FromQuery] string request = "", int page = 1, int size = 5)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var icds = await _dicsService.SearchIcd10(request, page, size);
            return Ok(icds);
        }

        [HttpGet]
        [Route("icd10/roots")]
        public async Task<IActionResult> GetRootIcd()
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var icds = await _dicsService.GetRootIcd();
            return Ok(icds);
        }
    }
}

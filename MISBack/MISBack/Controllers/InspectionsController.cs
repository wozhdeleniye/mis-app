using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;

namespace MISBack.Controllers
{
    [ApiController]
    [Route("api/inspection")]
    public class InspectionsController : ControllerBase
    {
        private readonly IInspectionsService _inspecsService;

        public InspectionsController(IInspectionsService inspecsService)
        {
            _inspecsService = inspecsService;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> CreateInspection([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inspec = await _inspecsService.GetInspection(id);
            return Ok(inspec);
        }

        [HttpPut]
        [Route("{id}")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> EditInspection([FromRoute] Guid id, [FromBody] InspectionEditModel inspectionModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _inspecsService.EditInspection(id, inspectionModel, Guid.Parse(User.Identity.Name));
            return Ok();
        }

        [HttpGet]
        [Route("{id}/chain")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> GetInspectionChain([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inspecList = await _inspecsService.GetInspectionChain(id);
            return Ok(inspecList);
        }
    }
}

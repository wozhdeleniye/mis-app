using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MISBack.Data.Models;
using MISBack.Services.Interfaces;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace MISBack.Controllers
{
    [ApiController]
    [Route("api/consultation")]
    public class ConsultationsController : ControllerBase
    {
        private readonly IConsultationsService _consService;

        public ConsultationsController(IConsultationsService consService)
        {
            _consService = consService;
        }

        [HttpGet]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> GetInspections([FromQuery] List<int>? icdRoots, bool grouped = false, int page = 1, int size = 5)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var inspecs = await _consService.GetInspections(icdRoots, grouped, page, size, Guid.Parse(User.Identity.Name));
            return Ok(inspecs);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> GetConsultation([FromRoute] Guid id)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var cons = await _consService.GetConsultation(id);
            return Ok(cons);
        }

        [HttpPost]
        [Route("{id}/comment")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> AddComment([FromRoute] Guid id, [FromBody] CommentCreateModel commentModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var comm = await _consService.AddComment(id, commentModel, Guid.Parse(User.Identity.Name));
            return Ok(comm);
        }

        [HttpPut]
        [Route("comment/{id}")]
        [Authorize]
        [Authorize(Policy = "ValidateToken")]
        public async Task<IActionResult> EditComment([FromRoute] Guid id, [FromBody] InspectionCommentCreateModel commentModel)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _consService.EditComment(id, commentModel);
            return Ok();
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.CQRS.Projects;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProjectsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/projects/raw
        [HttpGet("raw")]
        public async Task<IActionResult> GetRawProjects()
        {
            var list = await _mediator.Send(new GetRawProjectsQuery());
            return Ok(list);
        }

        // GET: api/projects?lang=tr
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] string lang = "tr")
        {
            var list = await _mediator.Send(new GetProjectsQuery(lang));
            return Ok(list);
        }

        // GET: api/projects/platar-lpr?lang=tr
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(string id, [FromQuery] string lang = "tr")
        {
            try
            {
                var project = await _mediator.Send(new GetProjectByIdQuery(id, lang));
                return Ok(project);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/projects
        [HttpPost]
        public async Task<IActionResult> CreateProject([FromBody] ProjectEntity project)
        {
            try
            {
                var created = await _mediator.Send(new CreateProjectCommand(project));
                return CreatedAtAction(nameof(GetProject), new { id = created.Id }, created);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        // PUT: api/projects/platar-lpr
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProject(string id, [FromBody] ProjectEntity updatedProject)
        {
            try
            {
                var updated = await _mediator.Send(new UpdateProjectCommand(id, updatedProject));
                return Ok(updated);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // PUT: api/projects/reorder
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderProjects([FromBody] List<ProjectReorderItem> items)
        {
            await _mediator.Send(new ReorderProjectsCommand(items));
            return Ok();
        }

        // DELETE: api/projects/platar-lpr
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(string id)
        {
            try
            {
                await _mediator.Send(new DeleteProjectCommand(id));
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.CQRS.Articles;
using Server.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ArticlesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArticlesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/articles/raw
        [HttpGet("raw")]
        public async Task<IActionResult> GetRawArticles()
        {
            var list = await _mediator.Send(new GetRawArticlesQuery());
            return Ok(list);
        }

        // GET: api/articles?lang=tr
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetArticles([FromQuery] string lang = "tr")
        {
            var list = await _mediator.Send(new GetArticlesQuery(lang));
            return Ok(list);
        }

        // GET: api/articles/saas-multitenancy?lang=tr
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetArticle(string id, [FromQuery] string lang = "tr")
        {
            try
            {
                var article = await _mediator.Send(new GetArticleByIdQuery(id, lang));
                return Ok(article);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/articles
        [HttpPost]
        public async Task<IActionResult> CreateArticle([FromBody] ArticleEntity article)
        {
            try
            {
                var created = await _mediator.Send(new CreateArticleCommand(article));
                return CreatedAtAction(nameof(GetArticle), new { id = created.Id }, created);
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

        // PUT: api/articles/saas-multitenancy
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArticle(string id, [FromBody] ArticleEntity updatedArticle)
        {
            try
            {
                var updated = await _mediator.Send(new UpdateArticleCommand(id, updatedArticle));
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

        // PUT: api/articles/reorder
        [HttpPut("reorder")]
        public async Task<IActionResult> ReorderArticles([FromBody] List<ArticleReorderItem> items)
        {
            await _mediator.Send(new ReorderArticlesCommand(items));
            return Ok();
        }

        // DELETE: api/articles/saas-multitenancy
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(string id)
        {
            try
            {
                await _mediator.Send(new DeleteArticleCommand(id));
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // POST: api/articles/{id}/publish-medium
        [HttpPost("{id}/publish-medium")]
        public async Task<IActionResult> PublishToMedium(string id, [FromQuery] string? token = null)
        {
            var res = await _mediator.Send(new PublishMediumCommand(id, token));
            if (!res.Success)
            {
                return BadRequest(new { message = res.Message });
            }
            return Ok(new { success = true, url = res.Url, message = res.Message });
        }
    }
}

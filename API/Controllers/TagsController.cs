using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var tags = await _tagService.GetAllTagsAsync();
            return Ok(tags);
        }

        [HttpPost]
        public async Task<IActionResult> CreateTag([FromBody] CreateTagDto dto)
        {
            try
            {
                var tag = await _tagService.CreateTagAsync(dto);
                return CreatedAtAction(nameof(GetAll), new { id = tag.Id }, tag);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var success = await _tagService.DeleteTagAsync(id);
            if (!success) return NotFound();
            return NoContent();
        }

        [HttpPost("api/photos/{photoId}/tags")]
        public async Task<IActionResult> AddTagsToPhoto(int photoId, [FromBody] List<string> tags)
        {
            try
            {
                await _tagService.AddTagsToPhotoAsync(photoId, tags);
                return Ok();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
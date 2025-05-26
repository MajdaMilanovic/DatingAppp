using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Authorize]
    public class PhotoController(UnitOfWork unitOfWork, IMapper mapper) : BaseApiController
    {
        [HttpGet("filter-by-tags")]

        public async Task<ActionResult<IEnumerable<PhotoWithTagsDto>>> GetPhotosByTags([FromQuery] List<string> tags)
        {

            if (tags == null || !tags.Any())
                return BadRequest("At least one tag is required.");

            var photos = await unitOfWork.PhotoRepository.GetPhotosByTagsAsync(tags);
            return Ok(mapper.Map<IEnumerable<PhotoWithTagsDto>>(photos));

        }

        [Authorize(Policy = "ModeratePhotoRole")]

        [HttpGet("unapproved-by-tags")]

        public async Task<ActionResult<IEnumerable<PhotoWithTagsDto>>> GetUnapprovedPhotosByTags([FromQuery] List<string> tags)

        {

            if (tags == null || !tags.Any())

                return BadRequest("At least one tag is required.");



            var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotosByTagsAsync(tags);



            return Ok(mapper.Map<IEnumerable<PhotoWithTagsDto>>(photos));
        }

        
    }
}
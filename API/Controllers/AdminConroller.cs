using System;
using System.Data;
using System.Text.RegularExpressions;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminConroller(UserManager<AppUser> userManager,
 IUnitOfWork unitOfWork,
 IPhotoService photoService, IMapper mapper,
 IConfiguration configuration) : BaseApiController
{
    private readonly IConfiguration configuration = configuration;

    [Authorize("RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult> GetUsersWithRoles()
    {
        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x => new
        {
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        }).ToListAsync();

        return Ok(users);
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if (string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");
        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if (user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!result.Succeeded) return BadRequest("Failed to add roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!result.Succeeded) return BadRequest("Failed to remove from roles");
        return Ok(await userManager.GetRolesAsync(user));
    }


    [Authorize("ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")]
    public async Task<ActionResult> GetPhotosForModeration()
    {

        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("approvePhoto/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return BadRequest("Couldnt get a photo");

        photo.IsApproved = true;

        var user = await unitOfWork.UserRepository.GetUserByPhotoId(photoId);

        if(user == null) return BadRequest("Couldnt find user");

        if(!user.Photos.Any(x => x.IsMain)) photo.IsMain = true;

        await unitOfWork.Complete();

        return Ok();
    }

    [Authorize(Policy = "ModeratePhotoRole")]
    [HttpPost("rejectPhoto/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if (photo == null) return BadRequest("Couldnt get a photo");

        if (photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);

            if (result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else
        {
            unitOfWork.PhotoRepository.RemovePhoto(photo);
        }

        await unitOfWork.Complete();

        return Ok();

    }

    [Authorize(Policy = "RequireAdminRole")]

    [HttpGet("tags")]

    public async Task<ActionResult<IEnumerable<TagDto>>> GetAllTags()

    {

        var tags = await unitOfWork.TagRepository.GetAllTagsAsync();

        return Ok(mapper.Map<IEnumerable<TagDto>>(tags));

    }

    [Authorize(Policy = "RequireAdminRole")]

    [HttpPost("add-tag")]

    public async Task<ActionResult<TagDto>> AddTag([FromBody] TagDto tagDto)
    {
        if (string.IsNullOrWhiteSpace(tagDto.Name))
        { return BadRequest("Tag name is required."); }

        var tagName = tagDto.Name.Trim();

        if (!Regex.IsMatch(tagName, @"^[a-zA-Z0-9\s\-]{2,30}$"))
        { return BadRequest("Tag name contains invalid characters or is too short/long."); }

        var existingTags = await unitOfWork.TagRepository.GetAllTagsAsync();

        if (existingTags.Any(t => t.Name.Equals(tagName, StringComparison.OrdinalIgnoreCase)))
        { return BadRequest("A tag with the same name already exists."); }

        var tag = new Tag { Name = tagName };

        await unitOfWork.TagRepository.AddTagAsync(tag);

        return Ok(mapper.Map<TagDto>(tag));

    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpDelete("delete-tag/{id:int}")]

    public async Task<ActionResult> DeleteTag(int id)

    {
        var tag = await unitOfWork.TagRepository.GetTagByIdAsync(id);

        if (tag == null) return NotFound("Tag not found");


        await unitOfWork.TagRepository.DeleteTagAsync(tag);


        return NoContent();

    }


}

using System;
using API.Data;
using API.Entities;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

public class AdminConroller(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,   IPhotoService photoService) : BaseApiController
{
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

    [Authorize(Policy ="RequireAdminRole")]
    [HttpPost("edit-roles/{username}")]
    public async Task<ActionResult> EditRoles(string username, string roles)
    {
        if(string.IsNullOrEmpty(roles)) return BadRequest("you must select at least one role");
        var selectedRoles = roles.Split(",").ToArray();

        var user = await userManager.FindByNameAsync(username);

        if(user == null) return BadRequest("User not found");

        var userRoles = await userManager.GetRolesAsync(user);

        var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if(!result.Succeeded) return BadRequest("Failed to add roles");

        result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if(!result.Succeeded) return BadRequest("Failed to remove from roles");
        return Ok(await userManager.GetRolesAsync(user));
    }


    [Authorize("ModeratePhotoRole")]
    [HttpGet("photos-to-moderate")] 
    public async Task<ActionResult> GetPhotosForModeration()
    {

        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();
        return Ok(photos);
    }

    [Authorize(Policy ="ModeratePhotoRole")]
    [HttpPost("approvePhoto/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if(photo == null) return BadRequest("Couldnt get a photo");

        photo.IsApproved = true;

        await unitOfWork.Complete();

        return Ok();
    }

    [Authorize(Policy ="ModeratePhotoRole")]
    [HttpPost("rejectPhoto/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if(photo == null) return BadRequest("Couldnt get a photo");

        if(photo.PublicId != null)
        {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);

            if(result.Result == "ok")
            {
                unitOfWork.PhotoRepository.RemovePhoto(photo);
            }
        }
        else 
        {
            await unitOfWork.Complete();
        }

        return Ok();
        
    }


}

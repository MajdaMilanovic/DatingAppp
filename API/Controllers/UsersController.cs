using System.Security.Claims;
using System.Security.Cryptography.Xml;
using System.Text.Json;
using System.Text.RegularExpressions;
using API.Controllers;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[Authorize]
public class UsersController(IUnitOfWork unitOfWork, 
IMapper mapper, IPhotoService photoService, DataContext context) : BaseApiController
{


    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDto>>>GetUsers([FromQuery]UserParams userParams)
    {
        var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);
        Response.AddPaginationHeader(users);
        return Ok(users);
    }

   
    [HttpGet("{username}")]  
    public async Task<ActionResult<MemberDto>>GetUser(string username)
    {
        var currentUsername = User.GetUsername();
        var user = await unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser: currentUsername == username);

        if(user== null) return NotFound();

        return user;
    }

    [HttpPut]
    public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto) 
    {
      
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return BadRequest("Could not find user");
        mapper.Map(memberUpdateDto, user);

        if(await unitOfWork.Complete()) return NoContent();

        return BadRequest("Failed to update the user");
    } 

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file, [FromForm] string? tagNames)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return BadRequest("Cannot update user");

        var result = await photoService.AddPhotoAsync(file);
        if(result.Error != null) return BadRequest(result.Error.Message);

        var photo = new Photo {
            Url = result.SecureUrl.AbsoluteUri,
            PublicId = result.PublicId
        };

        user.Photos.Add(photo);

        if(await unitOfWork.Complete()) return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, mapper.Map<PhotoDto>(photo));

        if (!string.IsNullOrWhiteSpace(tagNames))
        {
            var tagList = JsonSerializer.Deserialize<List<string>>(tagNames);
            foreach (var name in tagList!)
            {
                var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
                if (tag == null)
                {
                    tag = new Tag { Name = name };
                    context.Tags.Add(tag);
                    await context.SaveChangesAsync();
                }
                context.PhotoTags.Add(new PhotoTag
                {
                    PhotoId = photo.Id,
                    TagId = tag.Id
                });
            }
            await context.SaveChangesAsync();

            return Ok(new { photo.Id });
        }

            return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId:int}")]
    public async Task<ActionResult> SetMainPhoto(int photoId)
    {
        var user = await  unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        if(user == null) return BadRequest("Could not find user");

        var photo = user.Photos.FirstOrDefault( x => x.Id == photoId);
        if(photo == null || photo.IsMain) return BadRequest("Cannot use this as main photo");

        var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);
        if(currentMain != null) currentMain.IsMain = false;

        photo.IsMain = true;

        if(await unitOfWork.Complete()) return NoContent();

        return BadRequest("Problem setting main photo");

    }
     [HttpPost("add-photo-with-tags")]
        public async Task<ActionResult<PhotoWithTagsDto>> AddPhotoWithTags(IFormFile file,
            [FromQuery] List<string> tags)
        {

            var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

            if (user == null) return BadRequest("Cannot update user");



            var result = await photoService.AddPhotoAsync(file);

            if (result.Error != null) return BadRequest(result.Error.Message);



            var photo = new Photo

            {

                Url = result.SecureUrl.AbsoluteUri,

                PublicId = result.PublicId

            };

        var validTags = tags.Where(t => !string.IsNullOrWhiteSpace(t))
        .Select(t => t.Trim().ToLower()).Distinct().ToList();

        foreach (var tagName in validTags)
        {
            if(!Regex.IsMatch(tagName, @"^[a-zA-Z0-9\s\-]{2,30}$"))
                return BadRequest($"Invalid tag: {tagName}");
        }

        foreach (var tagName in validTags)
        {
            var tag = await unitOfWork.TagRepository.GetOrCreateTagAsync(tagName);

            photo.PhotoTags.Add(new PhotoTag { Photo = photo, Tag = tag });
        }
            user.Photos.Add(photo);



            if (await unitOfWork.Complete())

            {

                return CreatedAtAction(nameof(GetUser),

                    new { username = user.UserName },
                    mapper.Map<PhotoWithTagsDto>(photo)
                    );

            }
            return BadRequest("Problem adding photo");
        }
    [HttpDelete("delete-photo/{photoId:int}")]
    public async Task<ActionResult> DeletePhoto ( int photoId)
    {
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

        if(user == null) return BadRequest("User not found");

        var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoId);

        if(photo == null || photo.IsMain) return BadRequest("This photo cannot be deleted");  

        if(photo.PublicId != null) {
            var result = await photoService.DeletePhotoAsync(photo.PublicId);
            if(result.Error != null) return BadRequest(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if(await unitOfWork.Complete()) return Ok();

        return BadRequest("Problem deleting photo");
    }

}
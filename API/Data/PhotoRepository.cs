using System;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class PhotoRepository(DataContext context) : IPhotoRepository
{
    public async Task<Photo?> GetPhotoById(int id)
    {
        return await context.Photos
        .IgnoreQueryFilters()
        .SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<PhotoWithTagsDto>> GetUnapprovedPhotos()
    {
        return await context.Photos
        .IgnoreQueryFilters()
        .Where(p => p.IsApproved == false)
        .Include(p=>p.PhotoTags)
                .ThenInclude(pt=>pt.Tag)
                .Select(u => new PhotoWithTagsDto
                {
                    Id = u.Id,
                    Url = u.Url,
                    IsApproved = u.IsApproved,
                    Tags=u.PhotoTags.Select(pt=> new TagDto
                    {
                        Id=pt.TagId,
                        Name=pt.Tag.Name
                    }).ToList()
                }).ToListAsync();
    }

    public async Task<IEnumerable<Photo>> GetPhotosByTagsAsync(List<string> tagNames)

        {

            return await context.Photos

                .Where(p => p.IsApproved)
                .Where(p => p.PhotoTags.Any(pt => tagNames.Contains(pt.Tag.Name)))
                .Include(p => p.PhotoTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();

        }
         public async Task<IEnumerable<Photo>> GetUnapprovedPhotosByTagsAsync(List<string> tagNames)

        {

            return await context.Photos

                .IgnoreQueryFilters()
                .Where(p => !p.IsApproved)
                .Where(p => p.PhotoTags.Any(pt => tagNames.Contains(pt.Tag.Name)))
                .Include(p => p.PhotoTags)
                .ThenInclude(pt => pt.Tag)
                .ToListAsync();
        }


    public void RemovePhoto(Photo photo)
    {
        context.Photos.Remove(photo);
    }
    public async Task<AppUser?> GetUserByPhotoId(int photoId)
    {
        return await context.Users
        .Include(p => p.Photos)
        .IgnoreQueryFilters()
        .Where(p => p.Photos.Any(p => p.Id == photoId))
        .FirstOrDefaultAsync();
    }

    public async void SaveChangesAsync()
    {
        await context.SaveChangesAsync();
    }
}

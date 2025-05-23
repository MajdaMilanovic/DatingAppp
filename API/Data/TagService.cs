using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class TagService(DataContext context) : ITagService
    {

        public async Task AddTagsToPhotoAsync(int photoId, List<string> tagNames)
        {
            var photo = await context.Photos.Include(p => p.PhotoTags).FirstOrDefaultAsync(p => p.Id == photoId);

            if (photo == null)
                throw new KeyNotFoundException("Photo not found");

            foreach (var name in tagNames)
            {
                var tag = await context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());

                if (tag == null)
                {
                    tag = new Tag { Name = name };
                    context.Tags.Add(tag);
                    await context.SaveChangesAsync();
                }
                if (!photo.PhotoTags.Any(p => p.TagId == tag.Id))
                {
                    photo.PhotoTags.Add(new PhotoTag { PhotoId = photoId, TagId = tag.Id });
                }
            }
            await context.SaveChangesAsync();
        }

        public async Task<TagDto> CreateTagAsync(CreateTagDto dto)
        {
            var test = await context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == dto.Name.ToLower());

            if (test != null)
                throw new InvalidOperationException("Tag already exists");


            var tag = new Tag { Name = dto.Name };
            context.Tags.Add(tag);
            await context.SaveChangesAsync();

            return new TagDto { Id = tag.Id, Name = tag.Name };
        }

        public async Task<bool> DeleteTagAsync(int id)
        {
            var tag = await context.Tags.FindAsync(id);
            if (tag == null) return false;

            context.Tags.Remove(tag);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<TagDto>> GetAllTagsAsync()
        {
            return await context.Tags.Select(t => new TagDto { Id = t.Id, Name = t.Name }).ToListAsync();
        }
    }
}
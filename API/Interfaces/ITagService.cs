using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface ITagService
    {
        Task<IEnumerable<TagDto>> GetAllTagsAsync();
        Task<TagDto> CreateTagAsync(CreateTagDto dto);
        Task<bool> DeleteTagAsync(int id);
        Task AddTagsToPhotoAsync(int photoId, List<string> tagNames);
    }
}
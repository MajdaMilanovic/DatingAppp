using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Interfaces
{
    public interface ITagRepository
    {
        Task<Tag> GetOrCreateTagAsync(string tagName);
        Task<IEnumerable<Tag>> GetAllTagsAsync();
        Task<Tag?> GetTagByIdAsync(int id);
        Task AddTagAsync(Tag tag);
        Task DeleteTagAsync(Tag tag);
    }
}
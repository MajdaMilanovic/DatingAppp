using System;
using API.DTOs;
using API.Entities;

namespace API.Interfaces;

public interface IPhotoRepository
{
    Task<IEnumerable<PhotoWithTagsDto>> GetUnapprovedPhotos();
    Task<IEnumerable<Photo>> GetPhotosByTagsAsync(List<string> tagNames);
    Task<IEnumerable<Photo>> GetUnapprovedPhotosByTagsAsync(List<string> tagNames);
    Task<Photo?> GetPhotoById(int id);
    void RemovePhoto(Photo photo);
    Task<AppUser?> GetUserByPhotoId(int photoId);
     Task<IEnumerable<PhotoWithTagsDto>> GetApprovedPhotos();
    void SaveChangesAsync();
}

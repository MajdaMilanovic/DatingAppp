using System;
using API.Data;
using API.Helpers;
using API.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;

namespace API.Services;

public class PhotoService : IPhotoService
{
    private readonly Cloudinary _cloudinary;

    public PhotoService(IOptions<CloudinarySettings> config)
    {
        var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(acc);

    }

    // public PhotoService(IPhotoRepository @object)
    // {
    //     this.@object = @object;
    // }

    public async Task<ImageUploadResult> AddPhotoAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation()
                .Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "da-net8"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);

        }
        return uploadResult;

    }

    public async Task<DeletionResult> DeletePhotoAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);

        return await _cloudinary.DestroyAsync(deleteParams);
    }
    // public async Task ApprovePhotoAsync(int photoId)
    // {
    //     var photo = await _repository.GetPhotoById(photoId);
    //     if (photo == null) throw new Exception("Photo not found");

    //     photo.IsApproved = true;

    //     var user = await _userRepository.GetUserByPhotoId(photoId);
    //     if (user.MainPhotoId == null)
    //     {
    //         user.MainPhotoId = photoId;
    //     }

    //     await _unitOfWork.Complete();
    // }
}


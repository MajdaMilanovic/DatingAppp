using System;

namespace API.Interfaces;

public interface IUnitOfWork
{
    IUserRepository UserRepository {get;}
    IMessageRepository MessageRepository {get;}
    ILikesRepository LikesRepository {get;}
    IPhotoRepository PhotoRepository {get;}
    ITagRepository TagRepository { get; }
    IAdminRepository AdminRepository { get; }
    Task<bool> Complete();
    bool HasChanges();
}

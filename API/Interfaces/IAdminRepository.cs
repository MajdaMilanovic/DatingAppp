using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;

namespace API.Interfaces
{
    public interface IAdminRepository
    {
        Task<List<PhotoApprovalStatisticsDto>> GetPhotoApprovalStatsAsync();
        Task<List<UserWithoutMainPhotoDto>> GetUsersWithoutMainPhotoAsync();
    }
}
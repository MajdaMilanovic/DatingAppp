using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;



    public AdminRepository(DataContext context)

    {

        _context = context;

    }
        public async Task<List<PhotoApprovalStatisticsDto>> GetPhotoApprovalStatsAsync()
        {
            var result = new List<PhotoApprovalStatisticsDto>();
           await using var command = _context.Database.GetDbConnection().CreateCommand();

            command.CommandText = "GetPhotoApprovalStats";
            command.CommandType = CommandType.StoredProcedure;

            if (command.Connection!.State != ConnectionState.Open)
            { await command.Connection.OpenAsync(); }

          await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                result.Add(new PhotoApprovalStatisticsDto
                {
                    Username = reader.GetString("Username"),

                    ApprovedPhotos = reader.GetInt32("ApprovedPhotos"),

                    UnapprovedPhotos = reader.GetInt32("UnapprovedPhotos")

                });

            }
                return result;
        }

        public async Task<List<UserWithoutMainPhotoDto>> GetUsersWithoutMainPhotoAsync()
        {
            var result = new List<UserWithoutMainPhotoDto>();
            using var connection = _context.Database.GetDbConnection();

            await connection.OpenAsync();
            using var command = connection.CreateCommand();

            command.CommandText = "CALL GetUsersWithoutMainPhoto()";

            command.CommandType = CommandType.Text;

            using var reader = await command.ExecuteReaderAsync();

              while (await reader.ReadAsync())
                {

                    result.Add(new UserWithoutMainPhotoDto
                    {
                        Username = reader.GetString("Username")
                    });
                }

                return result;
    }
   }
}
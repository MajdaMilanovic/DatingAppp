using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoApprovalStatisticsDto
    {
        public string Username { get; set; } = string.Empty;
        public int ApprovedPhotos { get; set; }
        public int UnapprovedPhotos { get; set; }
    }
}
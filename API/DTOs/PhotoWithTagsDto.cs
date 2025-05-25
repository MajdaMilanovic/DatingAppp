using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class PhotoWithTagsDto : PhotoDto
    {
        public new List<TagDto> Tags { get; set; } = [];
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class PhotoTag
    {
        public int PhotoId { get; set; }
        public Photo Photo { get; set; } = null!;

        public int TagId { get; set; }
        public Tag Tag { get; set; } = null!;
    }
}
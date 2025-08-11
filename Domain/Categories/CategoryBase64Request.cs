using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Categories
{
    public class CategoryBase64Request
    {
        public string CategoryName { get; set; }
        public long CategoryTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
        public string? Base64Image { get; set; } // Base64-encoded image string
    }
}

using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Categories
{
    public class Category : BaseModel
    {
        public long Id { get; set; }
        public string CategoryName { get; set; }
        public string? ImageUrl { get; set; }
        public long CategoryTypeId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActive { get; set; }
    }
}

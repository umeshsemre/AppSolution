using Domain.Categories;
using Persistence.Interfaces.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Categories
{
    public class CategoryRepository(DapperContext context)
         : Repository<Category>(context), ICategoryRepository
    {
    }
}

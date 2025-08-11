using Domain.Categories;
using Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Categories
{
    public interface ICategoryServices
    {
        Task<Response<Category>> GetAllCategoriesAsync();
        Task<Response<Category>> GetCategoryByIdAsync(int id);
        Task<Response<Category>> AddCategoryAsync(Category category);
        Task<Response<Category>> UpdateCategoryAsync(Category category);
        Task<Response<Category>> DeleteCategoryAsync(int id);
    }
}

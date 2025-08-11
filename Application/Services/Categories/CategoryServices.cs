using Application.Interfaces.Categories;
using Domain.Categories;
using Domain.Response;
using Persistence.Interfaces.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Categories
{
    public class CategoryServices(ICategoryRepository categoryRepository) : ICategoryServices
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<Response<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Response<Category>> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Response<Category>> AddCategoryAsync(Category category)
        {
            return await _categoryRepository.AddAsync(category);
        }

        public async Task<Response<Category>> UpdateCategoryAsync(Category category)
        {
            return await _categoryRepository.UpdateAsync(category);
        }

        public async Task<Response<Category>> DeleteCategoryAsync(int id)
        {
            return await _categoryRepository.DeleteAsync(id);
        }
    }
}

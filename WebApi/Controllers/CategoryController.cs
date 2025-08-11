using Application.Interfaces.Categories;
using Application.Interfaces.Product;
using Domain.Categories;
using Domain.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController(ICategoryServices categoryServices, IWebHostEnvironment env) : ControllerBase
    {
        private readonly ICategoryServices _categoryServices = categoryServices;
        private readonly IWebHostEnvironment _env = env;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _categoryServices.GetAllCategoriesAsync();
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _categoryServices.GetCategoryByIdAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost("add")]
        
        public async Task<IActionResult> Add([FromBody] CategoryBase64Request request)
        {
            var category = new Category
            {
                CategoryName = request.CategoryName,
                CategoryTypeId = request.CategoryTypeId,
                CreateDate= DateTime.Now,
                ModifiedDate=DateTime.Now,
                IsDeleted = request.IsDeleted,
                IsActive = request.IsActive
            };

            if (!string.IsNullOrEmpty(request.Base64Image))
            {
                try
                {
                    byte[] imageBytes = Convert.FromBase64String(request.Base64Image);

                    string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string fileName = Guid.NewGuid().ToString() + ".png"; // or detect extension
                    string filePath = Path.Combine(uploadsFolder, fileName);

                    await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                    category.ImageUrl = $"/uploads/categories/{fileName}";
                }
                catch (FormatException)
                {
                    return BadRequest(new { message = "Invalid Base64 image format" });
                }
            }

            var result = await _categoryServices.AddCategoryAsync(category);
            return StatusCode((int)result.Status, result);
        }

        //[HttpPut("update/{id}")]
        //public async Task<IActionResult> Update(int id, [FromBody] CategoryBase64Request request)
        //{
        //    // Get the existing category from the database
        //    var existingCategoryResponse = await _categoryServices.GetCategoryByIdAsync(id);
        //    if (existingCategoryResponse.Data == null)
        //    {
        //        return NotFound(new { message = "Category not found" });
        //    }

        //    var category = existingCategoryResponse.Data;
        //    category.Cate = request.CategoryName;
        //    category.CategoryTypeId = request.CategoryTypeId;
        //    category.ModifiedDate = DateTime.Now;
        //    category.IsDeleted = request.IsDeleted;
        //    category.IsActive = request.IsActive;

        //    if (!string.IsNullOrEmpty(request.Base64Image))
        //    {
        //        try
        //        {
        //            byte[] imageBytes = Convert.FromBase64String(request.Base64Image);

        //            string uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "categories");
        //            if (!Directory.Exists(uploadsFolder))
        //            {
        //                Directory.CreateDirectory(uploadsFolder);
        //            }

        //            string fileName = Guid.NewGuid().ToString() + ".png"; // or detect extension
        //            string filePath = Path.Combine(uploadsFolder, fileName);

        //            await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

        //            category.ImageUrl = $"/uploads/categories/{fileName}";
        //        }
        //        catch (FormatException)
        //        {
        //            return BadRequest(new { message = "Invalid Base64 image format" });
        //        }
        //    }

        //    var result = await _categoryServices.UpdateCategoryAsync(category);
        //    return StatusCode((int)result.Status, result);
        //}


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryServices.DeleteCategoryAsync(id);
            return StatusCode((int)result.Status, result);
        }
    }
}

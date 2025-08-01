using Application.Interfaces.Product;
using Domain.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController(IProductServices productServices) : ControllerBase
    {
        private readonly IProductServices _productServices = productServices;

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _productServices.GetAllProductsAsync();
            return StatusCode((int)result.Status, result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _productServices.GetProductByIdAsync(id);
            return StatusCode((int)result.Status, result);
        }

        [HttpPost]
        public async Task<IActionResult> Add(Product product)
        {
            var result = await _productServices.AddProductAsync(product);
            return StatusCode((int)result.Status, result);
        }

        [HttpPut]
        public async Task<IActionResult> Update(Product product)
        {
            var result = await _productServices.UpdateProductAsync(product);
            return StatusCode((int)result.Status, result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productServices.DeleteProductAsync(id);
            return StatusCode((int)result.Status, result);
        }
    }
}

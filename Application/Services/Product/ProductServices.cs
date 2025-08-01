using Application.Interfaces.Product;
using Domain.Response;
using Persistence.Interfaces.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Product
{
    public class ProductServices(IProductRepository productRepository) : IProductServices
    {
        private readonly IProductRepository _productRepository = productRepository;

        public async Task<Response<Domain.Product.Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Response<Domain.Product.Product>> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<Response<Domain.Product.Product>> AddProductAsync(Domain.Product.Product product)
        {
            return await _productRepository.AddAsync(product);
        }

        public async Task<Response<Domain.Product.Product>> UpdateProductAsync(Domain.Product.Product product)
        {
            return await _productRepository.UpdateAsync(product);
        }

        public async Task<Response<Domain.Product.Product>> DeleteProductAsync(int id)
        {
            return await _productRepository.DeleteAsync(id);
        }
    }
}

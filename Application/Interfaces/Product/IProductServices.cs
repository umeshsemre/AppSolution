using Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.Product
{
    public interface IProductServices
    {
        Task<Response<Domain.Product.Product>> GetAllProductsAsync();
        Task<Response<Domain.Product.Product>> GetProductByIdAsync(int id);
        Task<Response<Domain.Product.Product>> AddProductAsync(Domain.Product.Product product);
        Task<Response<Domain.Product.Product>> UpdateProductAsync(Domain.Product.Product product);
        Task<Response<Domain.Product.Product>> DeleteProductAsync(int id);
    }
}

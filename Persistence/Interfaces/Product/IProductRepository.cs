using Azure;
using Domain.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Persistence.Interfaces.Product
{
    public interface IProductRepository : IRepository<Domain.Product.Product>
    {

    }
}

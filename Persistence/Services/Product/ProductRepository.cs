using Azure;
using Dapper;
using Domain.Product;
using Microsoft.Data.SqlClient;
using Persistence.Interfaces.Product;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services.Product
{

    public class ProductRepository(DapperContext context) : Repository<Domain.Product.Product>(context), IProductRepository
    {

    }

}

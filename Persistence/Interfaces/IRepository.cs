using Domain.Entities;
using Domain.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Interfaces
{
    public interface IRepository<T> where T : BaseModel
    {
        Task<Response<T>> GetAllAsync();
        Task<Response<T>> GetByIdAsync(int id);
        Task<Response<T>> AddAsync(T entity);
        Task<Response<T>> UpdateAsync(T entity);
        Task<Response<T>> DeleteAsync(int id);
    }

}

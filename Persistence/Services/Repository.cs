using Dapper;
using Domain.Entities;
using Domain.Response;
using Persistence.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Services
{
    public class Repository<T>(DapperContext context) : IRepository<T> where T : BaseModel
    {
        private readonly DapperContext _context = context;
        private readonly string _tableName = typeof(T).Name;

        public async Task<Response<T>> GetAllAsync()
        {
            using var connection = _context.CreateConnection();
            var data = (await connection.QueryAsync<T>($"SELECT * FROM {_tableName}")).ToList();
            return new Response<T>
            {
                recordsTotal = data.Count,
                recordsFiltered = data.Count,
                Status = HttpStatusCode.OK,
                Message = "Data fetched successfully.",
                Data = data
            };
        }

        public async Task<Response<T>> GetByIdAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.QueryFirstOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id = @Id", new { Id = id });

            return new Response<T>
            {
                Status = result != null ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                Message = result != null ? "Record found." : "Record not found.",
                Data = result != null ? new List<T> { result } : new List<T>(),
                recordsTotal = result != null ? 1 : 0,
                recordsFiltered = result != null ? 1 : 0
            };
        }

        public async Task<Response<T>> AddAsync(T entity)
        {
            using var connection = _context.CreateConnection();
            var insertQuery = GenerateInsertQuery();
            var result = await connection.ExecuteAsync(insertQuery, entity);

            return new Response<T>
            {
                Status = result > 0 ? HttpStatusCode.Created : HttpStatusCode.BadRequest,
                Message = result > 0 ? "Inserted successfully." : "Insert failed.",
                Data = new List<T> { entity },
                recordsTotal = 1,
                recordsFiltered = 1
            };
        }

        public async Task<Response<T>> UpdateAsync(T entity)
        {
            using var connection = _context.CreateConnection();
            var updateQuery = GenerateUpdateQuery();
            var result = await connection.ExecuteAsync(updateQuery, entity);

            return new Response<T>
            {
                Status = result > 0 ? HttpStatusCode.OK : HttpStatusCode.NotModified,
                Message = result > 0 ? "Updated successfully." : "Update failed.",
                Data = new List<T> { entity },
                recordsTotal = 1,
                recordsFiltered = 1
            };
        }

        public async Task<Response<T>> DeleteAsync(int id)
        {
            using var connection = _context.CreateConnection();
            var result = await connection.ExecuteAsync($"DELETE FROM {_tableName} WHERE Id = @Id", new { Id = id });

            return new Response<T>
            {
                Status = result > 0 ? HttpStatusCode.OK : HttpStatusCode.NotFound,
                Message = result > 0 ? "Deleted successfully." : "Record not found.",
                Data = new List<T>(),
                recordsTotal = result,
                recordsFiltered = result
            };
        }

        private static string GenerateInsertQuery()
        {
            var props = typeof(T).GetProperties().Where(p => p.Name != "Id");
            var columns = string.Join(", ", props.Select(p => p.Name));
            var values = string.Join(", ", props.Select(p => "@" + p.Name));
            return $"INSERT INTO {typeof(T).Name} ({columns}) VALUES ({values})";
        }

        private static string GenerateUpdateQuery()
        {
            var props = typeof(T).GetProperties().Where(p => p.Name != "Id");
            var setClause = string.Join(", ", props.Select(p => $"{p.Name} = @{p.Name}"));
            return $"UPDATE {typeof(T).Name} SET {setClause} WHERE Id = @Id";
        }
    }

}

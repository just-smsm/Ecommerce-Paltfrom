
using Ecommerce_platforms.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_platforms.Core.IRepository
{
    public interface IGenericRepository<T> where T : ModelBase
    {
        public Task<T> AddAsync(T entity);
        public Task<T> UpdateAsync(T entity);
        public  Task<bool> DeleteAsync(int id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<T> GetByIdAsync(int id);
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using Datify.API.Models;

namespace Datify.API.Data
{
    public interface IDatingRepository
    {
        void Add<T>(T entity) where T: class;
        void Delete<T>(T entity) where T: class;
        Task<bool> SaveAll();
        Task<IEnumerable<User>> GetUsers();
        // UserParams userParams
        Task<User> GetUser(int id);
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Datify.API.Helpers;
using Datify.API.Models;
using System.Linq;

namespace Datify.API.Data {
    public class DatingRepository : IDatingRepository {
        private readonly DataContext _context;
        public DatingRepository (DataContext context) {
            _context = context;

        }
        public void Add<T> (T entity) where T : class {
            _context.Add(entity);
        }

        public void Delete<T> (T entity) where T : class {
            _context.Remove(entity);
        }

        public async Task<Photo> GetMainPhotoForUser(int userId)
        {
            return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
        }

        public async Task<Photo> GetPhoto(int id)
        {
            var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);
            return photo;
        }

        public async Task<User> GetUser (int id) {
            var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
            return user;
        }

        public async Task<PagedList<User>> GetUsers (UserParams userParams) {
            var users = _context.Users.Include(p => p.Photos).AsQueryable();
            // Filter out the current logged in user
            users = users.Where(u => u.Id != userParams.UserId);
            // Filter out same gender - return opposite gender
            users = users.Where(u => u.Gender == userParams.Gender);
            // Filter out by age
            if (userParams.MinAge != 18 || userParams.MaxAge != 99) {
                // Find the lowest year of birth to be returned i.e oldest
                var minDOB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                // Find the highest year of birth to be returned i.e youngest
                var maxDOB = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOB);
            }
            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<bool> SaveAll () {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
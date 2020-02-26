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

        public async Task<Like> GetLike(int userId, int recipientId)
        {
            return await _context.Likes.FirstOrDefaultAsync(u => u.LikerId == userId && u.LikeeId == recipientId);
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
            // Order users default by most recent users
            var users = _context.Users.Include(p => p.Photos).OrderByDescending(u => u.LastActive).AsQueryable();

            // Filter out the current logged in user
            users = users.Where(u => u.Id != userParams.UserId);

            // Filter out same gender - return opposite gender
            users = users.Where(u => u.Gender == userParams.Gender);

            // Return list of user likers - those who have liked the user
            if (userParams.Likers) {
                // Get list of users' id that like user
                var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
                // Get users with those id in db
                users = users.Where(u => userLikers.Contains(u.Id));
            }
 
            // Return list of user likees - those who have been like by the user
            if (userParams.Likees) {
                // Get list of users' id that user likes
                var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
                // Get users with those id in db
                users = users.Where(u => userLikees.Contains(u.Id));
            }

            // Filter out by age
            if (userParams.MinAge != 18 || userParams.MaxAge != 99) {
                // Find the lowest year of birth to be returned i.e oldest
                var minDOB = DateTime.Today.AddYears(-userParams.MaxAge - 1);
                // Find the highest year of birth to be returned i.e youngest
                var maxDOB = DateTime.Today.AddYears(-userParams.MinAge);

                users = users.Where(u => u.DateOfBirth >= minDOB && u.DateOfBirth <= maxDOB);
            }

            // Order by created if specified
            if (!string.IsNullOrEmpty(userParams.OrderBy)) {
                switch (userParams.OrderBy)
                {
                    case "created":
                        users = users.OrderByDescending(u => u.Created);
                        break;
                    default:
                        users = users.OrderByDescending(u => u.LastActive);
                        break;
                }
            }

            return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
        }

        public async Task<Message> GetMessage(int id) {
            return await _context.Messages.FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<PagedList<Message>> GetMessagesForUser(MessageParams messageParams) {
            var messages = _context.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos).AsQueryable();

            // Filter out messages to be returned
            switch (messageParams.MessageContainer) {
                case "Inbox":
                    // Messages received by user and not deleted by user
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false);
                    break;
                    // Messages sent by user and not deleted by user
                case "Outbox":
                    messages = messages.Where(u => u.SenderId == messageParams.UserId && u.SenderDeleted == false);
                    break;
                default:
                    messages = messages.Where(u => u.RecipientId == messageParams.UserId && u.RecipientDeleted == false && u.IsRead == false);
                    break;
            }

            // Order messages by latest message
            messages = messages.OrderByDescending(d => d.MessageSent);
            return await PagedList<Message>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<Message>> GetMessageThread(int userId, int recipientId) {
            var messages = await _context.Messages.Include(u => u.Sender).ThenInclude(p => p.Photos)
                .Include(u => u.Recipient).ThenInclude(p => p.Photos)
                .Where(m => m.RecipientId == userId && m.RecipientDeleted == false && m.SenderId == recipientId 
                    || m.RecipientId == recipientId && m.SenderDeleted == false && m.SenderId == userId)
                .OrderBy(m => m.MessageSent).ToListAsync();

            return messages;
        }

        public async Task<bool> SaveAll () {
            return await _context.SaveChangesAsync() > 0;
        }

        private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers) {
            // Get user including list of likers and likees
            var user = await _context.Users.Include(x => x.Likers).Include(x => x.Likees).FirstOrDefaultAsync(u => u.Id == id);

            if (likers) {
                // Return a list of the user's likers' id
                return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
            } else {
                // Return a list of the user's likees' id
                return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
            }
        }
    }
}
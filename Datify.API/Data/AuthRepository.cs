using System;
using System.Threading.Tasks;
using Datify.API.Models;

namespace Datify.API.Data {
    public class AuthRepository : IAuthRepository {
        private readonly DataContext _context;
        public AuthRepository (DataContext context) {
            _context = context;
        }
        public async Task<User> Register (User user, string password) {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                // Set passwordSalt to the randomly generated key
                passwordSalt = hmac.Key;
                // Compute passwordHash using the given password
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            };
        }

        public Task<User> Login (string username, string password) {
            throw new System.NotImplementedException ();
        }

        public Task<bool> UserExists (string username) {
            throw new System.NotImplementedException ();
        }
    }
}
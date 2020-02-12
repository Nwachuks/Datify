using Microsoft.EntityFrameworkCore;
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

        public async Task<User> Login (string username, string password) {
            // Find user in database using username
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            // If user doesn't exist
            if (user == null) {
                return null;
            }

            // If password is incorrect
            if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt)) {
                return null;
            }

            return user;
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            // Generate hash using user's password salt
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt)) {
                // Compute passwordHash using the given password
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                // Compare both hashes for equality
                for (int i = 0; i < computedHash.Length; i++) {
                    if (computedHash[i] != passwordHash[i]) {
                        return false;
                    }
                }
            };
            return true;
        }

        public async Task<bool> UserExists (string username) {
            if (await _context.Users.AnyAsync(x => x.Username == username)) {
                return true;
            }
            return false;
        }
    }
}
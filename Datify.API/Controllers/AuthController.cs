using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Datify.API.Data;
using Datify.API.Models;

namespace Datify.API.Controllers {
    [ApiController]
    [Route ("[controller]")]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        public AuthController (IAuthRepository repo) {
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password) {
            // Validate requests

            // Store usernames in lower case to restrict to unique usernames
            username = username.ToLower();

            // Check if username exists
            if (await _repo.UserExists(username)) {
                return BadRequest("Username already exists!");
            }

            // Create new user and register in auth repo
            var userToCreate = new User {
                Username = username
            };

            var createdUser = await _repo.Register(userToCreate, password);

            return StatusCode(201);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Datify.API.Data;
using Datify.API.Models;
using Datify.API.Dtos;

namespace Datify.API.Controllers {
    [ApiController]
    [Route ("[controller]")]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        public AuthController (IAuthRepository repo) {
            _repo = repo;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) {
            // Validate requests

            // Store usernames in lower case to restrict to unique usernames
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            // Check if username exists
            if (await _repo.UserExists(userForRegisterDto.Username)) {
                return BadRequest("Username already exists!");
            }

            // Create new user and register in auth repo
            var userToCreate = new User {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }
    }
}
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Datify.API.Data;
using Datify.API.Dtos;
using Datify.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Datify.API.Controllers {
    // receive data from body and validate requests
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper) {
            _repo = repo;
            _config = config;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(UserForRegisterDto userForRegisterDto) {
            // Store usernames in lower case to restrict to unique usernames
            userForRegisterDto.Username = userForRegisterDto.Username.ToLower();

            // Check if username exists
            if (await _repo.UserExists(userForRegisterDto.Username)) {
                return BadRequest ("Username already exists!");
            }

            // Create new user and register in auth repo
            var userToCreate = new User {
                Username = userForRegisterDto.Username
            };

            var createdUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return StatusCode(201);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserForLoginDto userForLoginDto) {
            var userFromRepo = await _repo.Login(userForLoginDto.Username.ToLower(), userForLoginDto.Password);

            if (userFromRepo == null) {
                // Ensure client is not sure if username or password is correct
                return Unauthorized();
            }

            // Info to be added to the token - id and username
            var claims = new [] {
                new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
                new Claim(ClaimTypes.Name, userFromRepo.Username)
            };

            // Create key to sign token
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes (_config.GetSection ("AppSettings:Token").Value));

            // Encrypt key
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Create token
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            // Use to create token
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // Get the currently logged in user
            var user = _mapper.Map<UserForListDto>(userFromRepo);

            // Write token to response sent back to the client and include user with photos
            return Ok(new {
                token = tokenHandler.WriteToken(token),
                user
            });
        }
    }
}
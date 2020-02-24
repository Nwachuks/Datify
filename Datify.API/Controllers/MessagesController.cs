using System;
using AutoMapper;
using Datify.API.Data;
using Datify.API.Dtos;
using Datify.API.Helpers;
using Datify.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Datify.API.Controllers {
    [ServiceFilter (typeof (LogUserActivity))]
    [Authorize]
    [ApiController]
    [Route ("users/{userId}/[controller]")]
    public class MessagesController : ControllerBase {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController (IDatingRepository repo, IMapper mapper) {
            _mapper = mapper;
            _repo = repo;

        }

        [HttpGet("{id}", Name = "GetMessage")]
        public async Task<IActionResult> GetMessage(int userId, int id) {
            // Check if user is logged in
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            // Check if message exists
            var messageFromRepo = await _repo.GetMessage(id);
            if (messageFromRepo == null) {
                return NotFound();
            }

            return Ok(messageFromRepo);
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, MessageForCreationDto messageForCreationDto) {
            // Check if user is logged in
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }

            // Set sender id for dto
            messageForCreationDto.SenderId = userId;

            // Check if recipient exists
            var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);
            if (recipient == null) {
                return BadRequest("Could not find user");
            }

            // Create message
            var message = _mapper.Map<Message>(messageForCreationDto);
            _repo.Add(message);

            var messageToReturn = _mapper.Map<MessageForCreationDto>(message);

            if (await _repo.SaveAll()) {
                return CreatedAtRoute("GetMessage", new { userId, id = message.Id }, messageToReturn);
            }

            throw new Exception("Creating the message failed on save");
        }
    }
}
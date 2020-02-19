using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Datify.API.Data;
using Datify.API.Dtos;
using Datify.API.Helpers;
using Datify.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Datify.API.Controllers {
    [Authorize]
    [Route("users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig) {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account account = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id) {
            var photoFromRepo = await _repo.GetPhoto(id);
            var photo = _mapper.Map<PhotoForReturnDto>(photoFromRepo);
            return Ok(photo);
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoForCreationDto photoForCreationDto) {
            // Check if the user is authorized (adding photo for own profile)
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }
            // Get user
            var userFromRepo = await _repo.GetUser(userId);
            // Photo to be uploaded
            var file = photoForCreationDto.File;
            // Result from Cloudinary - url and public id   
            var uploadResult = new ImageUploadResult();

            if (file.Length > 0) {
                // Read photo file into memory
                using (var stream = file.OpenReadStream()) {
                    var uploadParams = new ImageUploadParams() {
                        File = new FileDescription(file.Name, stream),
                        // Transform image to square shape with focus on the face
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            // Set parameters of photo with results from Cloudinary
            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoForCreationDto);
            // Check if first photo and set it as main
            if (!userFromRepo.Photos.Any(u => u.IsMain)) {
                photo.IsMain = true;
            }
            // Add photo to user photos
            userFromRepo.Photos.Add(photo);

            if (await _repo.SaveAll()) {
                var photoToReturn = _mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhoto", new { userId, id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id) {
            // Check if user is authorized
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }
            // Get user from repo
            var user = await _repo.GetUser(userId);
            // Check if user is updating own photos
            if (!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }
            // Get photo from repo
            var photoFromRepo = await _repo.GetPhoto(id);
            // Check if photo is main photo
            if (photoFromRepo.IsMain) {
                return BadRequest("Photo is already your main photo");
            }
            // Get the current main photo from photos and swap is main identity
            var currentMainPhoto = await _repo.GetMainPhotoForUser(userId);
            currentMainPhoto.IsMain = false;
            photoFromRepo.IsMain = true;

            if (await _repo.SaveAll()) {
                return NoContent();
            }

            return BadRequest("Could not set photo to main");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id) {
            // Check if user is authorized
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) {
                return Unauthorized();
            }
            // Get user from repo
            var user = await _repo.GetUser(userId);
            // Check if user is deleting own photos
            if (!user.Photos.Any(p => p.Id == id)) {
                return Unauthorized();
            }
            // Get photo from repo
            var photoFromRepo = await _repo.GetPhoto(id);
            // Check if photo is main photo
            if (photoFromRepo.IsMain) {
                return BadRequest("You cannot delete your main photo");
            }

            // Delete from Cloudinary
            if (photoFromRepo.PublicId != null) {
                var deleteParams = new DeletionParams(photoFromRepo.PublicId);
                var result = _cloudinary.Destroy(deleteParams);
                // Delete from db if deletion from Cloudinary successful
                if (result.Result == "ok") {
                    _repo.Delete(photoFromRepo);
                }
            } else {
                // Delete random user photos
                _repo.Delete(photoFromRepo);
            }

            if (await _repo.SaveAll()) {
                return Ok();
            }

            return BadRequest("Failed to delete photo");
        }
    }
}
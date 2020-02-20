using System.Linq;
using AutoMapper;
using Datify.API.Dtos;
using Datify.API.Models;

namespace Datify.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
            // Map the profile photo for each user
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            CreateMap<User, UserForDetailedDto>()
            // Map the profile photo for specific user
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                })
            // Convert date of birth to age, extending the datetime class
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom(d => d.DateOfBirth.CalculateAge());
                });
            // Map photo collection for each user
            CreateMap<Photo, PhotosForDetailedDto>();
            // Map the updated details to the user
            CreateMap<UserForUpdateDto, User>();
            // Map the photo to the specified details to return
            CreateMap<Photo, PhotoForReturnDto>();
            // Map the created photo properties to photo
            CreateMap<PhotoForCreationDto, Photo>();
            // Map the registered user details to user
            CreateMap<UserForRegisterDto, User>();
        }
    }
}
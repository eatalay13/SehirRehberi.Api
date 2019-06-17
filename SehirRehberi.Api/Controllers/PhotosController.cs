using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SehirRehberi.Api.Data;
using SehirRehberi.Api.Dtos;
using SehirRehberi.Api.Helpers;
using SehirRehberi.Api.Models;
using System.Linq;
using System.Security.Claims;

namespace SehirRehberi.Api.Controllers
{
    [Route("api/cities/{cityId}/[controller]")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        public readonly IAppRepository appRepository;
        public readonly IMapper mapper;
        public readonly IOptions<CloudinarySettings> cloudinaryConfig;

        private Cloudinary cloudinary;

        public PhotosController(IAppRepository appRepository, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this.appRepository = appRepository;
            this.mapper = mapper;
            this.cloudinaryConfig = cloudinaryConfig;

            Account account = new Account(
                cloudinaryConfig.Value.ApiSecret,
                cloudinaryConfig.Value.ApiKey,
                cloudinaryConfig.Value.ApiSecret);

            cloudinary = new Cloudinary(account);
        }

        [HttpPost]
        public ActionResult AddPhotoForCity(int cityId, [FromBody]PhotoForCreationDto photoForCreationDto)
        {
            var city = appRepository.GetCityById(cityId);

            if (city == null)
                return BadRequest("Could not find the city");

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (currentUserId != city.UserId)
                return Unauthorized();

            var file = photoForCreationDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.Name, stream)
                    };

                    uploadResult = cloudinary.Upload(uploadParams);
                }
            }

            photoForCreationDto.Url = uploadResult.Uri.ToString();
            photoForCreationDto.PublicId = uploadResult.PublicId;

            var photo = mapper.Map<Photo>(photoForCreationDto);

            photo.City = city;

            if (!city.Photos.Any(p => p.IsMain))
                photo.IsMain = true;

            city.Photos.Add(photo);

            if (appRepository.SaveAll())
            {
                var photoToReturn = mapper.Map<PhotoForReturnDto>(photo);
                return CreatedAtRoute("GetPhotos", new { id = photo.Id }, photoToReturn);
            }

            return BadRequest("Could not add the photo");

        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public ActionResult GetPhoto(int id)
        {
            var photoFromDb = appRepository.GetPhoto(id);
            var photo = mapper.Map<PhotoForReturnDto>(photoFromDb);

            return Ok(photo);
        }
    }
}
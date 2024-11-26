using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;

namespace imageupload.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadImageController : ControllerBase
    {
        private readonly Cloudinary _cloudinary;
        private readonly IConfiguration _configuration;
        public UploadImageController(IConfiguration configuration)
        {
            var cloudinarySettings = configuration.GetSection("Cloudinary");

            var account = new Account(
                cloudinarySettings["CloudName"],
                cloudinarySettings["ApiKey"],
                cloudinarySettings["ApiSecret"]
            );

            _cloudinary = new Cloudinary(account);
        }
        [HttpPost("UploadImage")] 
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            try
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "FoodDeliveryApp" 
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return Ok(new { imageUrl = uploadResult.SecureUrl.ToString() });
                }

                return BadRequest("error while uploading the image");
            }
            catch (Exception)
            {
                return BadRequest("Internal server error");
            }
        }

    }
}

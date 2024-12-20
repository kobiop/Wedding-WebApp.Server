using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using WeddingUI.Server.Models;
using WeddingUI.Server.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WeddingUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PhotosController: ControllerBase
    {
        private readonly PhotoService _photoService;
        private readonly CounterService _counterService;

        public PhotosController(PhotoService photoService , CounterService counterService)
        {
            _photoService = photoService;
            _counterService = counterService;
        }
    
        [HttpGet("test-connection")]
        public async Task<IActionResult> TestConnection()
        {
            bool isConnected = await _photoService.TestConnectionAsync();
            if (isConnected)
            {
                return Ok("Database connection successful.");
            }
            return StatusCode(500, "Failed to connect to the database.");
        }

        // GET: photos
        [HttpGet]
        public async Task<ActionResult<List<Photo>>> GetPhotos()
        {
            var photos = await _photoService.GetPhotosAsync();
            return Ok(photos);
        }

        // POST: photos/upload
        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            // Validate file type - check if it is an image
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only image files are allowed.");
            }

            // Validate MIME type - check if the file is an image
            var validMimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };
            var mimeType = file.ContentType;

            if (!validMimeTypes.Contains(mimeType))
            {
                return BadRequest("Invalid file MIME type. Only image files are allowed.");
            }

            // Limit file size (example: 10MB)
            if (file.Length > 10 * 1024 * 1024)
            {
                return BadRequest("File size exceeds the 10MB limit.");
            }

            try
            {
                // Generate a new auto-incremented ID for the photo
                int newPhotoId = await _counterService.GetNextPhotoId();

                // Read the file into a byte array
                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);

                var photo = new Photo
                {
                    AutoIncrementId = newPhotoId,
                    UserId = userId,
                    Filename = file.FileName,
                    PhotoData = memoryStream.ToArray()
                };

                // Insert the photo into the database
                await _photoService.InsertPhotoAsync(photo);

                return Ok(new { message = "File uploaded successfully!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("/Photos/{id}")]
        public async Task<IActionResult> DeletePhoto(int id)
        {
            await _photoService.DeletePhotoAsync(id);
            return Ok(new { message = "Photo deleted successfully" });
        }




    }
}

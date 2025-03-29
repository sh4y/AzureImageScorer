using Microsoft.AspNetCore.Mvc;
using ImageService.ImageAnalysis.Helpers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System;

namespace ImageService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageAnalysisController : ControllerBase
    {
        private readonly IImageAnalysisService _imageService;
        private readonly TemporaryImageUploader _imageUploader;

        public ImageAnalysisController(
            IImageAnalysisService imageService,
            TemporaryImageUploader imageUploader)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _imageUploader = imageUploader ?? throw new ArgumentNullException(nameof(imageUploader));
        }

        [HttpGet("analyze")]
        public IActionResult AnalyzeImage([FromQuery] string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("Image URL is required");
            }

            try
            {
                var result = _imageService.AnalyzeImage(new Uri(imageUrl));
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error analyzing image: {ex.Message}");
            }
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadAndAnalyze(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            try
            {
                // Upload the file to temporary storage
                string imageUrl = await _imageUploader.UploadImageAsync(file);

                // Analyze the image
                var result = _imageService.AnalyzeImage(new Uri(imageUrl));

                return Ok(new
                {
                    ImageUrl = imageUrl,
                    Analysis = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error processing image: {ex.Message}");
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Services.ImageBlurrer; // Ensure this matches the actual namespace of ImageBlurrer
using System.Threading.Tasks;
using SixLabors.ImageSharp; // Add ImageSharp namespace
using SixLabors.ImageSharp.PixelFormats; // Add ImageSharp PixelFormats namespace

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        try
        {
            ImageBlurrer blurrer = new ImageBlurrer();
            Image<Rgba32> originalImage = await blurrer.ConvertToImageAsync(file); // Use Image<Rgba32>

            Console.WriteLine("UploadImage: Image conversion successful.");

            Image<Rgba32> blurredImage = blurrer.BlurImage(originalImage, 20); // Adjust the radius as needed

            Console.WriteLine("UploadImage: Image blurring successful.");

            using (var imageStream = blurrer.SaveImageToStream(blurredImage, "png")) // Use "png" as string
            {
                return File(imageStream.ToArray(), "image/png");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UploadImage: Error - {ex.Message}");
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}


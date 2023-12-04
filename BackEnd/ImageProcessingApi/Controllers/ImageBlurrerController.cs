using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Services.ImageBlurrer; // Ensure this matches the actual namespace of ImageBlurrer
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

[ApiController]
[Route("[controller]")]
public class ImageController : ControllerBase
{
    [HttpPost("upload")]
    public async Task<IActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            Console.WriteLine("UploadImage: No file uploaded.");
            return BadRequest("No file uploaded.");
        }

        try
        {
            ImageBlurrer blurrer = new ImageBlurrer();
            Bitmap originalImage = await blurrer.ConvertToBitmapAsync(file);

            Console.WriteLine("UploadImage: Image conversion successful.");

            Bitmap blurredImage = blurrer.BlurImage(originalImage, 5); // Adjust the radius as needed
            Console.WriteLine("UploadImage: Image blurring successful.");

            // Logic to send back the images (original and blurred)
            using (var stream = new MemoryStream())
            {
                blurredImage.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                stream.Position = 0; // Reset the stream position to the beginning
                return File(stream, "image/png");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"UploadImage: Error - {ex.Message}");
            return StatusCode(500, "Internal server error: " + ex.Message);
        }
    }
}

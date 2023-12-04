using System;
using System.Drawing;
using Microsoft.AspNetCore.Mvc;
using Services.ImageBlurrer; // Ensure this matches the actual namespace of ImageBlurrer
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using System.Drawing.Imaging;

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
        Bitmap originalImage = await blurrer.ConvertToBitmapAsync(file);

        Console.WriteLine("UploadImage: Image conversion successful.");

        Bitmap blurredImage = blurrer.BlurImage(originalImage, 70); // Adjust the radius as needed

        Console.WriteLine("UploadImage: Image blurring successful.");

        var imageBytes = blurrer.SaveImageToStream(blurredImage, ImageFormat.Png);
      
        return File(imageBytes, "image/png");

    }
    catch (Exception ex)
    {
        Console.WriteLine($"UploadImage: Error - {ex.Message}");
        return StatusCode(500, "Internal server error: " + ex.Message);
    }
}

}

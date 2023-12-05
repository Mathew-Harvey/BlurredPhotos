using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Formats;

namespace Services.ImageBlurrer
{
    public class ImageBlurrer
    {
        public async Task<Image<Rgba32>> ConvertToImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty.", nameof(file));
            }

            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0; // Reset the stream position to the beginning
                return Image.Load<Rgba32>(stream);
            }
        }

        public Image<Rgba32> BlurImage(Image<Rgba32> image, int radius)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Input image cannot be null.");
            }

            if (radius <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), "Radius must be greater than zero.");
            }

            image.Mutate(x => x.GaussianBlur(radius));
            return image;
        }

        public MemoryStream SaveImageToStream(Image<Rgba32> image, string format)
        {
            if (image == null)
            {
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");
            }

            var stream = new MemoryStream();
            var encoder = GetEncoder(format);
            image.Save(stream, encoder);
            stream.Position = 0; // Reset the stream position to the beginning
            return stream;
        }

        private IImageEncoder GetEncoder(string format)
        {
            return format.ToLower() switch
            {
                "png" => new SixLabors.ImageSharp.Formats.Png.PngEncoder(),
                "jpeg" => new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder(),
                "bmp" => new SixLabors.ImageSharp.Formats.Bmp.BmpEncoder(),
                "gif" => new SixLabors.ImageSharp.Formats.Gif.GifEncoder(),
                _ => throw new ArgumentException($"Unsupported format: {format}")
            };
        }
    }
}


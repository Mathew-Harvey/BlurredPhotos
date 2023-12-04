using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Services.ImageBlurrer;

public class ImageBlurrer
{

    public async Task<Bitmap> ConvertToBitmapAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is null or empty.", nameof(file));
        }

        using (var stream = new MemoryStream())
        {
            await file.CopyToAsync(stream);
            stream.Position = 0; // Reset the stream position to the beginning
            return new Bitmap(stream);
        }
    }

    public Bitmap BlurImage(Bitmap image, int radius)
    {
        if (image == null)
        {
            throw new ArgumentNullException("image", "Input image cannot be null.");
        }

        if (radius <= 0)
        {
            throw new ArgumentOutOfRangeException("radius", "Radius must be greater than zero.");
        }

        Bitmap blurredImage = new Bitmap(image.Width, image.Height);

        // Lock the bitmap's bits
        Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
        BitmapData srcData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        BitmapData dstData = blurredImage.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);

        int bytes = srcData.Stride * srcData.Height;
        byte[] buffer = new byte[bytes];
        byte[] result = new byte[bytes];

        // Copy the source image bytes to the buffer
        System.Runtime.InteropServices.Marshal.Copy(srcData.Scan0, buffer, 0, bytes);

        // Parallel processing
        Parallel.For(0, srcData.Height, y =>
        {
            for (int x = 0; x < srcData.Width; x++)
            {
                int avgR = 0, avgG = 0, avgB = 0;
                int blurPixelCount = 0;

                // Average the color of the neighboring pixels
                for (int yy = y - radius; yy < y + radius + 1; yy++)
                {
                    for (int xx = x - radius; xx < x + radius + 1; xx++)
                    {
                        int nx = Math.Min(srcData.Width - 1, Math.Max(0, xx));
                        int ny = Math.Min(srcData.Height - 1, Math.Max(0, yy));

                        if (nx >= 0 && ny >= 0 && nx < srcData.Width && ny < srcData.Height)
                        {
                            int offset = (ny * srcData.Stride) + (nx * 4);
                            avgB += buffer[offset + 0];
                            avgG += buffer[offset + 1];
                            avgR += buffer[offset + 2];
                            blurPixelCount++;
                        }
                    }
                }

                avgR /= blurPixelCount;
                avgG /= blurPixelCount;
                avgB /= blurPixelCount;

                int resultOffset = (y * dstData.Stride) + (x * 4);
                result[resultOffset + 0] = (byte)avgB;
                result[resultOffset + 1] = (byte)avgG;
                result[resultOffset + 2] = (byte)avgR;
                result[resultOffset + 3] = 255; // Alpha channel
            }
        });

        // Copy modified bytes back to the image
        System.Runtime.InteropServices.Marshal.Copy(result, 0, dstData.Scan0, bytes);

        image.UnlockBits(srcData);
        blurredImage.UnlockBits(dstData);

        return blurredImage;
    }
}

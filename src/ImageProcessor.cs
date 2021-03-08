using Converter.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;

namespace Converter
{
    /// <summary>
    /// Image processor
    /// </summary>
    public static class ImageProcessor
    {
        /// <summary>
        /// Draws a red rectangle on the specified image at the specified position
        /// </summary>
        /// <param name="imagePath">The path to the image</param>
        /// <param name="position">The position at which to draw the rectangle</param>
        public static void DrawRect(string imagePath, Bounds position)
        {
            using (Image image = Image.Load(imagePath))
            {
                var pen = Pens.Solid(Color.Red, 5);
                var rect = new Rectangle((int)position.X, (int)position.Y, (int)position.Width, (int)position.Height);
                image.Mutate(x => x.Draw(pen, rect));

                image.Save(imagePath);
            }
        }

        /// <summary>
        /// Resizes an image to the specified dimension, cropping it if necessary
        /// </summary>
        /// <param name="imagePath">The path to the image</param>
        /// <param name="width">The target width</param>
        /// <param name="height">The target height</param>
        public static void Resize(string imagePath, int width, int height)
        {
            using (Image image = Image.Load(imagePath))
            {
                if (image.Width > width || image.Height > height)
                {
                    image.Mutate(x => x.Resize(new ResizeOptions
                    {
                        Size = new Size { Width = width, Height = height },
                        Position = AnchorPositionMode.Center,
                        Mode = ResizeMode.Crop
                    }));
                }

                image.Save(imagePath);
            }
        }
    }
}

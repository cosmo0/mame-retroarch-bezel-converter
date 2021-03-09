using Converter.Model;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;

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
        /// Finds the screen position in the specified bezel, based on transparency.
        /// </summary>
        /// <param name="bezel">The bezel file.</param>
        /// <param name="options">The options.</param>
        /// <returns>The screen position</returns>
        public static Bounds FindScreen(byte[] bezel, Options options)
        {
            using (Image<Rgba32> image = Image.Load(bezel))
            {
                // scan around vertical center to find the screen (it's probably more or less centered horizontally)
                var centerX = image.Width / 2;
                var (topCenter, bottomCenter) = GetTransparentPixelsInColumn(image, centerX);
                var (topCenterPlus, bottomCenterPlus) = GetTransparentPixelsInColumn(image, centerX + 50);
                var (topCenterMinus, bottomCenterMinus) = GetTransparentPixelsInColumn(image, centerX - 50);

                var top = Math.Min(topCenter, Math.Min(topCenterPlus, topCenterMinus));
                var bottom = Math.Max(bottomCenter, Math.Max(bottomCenterMinus, bottomCenterPlus));
                var height = bottom - top;

                // screen is found, now scan horizontally based on these positions (it's often not centered vertically)
                var centerY = top + (height / 2);
                var (leftCenter, rightCenter) = GetTransparentPixelsInRow(image, centerY);
                var (leftCenterPlus, rightCenterPlus) = GetTransparentPixelsInRow(image, centerY + 50);
                var (leftCenterMinus, rightCenterMinus) = GetTransparentPixelsInRow(image, centerY - 50);

                var left = Math.Min(leftCenter, Math.Min(leftCenterMinus, leftCenterPlus));
                var right = Math.Max(rightCenter, Math.Max(rightCenterMinus, rightCenterPlus));
                var width = right - left;

                // apply margins
                if (options.Margin > 0)
                {
                    top -= options.Margin;
                    left -= options.Margin;
                    width += options.Margin * 2;
                    height += options.Margin * 2;
                }

                return new Bounds
                {
                    X = left,
                    Y = top,
                    Width = width,
                    Height = height
                };
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

        /// <summary>
        /// Gets the first and last transparent pixels in the specified column.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="column">The column to scan.</param>
        /// <returns>The first and last transparent pixels</returns>
        private static (int first, int last) GetTransparentPixelsInColumn(Image<Rgba32> image, int column)
        {
            int first = 0;
            int last = 0;

            for (int y = 0; y < image.Height; y++)
            {
                // first transparent pixel
                if (first == 0 && image[column, y].A < 255)
                {
                    first = y;
                }

                // last transparent pixel
                if (image[column, y].A < 255)
                {
                    last = y;
                }

                // last transparent pixel has been found
                if (image[column, y].A == 255 && last > 0)
                {
                    // arbitrary margin of error in case a transparent pixel (or column) exists somewhere in the bezel
                    if (last - first < 100)
                    {
                        // it's an error = reset values
                        first = 0;
                        last = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return (first, last);
        }

        /// <summary>
        /// Gets the first and last transparent pixels in the specified row.
        /// </summary>
        /// <param name="image">The image.</param>
        /// <param name="row">The row to scan.</param>
        /// <returns>The first and last transparent pixels</returns>
        private static (int first, int last) GetTransparentPixelsInRow(Image<Rgba32> image, int row)
        {
            int first = 0;
            int last = 0;

            Span<Rgba32> pixelRowSpan = image.GetPixelRowSpan(row);
            for (int x = 0; x < image.Width; x++)
            {
                // first transparent pixel
                if (first == 0 && pixelRowSpan[x].A < 255)
                {
                    first = x;
                }

                // last transparent pixel
                if (pixelRowSpan[x].A < 255)
                {
                    last = x;
                }

                // last transparent pixel has been found
                if (pixelRowSpan[x].A == 255 && last > 0)
                {
                    // arbitrary margin of error in case a transparent pixel (or column) exists somewhere in the bezel
                    if (last - first < 100)
                    {
                        // it's an error = reset values
                        first = 0;
                        last = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            return (first, last);
        }
    }
}

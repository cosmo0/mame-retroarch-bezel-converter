using Converter.Model;
using System;
using System.IO;

namespace Converter
{
    /// <summary>
    /// Conversion from MAME bezels to RetroArch bezels
    /// </summary>
    public static class Converter
    {
        /// <summary>
        /// Applies the specified offset to the specified bounds
        /// </summary>
        /// <param name="sourcePosition">The source screen position</param>
        /// <param name="offset">The offset to apply</param>
        /// <param name="sourceResolution">The source resolution</param>
        /// <param name="targetResolution">The target resolution</param>
        /// <returns>The new bounds</returns>
        public static Bounds ApplyOffset(Bounds sourcePosition, Offset offset, Bounds sourceResolution, Bounds targetResolution)
        {
            if (offset == null)
            {
                return sourcePosition;
            }

            var newPos = sourcePosition.Clone();

            // multiply w/h by stretch = get target screen size, centered => NEW DIMENSIONS AT SOURCE RESOLUTION
            newPos.Width *= offset.HStretch;
            newPos.Height *= offset.VStretch;

            // compute new base x/y (top/left): x = cx - (w / 2)
            newPos.X = sourcePosition.Center.X - (newPos.Width / 2);
            newPos.Y = sourcePosition.Center.Y - (newPos.Height / 2);

            // apply offset: x = x + ((hres / w * h) * hoffset) ; y = y + (vres * voffset) => NEW POSITION at source resolution
            if (offset.HOffset != 0)
            {
                if (newPos.Orientation == Orientation.Horizontal)
                {
                    newPos.X += (sourcePosition.Width / newPos.Width * newPos.Height) * offset.HOffset;
                }
                else
                {
                    newPos.X += sourcePosition.Width * offset.HOffset;
                }
            }

            if (offset.VOffset != 0)
            {
                if (newPos.Orientation == Orientation.Horizontal)
                {
                    newPos.Y += sourcePosition.Height * offset.VOffset;
                }
                else
                {
                    newPos.Y += (sourcePosition.Height / newPos.Height * newPos.Width) * offset.VOffset;
                }
            }

            // apply target resolution => NEW COORDINATES AT TARGET RESOLUTION
            newPos.X *= targetResolution.Width / sourceResolution.Width;
            newPos.Y *= targetResolution.Height / sourceResolution.Height;
            newPos.Width *= targetResolution.Width / sourceResolution.Width;
            newPos.Height *= targetResolution.Height / sourceResolution.Height;

            return newPos;
        }

        /// <summary>
        /// Fill a template config with the specified values
        /// </summary>
        /// <param name="configPath">The path to the config file to fill</param>
        /// <param name="game">The game name</param>
        /// <param name="position">The position of the image</param>
        public static void FillTemplate(string configPath, string game, Bounds position)
        {
            var content = File.ReadAllText(configPath);
            content = content
                .Replace("{{game}}", game)
                .Replace("{{width}}", Math.Round(position.Width, 0).ToString())
                .Replace("{{height}}", Math.Round(position.Height, 0).ToString())
                .Replace("{{x}}", Math.Round(position.X, 0).ToString())
                .Replace("{{y}}", Math.Round(position.Y, 0).ToString())
                .Replace("{{orientation}}", position.Orientation.ToString().ToLower());

            File.WriteAllText(configPath, content);
        }
    }
}

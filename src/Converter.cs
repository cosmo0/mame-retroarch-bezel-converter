using System;
using System.IO;
using Converter.Model;

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
        /// <returns>The new bounds</returns>
        public static Bounds ApplyOffset(Bounds sourcePosition, Offset offset, Bounds sourceResolution)
        {
            if (offset == null)
            {
                return sourcePosition;
            }

            // TODO
            return sourcePosition;
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

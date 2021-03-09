using Converter.Model;
using Converter.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Converter
{
    /// <summary>
    /// A processor for RetroArch files
    /// </summary>
    public class RetroArchProcessor
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="RetroArchProcessor"/> class from being created.
        /// </summary>
        private RetroArchProcessor()
        {
        }

        /// <summary>
        /// Gets the name of the overlay image file.
        /// </summary>
        public string OverlayImageFileName { get; private set; }

        /// <summary>
        /// Gets the overlay image full path.
        /// </summary>
        public string OverlayImagePath { get; private set; }

        /// <summary>
        /// Gets the source resolution.
        /// </summary>
        public Bounds SourceResolution { get; internal set; }

        /// <summary>
        /// Gets the source screen position.
        /// </summary>
        public Bounds SourceScreenPosition { get; private set; }

        /// <summary>
        /// Gets the RetroArch processor.
        /// </summary>
        /// <returns>The RetroArch processor</returns>
        public static RetroArchProcessor GetProcessor(string romFile, IEnumerable<string> overlayConfigFiles, RaToMameOptions options)
        {
            // get rom file content
            var romFileContent = File.ReadAllText(romFile);

            // get overlay content
            var overlayCfgFileSourcePath = GetCfgData(romFileContent, "input_overlay");
            var overlayCfgFileName = (new FileInfo(overlayCfgFileSourcePath)).Name;
            var overlayCfgFilePath = Path.Join(options.SourceConfigs, overlayCfgFileName);
            var overlayCfgFileContent = File.ReadAllText(overlayCfgFilePath);

            // extract data from configs
            var overlayImageFileName = GetCfgData(overlayCfgFileContent, "overlay0_overlay");
            var width = GetCfgData(romFileContent, "custom_viewport_width");
            var height = GetCfgData(romFileContent, "custom_viewport_height");
            var x = GetCfgData(romFileContent, "custom_viewport_x");
            var y = GetCfgData(romFileContent, "custom_viewport_y");

            var screenBounds = new Bounds
            {
                X = int.Parse(x),
                Y = int.Parse(y),
                Width = int.Parse(width),
                Height = int.Parse(height)
            };

            var xres = GetCfgData(romFileContent, "video_fullscreen_x");
            var yres = GetCfgData(romFileContent, "video_fullscreen_y");

            var resolution = new Bounds
            {
                X = 0,
                Y = 0,
                Width = int.Parse(xres),
                Height = int.Parse(yres)
            };

            return new RetroArchProcessor
            {
                OverlayImageFileName = overlayImageFileName,
                OverlayImagePath = Path.Join(options.SourceConfigs, overlayImageFileName),
                SourceScreenPosition = screenBounds,
                SourceResolution = resolution
            };
        }

        /// <summary>
        /// Gets data from the specified config file.
        /// </summary>
        /// <param name="fileContent">The content of the file.</param>
        /// <param name="key">The key to look for.</param>
        /// <returns>The config value</returns>
        private static string GetCfgData(string fileContent, string key)
        {
            /// searched value looks like:
            /// key = "value"
            /// with or without spaces, with or without quotes, with or without trailing spaces
            var match = Regex.Match(fileContent, "^" + key + "\\s*=\\s\"?([^\"\\n\\s]*)\"?\\s*$", RegexOptions.Multiline);
            if (match.Success && match.Captures.Any())
            {
                return match.Groups[1].Value;
            }

            return null;
        }
    }
}

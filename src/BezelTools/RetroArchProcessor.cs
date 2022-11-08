using BezelTools.Model;
using BezelTools.Options;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BezelTools
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
        /// Gets the bounds written in a config file
        /// </summary>
        /// <param name="fileContent">The content of the file</param>
        /// <returns>The rom file content</returns>
        public static Bounds GetBoundsFromConfig(string fileContent)
        {
            if (int.TryParse(GetCfgData(fileContent, "custom_viewport_width"), out int width)
                && int.TryParse(GetCfgData(fileContent, "custom_viewport_height"), out int height)
                && int.TryParse(GetCfgData(fileContent, "custom_viewport_x"), out int x)
                && int.TryParse(GetCfgData(fileContent, "custom_viewport_y"), out int y))
            {
                return new Bounds
                {
                    X = x,
                    Y = y,
                    Width = width,
                    Height = height
                };
            }

            return new Bounds { X = 0, Y = 0, Width = 0, Height = 0 };
        }

        /// <summary>
        /// Gets data from the specified config file.
        /// </summary>
        /// <param name="fileContent">The content of the file.</param>
        /// <param name="key">The key to look for.</param>
        /// <returns>The config value</returns>
        public static string GetCfgData(string fileContent, string key)
        {
            var match = Regex.Match(fileContent, BuildCfgRegex(key), RegexOptions.Multiline);
            if (match.Success && match.Captures.Any())
            {
                return match.Groups[1].Value.Trim();
            }

            return null;
        }

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
            var screenBounds = GetBoundsFromConfig(romFileContent);

            var xres = GetCfgData(romFileContent, "video_fullscreen_x");
            var yres = GetCfgData(romFileContent, "video_fullscreen_y");

            var resolution = new Bounds
            {
                X = 0,
                Y = 0,
                Width = int.Parse(xres ?? options.TargetResolutionBounds.Width.ToString()),
                Height = int.Parse(yres ?? options.TargetResolutionBounds.Height.ToString())
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
        /// Sets the bounds in the specified config
        /// </summary>
        /// <param name="filePath">The path to the file to set</param>
        /// <param name="bounds">The bounds to set</param>
        /// <returns>The modified config</returns>
        public static void SetBounds(string filePath, string game, Bounds bounds, Bounds resolution)
        {
            var fileContent = File.ReadAllText(filePath);

            fileContent = SetCfgData(fileContent, "custom_viewport_width", bounds.Width.ToString());
            fileContent = SetCfgData(fileContent, "custom_viewport_height", bounds.Height.ToString());
            fileContent = SetCfgData(fileContent, "custom_viewport_x", bounds.X.ToString());
            fileContent = SetCfgData(fileContent, "custom_viewport_y", bounds.Y.ToString());

            // fill placeholders
            fileContent = FileUtils.FillTemplateContent(fileContent, game, bounds, resolution);

            File.WriteAllText(filePath, fileContent);
        }

        /// <summary>
        /// Sets a value in the specified config file
        /// </summary>
        /// <param name="fileContent">The contents of the file</param>
        /// <param name="key">The key to set</param>
        /// <returns>The modified content</returns>
        public static string SetCfgData(string fileContent, string key, string value)
        {
            var r = BuildCfgRegex(key);
            var v = $"{key} = {value}";

            // if it exists: replace value
            if (Regex.Match(fileContent, r, RegexOptions.Multiline).Success)
            {
                return Regex.Replace(fileContent, r, v, RegexOptions.Multiline);
            }

            // if it doesn't exist: add value
            return $"{fileContent}\n{v}";
        }

        /// <summary>
        /// Creates a config file
        /// </summary>
        /// <param name="templatePath">The path to the template</param>
        /// <param name="game">The game name</param>
        /// <param name="dest">The destination path</param>
        /// <param name="bounds">The screen bounds</param>
        /// <param name="resolution">The target resolution</param>
        public static void CreateConfig(string templatePath, string game, string dest, Bounds bounds, Bounds resolution)
        {
            File.Copy(templatePath, dest);
            FileUtils.FillTemplate(dest, game);

            if (bounds != null)
            {
                SetBounds(dest, game, bounds, resolution);
            }
        }

        private static string BuildCfgRegex(string key)
        {
            /// searched value looks like:
            /// key = "value"
            /// with or without spaces, with or without quotes, with or without trailing spaces
            return $"^{key}\\s*=\\s?\"?([^\"\\n]*)\"?\\s*$";
        }
    }
}

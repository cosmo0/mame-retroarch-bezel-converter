using BezelTools.Options;
using System.IO;

namespace BezelTools
{
    /// <summary>
    /// Generator for overlays
    /// </summary>
    public static class Generator
    {
        private static readonly object errorFileLock = new();
        private static int createdNb = 0;
        private static int errorsNb = 0;

        /// <summary>
        /// Generates
        /// </summary>
        /// <param name="options">The options</param>
        public static void Generate(GenerateOptions options)
        {
            Interaction.Log("########## GENERATING ROM CONFIGS ##########");

            var images = Directory.GetFiles(options.ImagesFolder, "*.png", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            ThreadUtils.RunAsync(options.Threads, images, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".png", "");

                Interaction.Log($"{game} generating config");

                // resize
                ImageProcessor.Resize(f, (int)options.TargetResolutionBounds.Width, (int)options.TargetResolutionBounds.Height);

                // get data from image
                var bounds = ImageProcessor.FindScreen(File.ReadAllBytes(f), options.Margin);

                // generate config
                var config = Path.Combine(options.ImagesFolder, $"{game}.cfg");
                if (!options.Overwrite && File.Exists(config))
                {
                    Error(options.ErrorFile, game, $"config file already exists: {config}");
                }
                else
                {
                    File.Delete(config);
                    RetroArchProcessor.CreateConfig(options.TemplateOverlay, game, config, bounds, options.TargetResolutionBounds);
                    Create(options.ErrorFile, game, $"created config: {config}");
                }

                // generate rom
                var rom = Path.Combine(options.RomsFolder, $"{game}.zip.cfg");
                if (!options.Overwrite && File.Exists(rom))
                {
                    Error(options.ErrorFile, game, $"rom config file already exists: {rom}");
                }
                else
                {
                    File.Delete(rom);
                    RetroArchProcessor.CreateConfig(options.TemplateRom, game, rom, bounds, options.TargetResolutionBounds);
                    Create(options.ErrorFile, game, $"created rom config file: {rom}");
                }

                // debug
                if (!string.IsNullOrWhiteSpace(options.OutputDebug))
                {
                    ImageProcessor.DebugDraw($"{game}_image", options.OutputDebug, f, bounds, options.TargetResolutionBounds);
                }

                Interaction.Log($"{game} done");
            });

            Interaction.Log("########## DONE ##########");

            if (createdNb > 0 || errorsNb > 0)
            {
                Interaction.Log("");

                Interaction.Log($"- {errorsNb} errors");
                Interaction.Log($"- {createdNb} game files created");

                Interaction.Log($"Check {options.ErrorFile} to see the details");
            }
        }

        private static void Create(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "CREATE");

            createdNb++;
        }

        private static void Error(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "ERROR");

            errorsNb++;
        }

        private static void Write(string file, string game, string msg, string level)
        {
            Interaction.Log($"{game} {level}: {msg}");

            if (!string.IsNullOrWhiteSpace(file))
            {
                lock (errorFileLock)
                {
                    File.AppendAllText(file, $"{level};{game};{msg}\n");
                }
            }
        }
    }
}
using BezelTools.Options;
using System;
using System.Collections.Generic;
using System.IO;

namespace BezelTools
{
    /// <summary>
    /// Generator for overlays
    /// </summary>
    public static class Generator
    {
        private readonly static object errorFileLock = new();
        private static int createdNb = 0;
        private static int errorsNb = 0;

        /// <summary>
        /// Generates 
        /// </summary>
        /// <param name="options">The options</param>
        public static void Generate(GenerateOptions options)
        {
            Console.WriteLine("########## GENERATING ROM CONFIGS ##########");

            var images = Directory.GetFiles(options.ImagesFolder, "*.png", new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
            ThreadUtils.RunThreadsOnFiles(options.Threads, images, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".png", "");

                Console.WriteLine($"{game} generating config");

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
                }

                // generate rom
                var rom = Path.Combine(options.RomsFolder, $"{game}.cfg");
                if (!options.Overwrite && File.Exists(rom))
                {
                    Error(options.ErrorFile, game, $"rom file already exists: {rom}");
                }
                else
                {
                    File.Delete(rom);
                    RetroArchProcessor.CreateConfig(options.TemplateRom, game, rom, bounds, options.TargetResolutionBounds);
                }

                // debug
                if (!string.IsNullOrWhiteSpace(options.OutputDebug))
                {
                    ImageProcessor.DebugDraw($"{game}_image", options.OutputDebug, f, bounds);
                }

                Console.WriteLine($"{game} done");
            });

            Console.WriteLine("########## DONE ##########");

            if (createdNb > 0 || errorsNb > 0)
            {
                Console.WriteLine("");

                Console.WriteLine($"- {errorsNb} errors");
                Console.WriteLine($"- {createdNb} game files created");

                Console.WriteLine($"Check {options.ErrorFile} to see the details");
            }
        }

        private static void Error(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "ERROR");

            errorsNb++;
        }

        private static void Create(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "CREATE");

            createdNb++;
        }

        private static void Write(string file, string game, string msg, string level)
        {
            Console.WriteLine($"{game} {level}: {msg}");

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

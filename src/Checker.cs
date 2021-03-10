using BezelTools.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BezelTools
{
    /// <summary>
    /// Retroarch config files checker
    /// </summary>
    public static class Checker
    {
        private readonly static object errorFileLock = new object();

        private static int errorsNb = 0;

        /// <summary>
        /// Checks the Retroarch configuration files.
        /// </summary>
        /// <param name="options">The options.</param>
        public static void Check(CheckOptions options)
        {
            var searchOption = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };

            Console.WriteLine("########## CHECKING ROMS CONFIGS ##########");

            // check roms configs
            var romConfigs = Directory.GetFiles(options.RomsConfigFolder, "*.cfg", searchOption);
            var overlaysUsedByRoms = new List<string>();
            ThreadUtils.RunThreadsOnFiles(options.Threads, romConfigs, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".zip.cfg", "").Replace(".cfg", "");

                Console.WriteLine($"rom {game} start processing");

                var cfgContent = File.ReadAllText(f);

                var overlayPath = RetroArchProcessor.GetCfgData(cfgContent, "input_overlay");
                var ofi = new FileInfo(overlayPath);

                // check that there is an matching overlay file at the expected localtion
                if (!File.Exists(Path.Join(options.OverlaysConfigFolder, ofi.Name)))
                {
                    Error($"ERROR: rom {fi.Name} points to a non-existing overlay: {overlayPath}", options.ErrorFile);
                }
                else
                {
                    Console.WriteLine($"rom {game} uses an existing overlay config");
                    overlaysUsedByRoms.Add(ofi.Name);
                }
            });

            Console.WriteLine("########## CHECKING OVERLAY CONFIGS ##########");

            // check overlay config files
            var configs = Directory.GetFiles(options.OverlaysConfigFolder, "*.cfg", searchOption);
            var imagesUsedByOverlays = new List<string>();
            ThreadUtils.RunThreadsOnFiles(options.Threads, configs, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".cfg", "");
                Console.WriteLine($"overlay {game} start processing");

                var cfgContent = File.ReadAllText(f);
                var overlayFileName = RetroArchProcessor.GetCfgData(cfgContent, "overlay0_overlay");

                // check that the image exists
                if (!File.Exists(Path.Join(options.OverlaysConfigFolder, overlayFileName)))
                {
                    Error($"ERROR: overlay {fi.Name} points to a non-existing image: {overlayFileName}", options.ErrorFile);
                }
                else
                {
                    Console.WriteLine($"overlay {game} uses an existing image");
                    imagesUsedByOverlays.Add(overlayFileName);
                }

                // check that the overlay is used
                if (!overlaysUsedByRoms.Contains(fi.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (options.AutoFix)
                    {
                        Console.WriteLine($"overlay {game} is not used by any rom, creating a rom config file");
                        var dest = Path.Join(options.RomsConfigFolder, fi.Name);
                        File.Copy(options.TemplateRom, dest);
                        FileUtils.FillTemplate(dest, game);
                        overlaysUsedByRoms.Add(fi.Name);
                    }
                    else
                    {
                        Error($"ERROR: overlay {fi.Name} is not used by any game", options.ErrorFile);
                    }
                }
            });

            Console.WriteLine("########## CHECKING OVERLAY IMAGES ##########");

            // check that all images have an associated overlay config
            var images = Directory.GetFiles(options.OverlaysConfigFolder, "*.png", searchOption);
            ThreadUtils.RunThreadsOnFiles(options.Threads, images, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".png", "");

                Console.WriteLine($"image {game} start processing");

                // check that the image is used by an overlay
                if (!imagesUsedByOverlays.Contains(fi.Name, StringComparer.InvariantCultureIgnoreCase))
                {
                    if (options.AutoFix)
                    {
                        Console.WriteLine($"image {game} missing overlay, creating it");
                        var dest = Path.Join(options.OverlaysConfigFolder, $"{game}.cfg");
                        File.Copy(options.TemplateOverlay, dest);
                        FileUtils.FillTemplate(dest, game);
                        imagesUsedByOverlays.Add(fi.Name);
                    }
                    else
                    {
                        Error($"ERROR: image {fi.Name} is not used by any overlay", options.ErrorFile);
                    }
                }
                else
                {
                    Console.WriteLine($"image {game} is used");
                }
            });

            Console.WriteLine("########## DONE ##########");

            if (errorsNb > 0)
            {
                Console.WriteLine($"{errorsNb} error(s)! Check {options.ErrorFile}");
            }
        }

        private static void Error(string msg, string errorFile)
        {
            Console.WriteLine(msg);

            lock (errorFileLock)
            {
                File.AppendAllText(errorFile, msg + "\n");
            }

            errorsNb++;
        }
    }
}

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
        private readonly static object errorFileLock = new();

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

                if (string.IsNullOrWhiteSpace(overlayPath))
                {
                    Error(options.ErrorFile, game, $"rom has no input_overlay parameter");
                }
                else
                {
                    // make sure a Windows path is converted to Unix under *nix, and vice versa
                    var overlayFileName = Path.GetFileName(FileUtils.NormalizePath(overlayPath));

                    // check that there is an matching overlay file at the expected localtion
                    if (!File.Exists(Path.Join(options.OverlaysConfigFolder, overlayFileName)))
                    {
                        Error(options.ErrorFile, game, $"rom points to a non-existing overlay: {overlayFileName}");
                    }
                    else
                    {
                        Console.WriteLine($"rom {game} uses an existing overlay config");
                        overlaysUsedByRoms.Add(overlayFileName);
                    }

                    // check that the path in the rom config is valid
                    if (!string.IsNullOrEmpty(options.InputOverlayConfigPathInRomConfig))
                    {
                        var overlayShouldBe = $"{options.InputOverlayConfigPathInRomConfig}/{overlayFileName}";
                        if (!overlayPath.Equals(overlayShouldBe, StringComparison.InvariantCultureIgnoreCase))
                        {
                            if (options.AutoFix)
                            {
                                Info(options.ErrorFile, game, "fixing overlay path in rom config");
                                cfgContent = RetroArchProcessor.SetCfgData(cfgContent, "input_overlay", overlayShouldBe);
                                File.WriteAllText(f, cfgContent);
                            }
                            else
                            {
                                Error(options.ErrorFile, game, $"rom has a wrong overlay path: {overlayPath}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"rom {game} has correct overlay path");
                        }
                    }
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
                    Error(options.ErrorFile, game, $"overlay points to a non-existing image: {overlayFileName}");
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
                        var dest = Path.Join(options.RomsConfigFolder, fi.Name);
                        Info(options.ErrorFile, game, $"creating rom config file for unused overlay at {dest}");
                        CreateConfig(options.TemplateRom, game, dest);
                        overlaysUsedByRoms.Add(fi.Name);
                    }
                    else
                    {
                        Error(options.ErrorFile, game, $"overlay is not used by any game");
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
                        if (File.Exists(dest))
                        {
                            Error(options.ErrorFile, game, $"trying to create overlay {dest} but file already exists");
                        }
                        else
                        {
                            Info(options.ErrorFile, game, $"Creating overlay config for orphan image at {dest}");
                            CreateConfig(options.TemplateOverlay, game, dest);
                            imagesUsedByOverlays.Add(fi.Name);

                            var romDest = Path.Join(options.RomsConfigFolder, fi.Name);
                            Info(options.ErrorFile, game, $"Creating rom config for orphan image at {romDest}");
                            CreateConfig(options.TemplateRom, game, romDest);
                            overlaysUsedByRoms.Add(fi.Name.Replace(".png", ".cfg"));
                        }
                    }
                    else
                    {
                        Error(options.ErrorFile, game, $"image {fi.Name} is not used by any overlay");
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

        private static void CreateConfig(string templatePath, string game, string dest)
        {
            File.Copy(templatePath, dest);
            FileUtils.FillTemplate(dest, game);
        }

        private static void Error(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "ERROR");
        }

        private static void Info(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "INFO");
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

            errorsNb++;
        }
    }
}

using BezelTools.Model;
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
        private readonly static object configsLock = new();
        private readonly static EnumerationOptions searchOption = new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive };

        private static readonly List<Config> configs = new();
        private static int errorsNb = 0;

        private static Config AddConfigEntry(string rom, string overlay, string image)
        {
            var entry = new Config { Rom = rom, Overlay = overlay, Image = image };
            lock (configsLock)
            {
                Checker.configs.Add(entry);
            }

            return entry;
        }

        private static Config GetConfigEntry(Func<Config, bool> predicate)
        {
            lock (configsLock)
            {
                return Checker.configs.Where(predicate).FirstOrDefault();
            }
        }

        /// <summary>
        /// Checks the Retroarch configuration files.
        /// </summary>
        /// <param name="options">The options.</param>
        public static void Check(CheckOptions options)
        {
            Console.WriteLine("########## CHECKING ROMS CONFIGS ##########");

            var romTemplateContent = File.ReadAllText(options.TemplateRom);

            // check roms configs
            var romConfigs = Directory.GetFiles(options.RomsConfigFolder, "*.cfg", searchOption);
            ThreadUtils.RunThreadsOnFiles(options.Threads, romConfigs, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".zip.cfg", "").Replace(".cfg", "");
                var romConfEntry = AddConfigEntry(fi.Name, null, null);

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
                        romConfEntry.Overlay = overlayFileName;
                    }

                    // check that the path in the rom config is valid
                    if (!string.IsNullOrEmpty(options.InputOverlayConfigPathInRomConfig))
                    {
                        var separator = options.InputOverlayConfigPathInRomConfig.EndsWith("/") ? "" : "/";
                        var overlayShouldBe = $"{options.InputOverlayConfigPathInRomConfig}{separator}{overlayFileName}";
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
            var configFiles = Directory.GetFiles(options.OverlaysConfigFolder, "*.cfg", searchOption);
            ThreadUtils.RunThreadsOnFiles(options.Threads, configFiles, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".cfg", "");
                Console.WriteLine($"overlay {game} start processing");

                var cfgContent = File.ReadAllText(f);
                var overlayFileName = RetroArchProcessor.GetCfgData(cfgContent, "overlay0_overlay");

                // check that the overlay is used
                var cfgEntry = GetConfigEntry(c => c.Overlay != null && c.Overlay.Equals(fi.Name, StringComparison.InvariantCultureIgnoreCase));
                if (cfgEntry == null)
                {
                    if (options.AutoFix)
                    {
                        var dest = Path.Join(options.RomsConfigFolder, fi.Name);
                        Info(options.ErrorFile, game, $"creating rom config file for unused overlay at {dest}");

                        // get bounds
                        var img = File.ReadAllBytes(Path.Join(options.OverlaysConfigFolder, overlayFileName));
                        var bounds = ImageProcessor.FindScreen(img, options);

                        CreateConfig(options.TemplateRom, game, dest, bounds, options.TargetResolutionBounds);

                        cfgEntry = AddConfigEntry(fi.Name, fi.Name, overlayFileName);
                    }
                    else
                    {
                        Error(options.ErrorFile, game, $"overlay is not used by any game");
                    }
                }

                // check that the image exists
                if (!File.Exists(Path.Join(options.OverlaysConfigFolder, overlayFileName)))
                {
                    Error(options.ErrorFile, game, $"overlay points to a non-existing image: {overlayFileName}");
                }
                else
                {
                    Console.WriteLine($"overlay {game} uses an existing image");
                    if (cfgEntry == null)
                    {
                        cfgEntry = AddConfigEntry(null, fi.Name, overlayFileName);
                    }
                    else
                    {
                        cfgEntry.Image = overlayFileName;
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
                var cfgEntry = GetConfigEntry(c => c.Image != null && c.Image.Equals(fi.Name, StringComparison.InvariantCultureIgnoreCase));
                if (cfgEntry == null)
                {
                    if (options.AutoFix)
                    {
                        Console.WriteLine($"image {game} missing overlay, creating it");
                        var cfgFilesName = $"{game}.cfg";
                        var dest = Path.Join(options.OverlaysConfigFolder, cfgFilesName);
                        if (File.Exists(dest))
                        {
                            Error(options.ErrorFile, game, $"trying to create overlay {dest} but file already exists");
                        }
                        else
                        {
                            Info(options.ErrorFile, game, $"Creating overlay config for orphan image at {dest}");
                            CreateConfig(options.TemplateOverlay, game, dest, null, options.TargetResolutionBounds);

                            var romDest = Path.Join(options.RomsConfigFolder, cfgFilesName);
                            Info(options.ErrorFile, game, $"Creating rom config for orphan image at {romDest}");

                            // create the config
                            var bounds = ImageProcessor.FindScreen(File.ReadAllBytes(f), options);
                            CreateConfig(options.TemplateRom, game, romDest, bounds, options.TargetResolutionBounds);

                            AddConfigEntry(cfgFilesName, cfgFilesName, fi.Name);
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

                // check that image is not too large
                var imgSize = ImageProcessor.GetSize(f);
                if (imgSize.Width > options.TargetResolutionBounds.Width || imgSize.Height > options.TargetResolutionBounds.Height)
                {
                    if (options.AutoFix)
                    {
                        Info(options.ErrorFile, game, $"resizing image (previous size: {imgSize.Width}x{imgSize.Height})");
                        ImageProcessor.Resize(f, (int)options.TargetResolutionBounds.Width, (int)options.TargetResolutionBounds.Height);
                    }
                    else
                    {
                        Error(options.ErrorFile, game, $"image has wrong size: {imgSize.Width}x{imgSize.Height}");
                    }
                }
                else
                {
                    Console.WriteLine($"image {game} has the right size");
                }

                // check screen bounds
                if (cfgEntry != null) // can't write to rom config if we don't know where the image is used
                {
                    var img = File.ReadAllBytes(f);
                    var romCfgPath = Path.Join(options.RomsConfigFolder, cfgEntry.Rom);
                    var rom = File.ReadAllText(romCfgPath);
                    var boundsInImage = ImageProcessor.FindScreen(img, options);
                    var boundsInConf = RetroArchProcessor.GetBoundsFromConfig(rom);

                    if (CheckCoordinate(boundsInImage.X, boundsInConf.X, options.Margin)
                        && CheckCoordinate(boundsInImage.Y, boundsInConf.Y, options.Margin)
                        && CheckCoordinate(boundsInImage.Width, boundsInConf.Width, options.Margin)
                        && CheckCoordinate(boundsInImage.Height, boundsInConf.Height, options.Margin))
                    {
                        // bounds are similar
                        Console.WriteLine($"image {game} has proper bounds in config");
                    }
                    else
                    {
                        // bounds are too different
                        if (options.AutoFix)
                        {
                            Info(options.ErrorFile, game, "Fixing screen position in config");

                            // rewrite conf
                            RetroArchProcessor.SetBounds(romCfgPath, game, boundsInImage, options.TargetResolutionBounds);

                            // output debug
                            ImageProcessor.DebugDraw($"{game}_conf", options.OutputDebug, f, boundsInConf);
                            ImageProcessor.DebugDraw($"{game}_image", options.OutputDebug, f, boundsInImage);
                        }
                        else
                        {
                            Error(options.ErrorFile, game, $"image has wrong coordinates in config");
                        }
                    }
                }
                else
                {
                    throw new NotImplementedException("wait what");
                }
            });

            Console.WriteLine("########## DONE ##########");

            if (errorsNb > 0)
            {
                Console.WriteLine($"{errorsNb} error(s)! Check {options.ErrorFile}");
            }
        }

        private static bool CheckCoordinate(double a, double b, int margin)
        {
            return Math.Abs(a - b) <= margin;
        }

        private static void CreateConfig(string templatePath, string game, string dest, Bounds bounds, Bounds resolution)
        {
            File.Copy(templatePath, dest);
            FileUtils.FillTemplate(dest, game);

            if (bounds != null)
            {
                RetroArchProcessor.SetBounds(dest, game, bounds, resolution);
            }
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

        /// <summary>
        /// A rom/overlay/image configuration
        /// </summary>
        private class Config
        {
            public string Image;
            public string Overlay;
            public string Rom;
        }
    }
}

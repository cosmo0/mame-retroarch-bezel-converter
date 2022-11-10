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
        private static readonly List<Config> configs = new();
        private static readonly object configsLock = new();
        private static readonly object errorFileLock = new();
        private static readonly EnumerationOptions searchOption = new() { MatchCasing = MatchCasing.CaseInsensitive };
        private static int errorsNb = 0;
        private static int fixesNb = 0;

        /// <summary>
        /// Checks the Retroarch configuration files.
        /// </summary>
        /// <param name="options">The options.</param>
        public static void Check(CheckOptions options)
        {
            Interaction.Log("########## CHECKING ROMS CONFIGS ##########");

            // check roms configs
            var romConfigs = Directory.GetFiles(options.RomsConfigFolder, "*.cfg", searchOption);
            ThreadUtils.RunThreadsOnFiles(options.Threads, romConfigs, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".zip.cfg", "").Replace(".cfg", "");
                var romConfEntry = AddConfigEntry(fi.Name, null, null);

                Interaction.Log($"rom {game} start processing");

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
                        Interaction.Log($"rom {game} uses an existing overlay config");
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
                                Fix(options.ErrorFile, game, "fixing overlay path in rom config");
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
                            Interaction.Log($"rom {game} has correct overlay path");
                        }
                    }
                }
            });

            Interaction.Log("########## CHECKING OVERLAY CONFIGS ##########");

            // check overlay config files
            var configFiles = Directory.GetFiles(options.OverlaysConfigFolder, "*.cfg", searchOption);
            ThreadUtils.RunThreadsOnFiles(options.Threads, configFiles, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".cfg", "");
                Interaction.Log($"overlay {game} start processing");

                var cfgContent = File.ReadAllText(f);
                var overlayFileName = RetroArchProcessor.GetCfgData(cfgContent, "overlay0_overlay");

                // check that the overlay is used
                var cfgEntry = GetConfigEntry(c => c.Overlay != null && c.Overlay.Equals(fi.Name, StringComparison.InvariantCultureIgnoreCase));
                if (cfgEntry == null)
                {
                    if (options.AutoFix)
                    {
                        // create a rom
                        var romFile = fi.Name.Replace(".cfg", ".zip.cfg");
                        var dest = Path.Join(options.RomsConfigFolder, romFile);
                        if (File.Exists(dest))
                        {
                            Error(options.ErrorFile, game, $"overlay matches rom file but is not used by it: {romFile}");
                        }
                        else
                        {
                            Fix(options.ErrorFile, game, $"creating rom config file for unused overlay at {dest}");

                            var imgFileName = Path.Join(options.OverlaysConfigFolder, overlayFileName);
                            if (File.Exists(imgFileName))
                            {
                                // get bounds
                                var img = File.ReadAllBytes(imgFileName);
                                var bounds = ImageProcessor.FindScreen(img, options.Margin);

                                RetroArchProcessor.CreateConfig(options.TemplateRom, game, dest, bounds, options.TargetResolutionBounds);

                                cfgEntry = AddConfigEntry(romFile, fi.Name, overlayFileName);
                            }
                            else
                            {
                                Error(options.ErrorFile, game, $"overlay points to a non-existing image: {imgFileName}");
                            }
                        }
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
                    Interaction.Log($"overlay {game} uses an existing image");
                    if (cfgEntry == null)
                    {
                        AddConfigEntry(null, fi.Name, overlayFileName);
                    }
                }
            });

            Interaction.Log("########## CHECKING OVERLAY IMAGES ##########");

            // check that all images have an associated overlay config
            var images = Directory.GetFiles(options.OverlaysConfigFolder, "*.png", searchOption);
            ThreadUtils.RunThreadsOnFiles(options.Threads, images, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".png", "");

                Interaction.Log($"image {game} start processing");

                // check that the image is used by an overlay
                var cfgEntry = GetConfigEntry(c => c.Image != null && c.Image.Equals(fi.Name, StringComparison.InvariantCultureIgnoreCase));
                if (cfgEntry == null)
                {
                    if (options.AutoFix)
                    {
                        Interaction.Log($"image {game} missing overlay, creating it");
                        var cfgFilesName = $"{game}.cfg";
                        var dest = Path.Join(options.OverlaysConfigFolder, cfgFilesName);
                        if (File.Exists(dest))
                        {
                            Error(options.ErrorFile, game, $"trying to create overlay {dest} but file already exists");
                        }
                        else
                        {
                            Fix(options.ErrorFile, game, $"Creating overlay config for orphan image at {dest}");
                            RetroArchProcessor.CreateConfig(options.TemplateOverlay, game, dest, null, options.TargetResolutionBounds);

                            var romDest = Path.Join(options.RomsConfigFolder, cfgFilesName);
                            Fix(options.ErrorFile, game, $"Creating rom config for orphan image at {romDest}");

                            // create the config
                            var bounds = ImageProcessor.FindScreen(File.ReadAllBytes(f), options.Margin);
                            RetroArchProcessor.CreateConfig(options.TemplateRom, game, romDest, bounds, options.TargetResolutionBounds);

                            AddConfigEntry(cfgFilesName, cfgFilesName, fi.Name);
                        }
                    }
                    else
                    {
                        Error(options.ErrorFile, game, $"image is not used by any overlay: {fi.Name}");
                    }
                }
                else
                {
                    Interaction.Log($"image {game} is used");
                }

                // check that image is not too large
                var imgSize = ImageProcessor.GetSize(f);
                if (imgSize.Width > options.TargetResolutionBounds.Width || imgSize.Height > options.TargetResolutionBounds.Height)
                {
                    if (options.AutoFix)
                    {
                        Fix(options.ErrorFile, game, $"resizing image (previous size: {imgSize.Width}x{imgSize.Height})");
                        ImageProcessor.Resize(f, (int)options.TargetResolutionBounds.Width, (int)options.TargetResolutionBounds.Height);
                    }
                    else
                    {
                        Error(options.ErrorFile, game, $"image has wrong size: {imgSize.Width}x{imgSize.Height}");
                    }
                }
                else
                {
                    Interaction.Log($"image {game} has the right size");
                }
            });

            Interaction.Log("########## CHECKING SCREEN POSIITIONS ##########");

            // get list of roms configs, again (in case some have been created)
            romConfigs = Directory.GetFiles(options.RomsConfigFolder, "*.cfg", searchOption);
            ThreadUtils.RunThreadsOnFiles(options.Threads, romConfigs, (f) =>
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".cfg", "").Replace(".zip", "");
                Interaction.Log($"{game} checking screen position");

                // get overlay file name
                var romContent = File.ReadAllText(f);
                var overlayFileName = RetroArchProcessor.GetCfgData(romContent, "input_overlay");
                if (string.IsNullOrWhiteSpace(overlayFileName))
                {
                    Error(options.ErrorFile, game, $"fixing screen: rom config doesn't have an input_overlay");
                    return;
                }

                var overlayFile = Path.GetFileName(FileUtils.NormalizePath(overlayFileName));
                var overlayPath = Path.Join(options.OverlaysConfigFolder, overlayFile);

                if (!File.Exists(overlayPath))
                {
                    Error(options.ErrorFile, game, $"fixing screen: overlay file does not exist: {overlayPath}");
                    return;
                }

                var overlayContent = File.ReadAllText(overlayPath);
                var imageFile = RetroArchProcessor.GetCfgData(overlayContent, "overlay0_overlay");
                var imagePath = Path.Join(options.OverlaysConfigFolder, imageFile);

                if (!File.Exists(imagePath))
                {
                    Error(options.ErrorFile, game, $"fixing screen: image file does not exist: {imagePath}");
                    return;
                }

                var imageContent = File.ReadAllBytes(imagePath);

                // get bounds
                var boundsInImage = ImageProcessor.FindScreen(imageContent, 0);
                var boundsInConf = RetroArchProcessor.GetBoundsFromConfig(romContent);

                // make sure the bounds match
                if (CheckCoordinate(boundsInImage.X, boundsInConf.X, options.ErrorMargin)
                    && CheckCoordinate(boundsInImage.Y, boundsInConf.Y, options.ErrorMargin)
                    && CheckCoordinate(boundsInImage.Width, boundsInConf.Width, options.ErrorMargin * 2)
                    && CheckCoordinate(boundsInImage.Height, boundsInConf.Height, options.ErrorMargin * 2))
                {
                    // bounds are similar
                    Interaction.Log($"image {game} has proper bounds in config");
                }
                else
                {
                    Interaction.Log($"image {game} has wrong bounds in config: {boundsInConf.ToShortString()} instead of {boundsInImage.ToShortString()}");

                    boundsInImage = boundsInImage.ApplyMargin(options.Margin);

                    if (!string.IsNullOrWhiteSpace(options.OutputDebug))
                    {
                        // output debug whether fixing or not
                        if (boundsInConf.Width > 0 && boundsInConf.Height > 0)
                        {
                            ImageProcessor.DebugDraw($"{game}_conf", options.OutputDebug, imagePath, boundsInConf);
                        }
                        else
                        {
                            Interaction.Log($"image {game} has width/height equal to zero in config");
                        }

                        ImageProcessor.DebugDraw($"{game}_image", options.OutputDebug, imagePath, boundsInImage);
                    }

                    // fix the image
                    if (options.AutoFix)
                    {
                        Fix(options.ErrorFile, game, "Fixing screen position in config");
                        RetroArchProcessor.SetBounds(f, game, boundsInImage, options.TargetResolutionBounds);
                    }
                    else
                    {
                        Error(options.ErrorFile, game, $"image has wrong coordinates in config");
                    }
                }
            });

            Interaction.Log("########## DONE ##########");

            if (errorsNb > 0 || fixesNb > 0)
            {
                Interaction.Log("");

                Interaction.Log("Check result:");
                Interaction.Log($"- {errorsNb} error(s)");
                Interaction.Log($"- {fixesNb} fixes(s)");

                Interaction.Log($"Check {options.ErrorFile} to see the details");
            }
        }

        private static Config AddConfigEntry(string rom, string overlay, string image)
        {
            var entry = new Config { Rom = rom, Overlay = overlay, Image = image };
            lock (configsLock)
            {
                Checker.configs.Add(entry);
            }

            return entry;
        }

        private static bool CheckCoordinate(double a, double b, int margin)
        {
            return Math.Abs(a - b) <= margin;
        }

        private static void Error(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "ERROR");

            errorsNb++;
        }

        private static void Fix(string errorFile, string game, string msg)
        {
            Write(errorFile, game, msg, "FIX");

            fixesNb++;
        }

        private static Config GetConfigEntry(Func<Config, bool> predicate)
        {
            lock (configsLock)
            {
                return Checker.configs.FirstOrDefault(predicate);
            }
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

        /// <summary>
        /// A rom/overlay/image configuration
        /// </summary>
        private sealed class Config
        {
            public string Image;
            public string Overlay;
            public string Rom;
        }
    }
}
using BezelTools.Model;
using BezelTools.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BezelTools
{
    public static class Converter
    {
        private static readonly object errorFileLock = new();
        private static int errorsNb = 0;

        /// <summary>
        /// Starts the import from MAME to Retroarch
        /// </summary>
        public static void ConvertMameToRetroarch(MameToRaOptions options)
        {
            var fsEntries = Directory.EnumerateFileSystemEntries(options.Source).OrderBy(f => f);

            ThreadUtils.RunAsync(options.Threads, fsEntries, (f) =>
            {
                ProcessMameFile(f, options);
            });

            WriteEnd(options.ErrorFile);
        }

        /// <summary>
        /// Starts the import from Retroarch to MAME
        /// </summary>
        /// <param name="options"></param>
        public static void ConvertRetroarchToMame(RaToMameOptions options)
        {
            // get files to process
            var romFiles = Directory.EnumerateFiles(options.SourceRoms, "*.zip.cfg").OrderBy(f => f);
            var configFiles = Directory.EnumerateFiles(options.SourceConfigs, "*.cfg"); // read-only collection, no need for it to be concurrent

            ThreadUtils.RunAsync(options.Threads, romFiles, (f) =>
            {
                ProcessRetroarchFile(f, configFiles, options);
            });

            WriteEnd(options.ErrorFile);
        }

        /// <summary>
        /// Processes a file
        /// </summary>
        /// <param name="zipFile">The file to process</param>
        /// <param name="options">The options</param>
        public static void ProcessMameFile(string fsEntry, MameToRaOptions options)
        {
            var isFolder = File.GetAttributes(fsEntry).HasFlag(FileAttributes.Directory);
            FileSystemInfo fsi = isFolder ? new DirectoryInfo(fsEntry) : new FileInfo(fsEntry);

            // don't process files that are not zip
            if (!isFolder && !fsi.Name.EndsWith(".zip", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            var game = fsi.Name.Replace(".zip", "", StringComparison.InvariantCultureIgnoreCase);

            try
            {
                var cfgFile = string.IsNullOrEmpty(options.SourceConfigs) ? string.Empty : Path.Join(options.SourceConfigs, $"{game}.cfg");

                Interaction.Log($"{game} processing start");

                var (lay, cfg, bezel) = isFolder
                    ? FileUtils.ReadFiles(game, fsEntry, cfgFile, options)
                    : FileUtils.ExtractFiles(game, fsEntry, cfgFile, options);

                // extracts the data from the MAME files
                var mameProcessor = MameProcessor.BuildProcessor(options, lay, cfg);

                Interaction.Log($"{game} image: {mameProcessor.BezelFileName}");
                Interaction.Log($"{game} source screen: {mameProcessor.SourceScreenPosition}");
                Interaction.Log($"{game} screen offset: {mameProcessor.Offset}");

                // resize the bezel image
                Interaction.Log($"{game} resizing image");
                bezel = ImageProcessor.Resize(
                    bezel,
                    (int)options.TargetResolutionBounds.Width,
                    (int)options.TargetResolutionBounds.Height);

                Interaction.Log($"{game} getting target screen position");
                Bounds newPosition;
                if (options.ScanBezelForScreenCoordinates)
                {
                    // scan image for transparent pixels
                    newPosition = ImageProcessor.FindScreen(bezel, options.Margin);
                }
                else
                {
                    // convert from LAY and CFG
                    newPosition = ApplyOffset(
                                        mameProcessor.SourceScreenPosition,
                                        mameProcessor.Offset,
                                        mameProcessor.SourceResolution,
                                        options.TargetResolutionBounds);
                }

                Interaction.Log($"{game} target screen: {newPosition}");

                if (newPosition.Width <= 0 || newPosition.Height <= 0)
                {
                    Error(options.ErrorFile, game, $"Width/height of screen are invalid: {newPosition}");
                    return;
                }

                // get bezel image
                var outputImage = Path.Join(options.OutputOverlays, $"{game}.png");
                if (options.Overwrite && File.Exists(outputImage)) { File.Delete(outputImage); }
                if (options.Overwrite || !File.Exists(outputImage))
                {
                    File.WriteAllBytes(outputImage, bezel);
                }

                // debug: draw target position
                ImageProcessor.DebugDraw(game, options.OutputDebug, outputImage, newPosition, options.TargetResolutionBounds);

                Interaction.Log($"{game} creating configs");

                // create game config files
                var outputGameCfg = Path.Join(options.OutputRoms, $"{game}.zip.cfg");
                File.Copy(options.TemplateGameCfg, outputGameCfg, options.Overwrite);
                FileUtils.FillTemplate(outputGameCfg, game, newPosition, options.TargetResolutionBounds);

                // create overlay config files
                var outputOverlayCfg = Path.Join(options.OutputOverlays, $"{game}.cfg");
                File.Copy(options.TemplateOverlayCfg, outputOverlayCfg, options.Overwrite);
                FileUtils.FillTemplate(outputOverlayCfg, game, newPosition, options.TargetResolutionBounds);

                Interaction.Log($"{game} processing done");
            }
            catch (Exception ex)
            {
                Error(options.ErrorFile, game, ex.Message);
            }
        }

        /// <summary>
        /// Processes the Retroarch file.
        /// </summary>
        /// <param name="romFile">The rom config file.</param>
        /// <param name="overlayConfigFiles">All the overlay configuration files.</param>
        /// <param name="options">The options.</param>
        public static void ProcessRetroarchFile(string romFile, IEnumerable<string> overlayConfigFiles, RaToMameOptions options)
        {
            var romFi = new FileInfo(romFile);
            var game = romFi.Name.Replace(".zip.cfg", "");

            Interaction.Log($"{game} processing start");

            try
            {
                var target = Path.Join(options.Output, game);

                // get RA processor
                var processor = RetroArchProcessor.GetProcessor(romFile, overlayConfigFiles, options);

                Interaction.Log($"{game} image: {processor.OverlayImageFileName}");
                Interaction.Log($"{game} source screen: {processor.SourceScreenPosition}");

                Bounds newPosition;
                if (options.ScanBezelForScreenCoordinates)
                {
                    newPosition = ImageProcessor.FindScreen(File.ReadAllBytes(processor.OverlayImagePath), options.Margin);
                }
                else
                {
                    // convert from LAY and CFG
                    newPosition = processor.SourceScreenPosition;
                }

                Interaction.Log($"{game} target screen: {newPosition}");

                // create destination folder
                if (options.Overwrite && Directory.Exists(target)) { Directory.Delete(target, true); }
                if (!Directory.Exists(target)) { Directory.CreateDirectory(target); }

                // copy overlay image
                File.Copy(processor.OverlayImagePath, Path.Join(target, processor.OverlayImageFileName), options.Overwrite);

                // resize the bezel image
                Interaction.Log($"{game} processing image");
                ImageProcessor.Resize(
                    processor.OverlayImagePath,
                    (int)processor.SourceResolution.Width,
                    (int)processor.SourceResolution.Height);

                // debug: draw target position
                ImageProcessor.DebugDraw(game, options.OutputDebug, processor.OverlayImagePath, newPosition, options.TargetResolutionBounds);

                Interaction.Log($"{game} creating configs");

                // create lay file
                var outputLay = Path.Join(target, "default.lay");
                File.Copy(options.Template, outputLay, options.Overwrite);
                FileUtils.FillTemplate(outputLay, game, newPosition, processor.SourceResolution);

                // zip overlay
                if (options.Zip)
                {
                    Interaction.Log($"{game} zipping file");
                    var targetZip = Path.Join(options.Output, $"{game}.zip");
                    if (File.Exists(targetZip)) { File.Delete(targetZip); }
                    FileUtils.CompressFolderContent(target, targetZip);
                    Directory.Delete(target, true);
                }

                Interaction.Log($"{game} processing done");
            }
            catch (Exception ex)
            {
                Error(options.ErrorFile, game, ex.Message);
            }
        }

        /// <summary>
        /// Applies the specified offset to the specified bounds
        /// </summary>
        /// <param name="sourcePosition">The source screen position</param>
        /// <param name="offset">The offset to apply</param>
        /// <param name="sourceResolution">The source resolution</param>
        /// <param name="targetResolution">The target resolution</param>
        /// <returns>The new bounds</returns>
        private static Bounds ApplyOffset(Bounds sourcePosition, Offset offset, Bounds sourceResolution, Bounds targetResolution)
        {
            var newPos = sourcePosition.Clone();

            if (offset != null)
            {
                // multiply w/h by stretch = get target screen size, centered => NEW DIMENSIONS AT
                // SOURCE RESOLUTION
                newPos.Width *= offset.HStretch;
                newPos.Height *= offset.VStretch;

                // compute new base x/y (top/left): x = cx - (w / 2)
                newPos.X = sourcePosition.Center.X - (newPos.Width / 2);
                newPos.Y = sourcePosition.Center.Y - (newPos.Height / 2);

                // apply offset: x = x + ((hres / w * h) * hoffset) ; y = y + (vres * voffset) =>
                // NEW POSITION at source resolution
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
            }

            // apply target resolution => NEW COORDINATES AT TARGET RESOLUTION
            newPos.X *= targetResolution.Width / sourceResolution.Width;
            newPos.Y *= targetResolution.Height / sourceResolution.Height;
            newPos.Width *= targetResolution.Width / sourceResolution.Width;
            newPos.Height *= targetResolution.Height / sourceResolution.Height;

            return newPos;
        }

        private static void Error(string errorFile, string game, string msg)
        {
            Interaction.Log($"{game} PROCESSING ERROR: {msg}");

            if (!string.IsNullOrWhiteSpace(errorFile))
            {
                lock (errorFileLock)
                {
                    File.AppendAllText(errorFile, $"{game};{msg}\n");
                }
            }

            errorsNb++;
        }

        private static void WriteEnd(string errorFile)
        {
            Interaction.Log("########## DONE ##########");

            if (errorsNb > 0)
            {
                Interaction.Log($"{errorsNb} error(s)! Check {errorFile} to see the details");
            }
        }
    }
}
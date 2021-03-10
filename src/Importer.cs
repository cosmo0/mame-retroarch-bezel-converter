using BezelTools.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace BezelTools
{
    public static class Importer
    {
        /// <summary>
        /// Starts the import from MAME to Retroarch
        /// </summary>
        public static void ConvertMameToRetroarch(MameToRaOptions options)
        {
            var fsEntries = Directory.EnumerateFileSystemEntries(options.Source).OrderBy(f => f);

            RunThreads(options.Threads, fsEntries, (f) =>
            {
                ProcessMameFile(f, options);
            });

            Console.WriteLine($"########## DONE");
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

            RunThreads(options.Threads, romFiles, (f) =>
            {
                ProcessRetroarchFile(f, configFiles, options);
            });

            Console.WriteLine($"########## DONE");
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

                Console.WriteLine($"{game} processing start");

                var (lay, cfg, bezel) = isFolder
                    ? FileUtils.ReadFiles(game, fsEntry, cfgFile, options)
                    : FileUtils.ExtractFiles(game, fsEntry, cfgFile, options);

                // extracts the data from the MAME files
                var mameProcessor = MameProcessor.BuildProcessor(options, lay, cfg);

                Console.WriteLine($"{game} image: {mameProcessor.BezelFileName}");
                Console.WriteLine($"{game} source screen: {mameProcessor.SourceScreenPosition}");
                Console.WriteLine($"{game} screen offset: {mameProcessor.Offset}");

                var newPosition = new Model.Bounds();
                if (options.ScanBezelForScreenCoordinates)
                {
                    newPosition = ImageProcessor.FindScreen(bezel, options);
                }
                else
                {
                    // convert from LAY and CFG
                    newPosition = Converter.ApplyOffset(
                                        mameProcessor.SourceScreenPosition,
                                        mameProcessor.Offset,
                                        mameProcessor.SourceResolution,
                                        options.TargetResolutionBounds);
                }

                Console.WriteLine($"{game} target screen: {newPosition}");

                // get bezel image
                var outputImage = Path.Join(options.OutputOverlays, $"{game}.png");
                if (options.Overwrite && File.Exists(outputImage)) { File.Delete(outputImage); }
                if (options.Overwrite || !File.Exists(outputImage))
                {
                    File.WriteAllBytes(outputImage, bezel);
                }

                // resize the bezel image
                Console.WriteLine($"{game} processing image");
                ImageProcessor.Resize(
                    outputImage,
                    (int)options.TargetResolutionBounds.Width,
                    (int)options.TargetResolutionBounds.Height);

                // debug: draw target position
                DebugDraw(game, options, outputImage, newPosition);

                Console.WriteLine($"{game} creating configs");

                // create game config files
                var outputGameCfg = Path.Join(options.OutputRoms, $"{game}.zip.cfg");
                File.Copy(options.TemplateGameCfg, outputGameCfg, options.Overwrite);
                Converter.FillTemplate(outputGameCfg, game, newPosition, options.TargetResolutionBounds);

                // create overlay config files
                var outputOverlayCfg = Path.Join(options.OutputOverlays, $"{game}.cfg");
                File.Copy(options.TemplateOverlayCfg, outputOverlayCfg, options.Overwrite);
                Converter.FillTemplate(outputOverlayCfg, game, newPosition, options.TargetResolutionBounds);

                Console.WriteLine($"{game} processing done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{game} PROCESSING ERROR: {ex.Message}");
                File.AppendAllText(options.ErrorFile, $"{game} - {ex.Message}");
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

            Console.WriteLine($"{game} processing start");

            try
            {
                var target = Path.Join(options.Output, game);

                // get RA processor
                var processor = RetroArchProcessor.GetProcessor(romFile, overlayConfigFiles, options);

                Console.WriteLine($"{game} image: {processor.OverlayImageFileName}");
                Console.WriteLine($"{game} source screen: {processor.SourceScreenPosition}");

                var newPosition = new Model.Bounds();
                if (options.ScanBezelForScreenCoordinates)
                {
                    newPosition = ImageProcessor.FindScreen(File.ReadAllBytes(processor.OverlayImagePath), options);
                }
                else
                {
                    // convert from LAY and CFG
                    newPosition = processor.SourceScreenPosition;
                }

                Console.WriteLine($"{game} target screen: {newPosition}");

                // create destination folder
                if (options.Overwrite && Directory.Exists(target)) { Directory.Delete(target, true); }
                if (!Directory.Exists(target)) { Directory.CreateDirectory(target); }

                // copy overlay image
                File.Copy(processor.OverlayImagePath, Path.Join(target, processor.OverlayImageFileName), options.Overwrite);

                // resize the bezel image
                Console.WriteLine($"{game} processing image");
                ImageProcessor.Resize(
                    processor.OverlayImagePath,
                    (int)processor.SourceResolution.Width,
                    (int)processor.SourceResolution.Height);

                // debug: draw target position
                DebugDraw(game, options, processor.OverlayImagePath, newPosition);

                Console.WriteLine($"{game} creating configs");

                // create lay file
                var outputLay = Path.Join(target, "default.lay");
                File.Copy(options.Template, outputLay, options.Overwrite);
                Converter.FillTemplate(outputLay, game, newPosition, processor.SourceResolution);

                // zip overlay
                if (options.Zip)
                {
                    Console.WriteLine($"{game} zipping file");
                    var targetZip = Path.Join(options.Output, $"{game}.zip");
                    if (File.Exists(targetZip)) { File.Delete(targetZip); }
                    FileUtils.CompressFolderContent(target, targetZip);
                    Directory.Delete(target, true);
                }

                Console.WriteLine($"{game} processing done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{game} PROCESSING ERROR: {ex.Message}");
                File.AppendAllText(options.ErrorFile, $"{game} - {ex.Message}");
            }
        }

        /// <summary>
        /// Draws a debug rectangle on the overlay image
        /// </summary>
        /// <param name="game">The game name</param>
        /// <param name="options">The options</param>
        /// <param name="imagePath">The path to the image</param>
        /// <param name="position">The position of the screen</param>
        private static void DebugDraw(string game, BaseOptions options, string imagePath, Model.Bounds position)
        {
            if (!string.IsNullOrEmpty(options.OutputDebug))
            {
                Console.WriteLine($"{game} generating debug image");
                var debugImage = Path.Join(options.OutputDebug, $"{game}.png");
                File.Copy(imagePath, debugImage, true);
                ImageProcessor.DrawRect(debugImage, position);
            }
        }

        /// <summary>
        /// Runs threads on the specified files
        /// </summary>
        /// <param name="threadsNb">The number of threads to run</param>
        /// <param name="inputFiles">The input files collection</param>
        /// <param name="threadMethod">The method executed by the thread</param>
        private static void RunThreads(int threadsNb, IEnumerable<string> inputFiles, Action<string> threadMethod)
        {
            // get files to process
            var files = new ConcurrentQueue<string>(inputFiles);

            // run threads
            var threads = new List<Thread>();
            for (int i = 0; i < threadsNb; i++)
            {
                var t = new Thread(() =>
                {
                    while (files.TryDequeue(out var f))
                    {
                        threadMethod(f);
                    }
                });
                t.Start();
            }

            // wait for all threads to finish
            for (int t = 0; t < threads.Count; t++)
            {
                threads[t].Join();
            }
        }
    }
}

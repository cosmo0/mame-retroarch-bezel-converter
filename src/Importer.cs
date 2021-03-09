using Converter.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

namespace Converter
{
    public static class Importer
    {
        /// <summary>
        /// Starts the import from MAME to Retroarch
        /// </summary>
        public static void MameToRetroarch(MameToRaOptions options)
        {
            // get files to process
            var files = new ConcurrentQueue<string>(Directory.EnumerateFiles(options.Source, "*.zip").OrderBy(ff => ff));

            // run threads
            var threads = new List<Thread>();
            for (int i = 0; i < options.Threads; i++)
            {
                var t = new Thread(() =>
                {
                    while (files.TryDequeue(out var f))
                    {
                        ProcessMameFile(f, options);
                    }
                });
                t.Start();
            }

            // wait for all threads to finish
            foreach (var t in threads)
            {
                t.Join();
            }

            Console.WriteLine($"########## DONE");
        }

        /// <summary>
        /// Processes a file
        /// </summary>
        /// <param name="f">The file to process</param>
        /// <param name="options">The options</param>
        public static void ProcessMameFile(string f, MameToRaOptions options)
        {
            var fi = new FileInfo(f);
            var game = fi.Name.Replace(".zip", "");

            try
            {
                var cfgFile = Path.Join(options.Source, $"{game}.cfg");

                Console.WriteLine($"{game} processing start");

                var (lay, cfg, bezel) = FileUtils.ExtractFiles(game, f, cfgFile, options);

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
                if (!string.IsNullOrEmpty(options.OutputDebug))
                {
                    Console.WriteLine($"{game} generating debug image");
                    var debugImage = Path.Join(options.OutputDebug, $"{game}.png");
                    File.Copy(outputImage, debugImage, true);
                    ImageProcessor.DrawRect(debugImage, newPosition);
                }

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
        /// Starts the import from Retroarch to MAME
        /// </summary>
        /// <param name="options"></param>
        public static void RetroarchToMame(RaToMameOptions options)
        {
            // get files to process
            var romFiles = new ConcurrentQueue<string>(Directory.EnumerateFiles(options.SourceRoms, "*.zip.cfg").OrderBy(ff => ff));
            var configFiles = Directory.EnumerateFiles(options.SourceConfigs, "*.cfg"); // read-only, no need for it to be concurrent

            // run threads
            var threads = new List<Thread>();
            for (int i = 0; i < options.Threads; i++)
            {
                var t = new Thread(() =>
                {
                    while (romFiles.TryDequeue(out var f))
                    {
                        throw new NotImplementedException();
                    }
                });
                t.Start();
            }

            // wait for all threads to finish
            foreach (var t in threads)
            {
                t.Join();
            }

            Console.WriteLine($"########## DONE");
        }
    }
}

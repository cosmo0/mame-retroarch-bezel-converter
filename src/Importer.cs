using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

namespace Converter
{
    public static class Importer
    {
        /// <summary>
        /// Starts the import
        /// </summary>
        public static void MameToRetroarch(Options options)
        {
            // get all zip files in the source folder
            foreach (var f in Directory.EnumerateFiles(options.Source, "*.zip").OrderBy(ff => ff))
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".zip", "");
                var cfgFile = Path.Join(options.Source, $"{game}.cfg");

                Console.WriteLine($"########## PROCESSING {game}");

                var (lay, cfg, bezel) = ExtractFiles(game, f, cfgFile, options);

                // extracts the data from the MAME files
                var mameProcessor = MameProcessor.BuildProcessor(options, lay, cfg);

                Console.WriteLine($"{game} image: {mameProcessor.BezelFileName}");
                Console.WriteLine($"{game} source screen: {mameProcessor.SourceScreenPosition}");
                Console.WriteLine($"{game} screen offset: {mameProcessor.Offset}");

                throw new Exception("TODO : get position from transparency");

                // calculates the new screen position
                var newPosition = Converter.ApplyOffset(
                    mameProcessor.SourceScreenPosition,
                    mameProcessor.Offset,
                    mameProcessor.SourceResolution,
                    options.TargetResolutionBounds);

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
                Converter.FillTemplate(outputGameCfg, game, newPosition);

                // create overlay config files
                var outputOverlayCfg = Path.Join(options.OutputOverlays, $"{game}.cfg");
                File.Copy(options.TemplateOverlayCfg, outputOverlayCfg, options.Overwrite);
                Converter.FillTemplate(outputOverlayCfg, game, newPosition);

                Console.WriteLine($"{game} done");
            }

            Console.WriteLine($"########## DONE");
        }

        /// <summary>
        /// Removes everything in a folder without removing the folder itself
        /// </summary>
        /// <param name="folder">The folder to clean</param>
        private static void CleanupFolder(string folder)
        {
            DirectoryInfo di = new DirectoryInfo(folder);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        /// Deserializes the specified XML file
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="filePath">The path to the XML file</param>
        /// <returns>The deserialiazed XML</returns>
        private static T DeserializeXmlFile<T>(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fileStream);
            }
        }

        /// <summary>
        /// Deserializes the specified XML file stream.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>The deserialized XML</returns>
        private static T DeserializeXmlFile<T>(Stream fileStream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            return (T)serializer.Deserialize(fileStream);
        }

        /// <summary>
        /// Extracts and deserializes the LAY and CFG files
        /// </summary>
        /// <param name="game">The game name.</param>
        /// <param name="zipFile">The zip file path.</param>
        /// <param name="cfgFile">The CFG file path.</param>
        /// <param name="tmpFolder">The temporary folder path.</param>
        /// <returns>The deserialized LAY and CFG files</returns>
        /// <exception cref="Exceptions.LayFileException">Unable to find a view in the LAY file</exception>
        private static (Model.LayFile lay, Model.CfgFile cfg, byte[] bezel) ExtractFiles(string game, string zipFile, string cfgFile, Options options)
        {
            Model.LayFile lay;
            byte[] bezel = null;

            Console.WriteLine($"{game} Extracting files from archive {zipFile}");

            // extract files
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                // get layout file
                var layEntry = archive.GetEntry("default.lay");
                if (layEntry == null) { throw new Exceptions.LayFileException($"Unable to find default.lay file in {zipFile}"); }
                using (Stream layStream = layEntry.Open())
                {
                    lay = DeserializeXmlFile<Model.LayFile>(layStream);
                }

                // check that LAY is useful
                if (!lay.Views.Any()) { throw new Exceptions.LayFileException("Unable to find a view in the LAY file"); }

                // get associated bezel
                var view = MameProcessor.GetView(lay, options.UseFirstView);
                var bezelFileNameInLay = MameProcessor.GetBezelFile(lay, view);
                // sometimes the bezel file name in LAY doesn't have the same case as the actual file
                var bezelFileNameInZip = FindFile(archive, bezelFileNameInLay);
                var bezelEntry = archive.GetEntry(bezelFileNameInZip);
                if (bezelEntry == null) { throw new Exceptions.BezelNotFoundException($"Unable to find bezel file {bezelFileNameInZip} in {zipFile}"); }
                using (Stream bezelStream = bezelEntry.Open())
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bezelStream.CopyTo(ms);
                        ms.Position = 0;
                        bezel = ms.ToArray();
                    }
                }
            }

            // parse the config file if it exists
            Model.CfgFile cfg = null;
            if (File.Exists(cfgFile))
            {
                Console.WriteLine($"{game} config file exists");
                cfg = DeserializeXmlFile<Model.CfgFile>(cfgFile);
            }
            else
            {
                Console.WriteLine($"{game} doesn't have a cfg file");
            }

            return (lay, cfg, bezel);
        }

        /// <summary>
        /// Searches for a file in a case-sensitive way, and returns the actual file name
        /// </summary>
        /// <param name="folder">The folder to search</param>
        /// <param name="fileName">The file name to search</param>
        /// <returns>The proper file name, case sensitiv-ed</returns>
        private static string FindFile(ZipArchive archive, string fileName)
        {
            foreach (var e in archive.Entries)
            {
                if (e.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return e.Name;
                }
            }

            throw new FileNotFoundException($"Unable to find file {fileName} in archive");
        }
    }
}

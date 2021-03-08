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
        public static void Run(Options options)
        {
            var tmp = Path.Join(options.OutputOverlays, "tmp");
            if (!Directory.Exists(tmp))
            {
                Directory.CreateDirectory(tmp);
            }
            else
            {
                CleanupFolder(tmp);
            }

            // get all zip files in the source folder
            foreach (var f in Directory.EnumerateFiles(options.Source, "*.zip").OrderBy(ff => ff))
            {
                var fi = new FileInfo(f);
                var game = fi.Name.Replace(".zip", "");
                var cfgFile = Path.Join(options.Source, $"{game}.cfg");

                Console.WriteLine($"########## PROCESSING {game}");

                var (lay, cfg) = ExtractFiles(game, f, cfgFile, tmp);

                // extracts the data from the MAME files
                var mameProcessor = MameProcessor.BuildProcessor(options, lay, cfg);

                Console.WriteLine($"{game} image: {mameProcessor.BezelFileName}");
                Console.WriteLine($"{game} source screen: {mameProcessor.SourceScreenPosition}");
                Console.WriteLine($"{game} screen offset: {mameProcessor.Offset}");

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
                    File.Copy(FindFile(tmp, mameProcessor.BezelFileName), outputImage);
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

                // clean
                CleanupFolder(tmp);
            }

            Directory.Delete(tmp, true);

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
        /// Extracts and deserializes the LAY and CFG files
        /// </summary>
        /// <param name="game">The game name.</param>
        /// <param name="zipFile">The zip file path.</param>
        /// <param name="cfgFile">The CFG file path.</param>
        /// <param name="tmpFolder">The temporary folder path.</param>
        /// <returns>The deserialized LAY and CFG files</returns>
        /// <exception cref="Exceptions.LayFileException">Unable to find a view in the LAY file</exception>
        private static (Model.LayFile, Model.CfgFile) ExtractFiles(string game, string zipFile, string cfgFile, string tmpFolder)
        {
            // extract files
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                archive.ExtractToDirectory(tmpFolder);
            }

            // parse the layout file
            Model.LayFile lay = DeserializeXmlFile<Model.LayFile>(Path.Join(tmpFolder, "default.lay"));
            if (!lay.Views.Any())
            {
                throw new Exceptions.LayFileException("Unable to find a view in the LAY file");
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

            return (lay, cfg);
        }

        /// <summary>
        /// Searches for a file in a case-sensitive way, and returns the actual file name
        /// </summary>
        /// <param name="folder">The folder to search</param>
        /// <param name="fileName">The file name to search</param>
        /// <returns>The proper file name, case sensitiv-ed</returns>
        private static string FindFile(string folder, string fileName)
        {
            foreach (var f in Directory.GetFiles(folder))
            {
                var fi = new FileInfo(f);
                if (fi.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return fi.FullName;
                }
            }

            throw new FileNotFoundException($"Unable to find file {fileName} in folder {folder}");
        }
    }
}

using Converter.Options;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

namespace Converter
{
    /// <summary>
    /// Utilities for file management
    /// </summary>
    public static class FileUtils
    {
        /// <summary>
        /// Compresses the content of the folder.
        /// </summary>
        /// <param name="targetFolder">The target folder to compress.</param>
        /// <param name="targetZip">The target zip to compress into.</param>
        public static void CompressFolderContent(string targetFolder, string targetZip)
        {
            ZipFile.CreateFromDirectory(targetFolder, targetZip);
        }

        /// <summary>
        /// Deserializes the specified XML file
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="filePath">The path to the XML file</param>
        /// <returns>The deserialiazed XML</returns>
        public static T DeserializeXmlFile<T>(string filePath)
        {
            using (var fileStream = File.Open(filePath, FileMode.Open))
            {
                var serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(fileStream);
            }
        }

        /// <summary>
        /// Deserializes the specified XML file stream.
        /// </summary>
        /// <typeparam name="T">The type to deserialize into</typeparam>
        /// <param name="fileStream">The file stream.</param>
        /// <returns>The deserialized XML</returns>
        public static T DeserializeXmlFile<T>(Stream fileStream)
        {
            var serializer = new XmlSerializer(typeof(T));
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
        public static (Model.MameLayFile lay, Model.MameCfgFile cfg, byte[] bezel) ExtractFiles(string game, string zipFile, string cfgFile, MameToRaOptions options)
        {
            Model.MameLayFile lay;
            byte[] bezel = null;

            Console.WriteLine($"{game} Extracting files from archive {zipFile}");

            // extract files
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                // get layout file
                var layEntry = archive.Entries.Where(e => e.Name.EndsWith("default.lay", StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                if (layEntry == null) { throw new Exceptions.LayFileException($"Unable to find default.lay file in {zipFile}"); }
                using (Stream layStream = layEntry.Open())
                {
                    lay = DeserializeXmlFile<Model.MameLayFile>(layStream);
                }

                // check that LAY is useful
                if (!lay.Views.Any()) { throw new Exceptions.LayFileException("Unable to find a view in the LAY file"); }

                // get associated bezel
                var view = MameProcessor.GetView(lay, options.UseFirstView);
                var bezelFileNameInLay = MameProcessor.GetBezelFile(lay, view);
                if (!string.IsNullOrEmpty(bezelFileNameInLay))
                {
                    // sometimes the bezel file name in LAY doesn't have the same case as the actual file
                    var bezelFileNameInZip = FindFile(archive, bezelFileNameInLay);
                    var bezelEntry = archive.Entries.Where(e => e.Name.EndsWith(bezelFileNameInZip, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                    if (bezelEntry == null) { throw new Exceptions.BezelNotFoundException($"Unable to find bezel file {bezelFileNameInZip} in {zipFile}"); }
                    using (var bezelStream = bezelEntry.Open())
                    {
                        using (var ms = new MemoryStream())
                        {
                            bezelStream.CopyTo(ms);
                            ms.Position = 0;
                            bezel = ms.ToArray();
                        }
                    }
                }
            }

            // parse the config file if it exists
            Model.MameCfgFile cfg = null;
            if (File.Exists(cfgFile))
            {
                Console.WriteLine($"{game} config file exists");
                cfg = DeserializeXmlFile<Model.MameCfgFile>(cfgFile);
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
        public static string FindFile(ZipArchive archive, string fileName)
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

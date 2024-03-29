﻿using BezelTools.Model;
using BezelTools.Options;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Xml.Serialization;

namespace BezelTools
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
        /// <exception cref="Exceptions.LayFileException">
        /// Unable to find a view in the LAY file
        /// </exception>
        public static (MameLayFile lay, MameCfgFile cfg, byte[] bezel) ExtractFiles(string game, string zipFile, string cfgFile, MameToRaOptions options)
        {
            MameLayFile lay = null;
            MameCfgFile cfg = null;
            byte[] bezel = null;

            Interaction.Log($"{game} Extracting files from archive {zipFile}");

            // extract files
            using (ZipArchive archive = ZipFile.OpenRead(zipFile))
            {
                // get layout file
                var layEntry = archive.Entries.FirstOrDefault(e => e.Name.EndsWith("default.lay", StringComparison.InvariantCultureIgnoreCase));
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
                    var bezelEntry = archive.Entries.FirstOrDefault(e => e.Name.EndsWith(bezelFileNameInZip, StringComparison.InvariantCultureIgnoreCase));
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

            cfg = GetConfigFile(cfgFile, game);

            return (lay, cfg, bezel);
        }

        /// <summary>
        /// Fill a template config with the specified values
        /// </summary>
        /// <param name="configPath">The configuration path.</param>
        /// <param name="game">The game name</param>
        public static void FillTemplate(string configPath, string game)
        {
            FillTemplate(configPath, game, null, null);
        }

        /// <summary>
        /// Fill a template config with the specified values
        /// </summary>
        /// <param name="configPath">The configuration path.</param>
        /// <param name="game">The game name</param>
        /// <param name="position">The position of the image</param>
        public static void FillTemplate(string configPath, string game, Bounds position)
        {
            FillTemplate(configPath, game, position, null);
        }

        /// <summary>
        /// Fill a template config with the specified values
        /// </summary>
        /// <param name="configPath">The path to the config file to fill</param>
        /// <param name="game">The game name</param>
        /// <param name="position">The position of the image</param>
        /// <param name="resolution">The target resolution</param>
        public static void FillTemplate(string configPath, string game, Bounds position, Bounds resolution)
        {
            var content = File.ReadAllText(configPath);

            content = FillTemplateContent(content, game, position, resolution);

            File.WriteAllText(configPath, content);
        }

        /// <summary>
        /// Fills the specified config content template with the specified infos
        /// </summary>
        /// <param name="content">The content to fill</param>
        /// <param name="game">The game name</param>
        /// <param name="position">The screen position</param>
        /// <param name="resolution">The target resolution</param>
        /// <returns>The filled content</returns>
        public static string FillTemplateContent(string content, string game, Bounds position, Bounds resolution)
        {
            content = content.Replace("{{game}}", game);

            if (position != null)
            {
                content = content.Replace("{{width}}", Math.Round(position.Width, 0).ToString())
                    .Replace("{{height}}", Math.Round(position.Height, 0).ToString())
                    .Replace("{{x}}", Math.Round(position.X, 0).ToString())
                    .Replace("{{y}}", Math.Round(position.Y, 0).ToString())
                    .Replace("{{orientation}}", position.Orientation.ToString().ToLower());
            }

            if (resolution != null)
            {
                content = content
                    .Replace("{{width_res}}", Math.Round(resolution.Width, 0).ToString())
                    .Replace("{{height_res}}", Math.Round(resolution.Height, 0).ToString());
            }

            return content;
        }

        /// <summary>
        /// Searches for a file in a case-sensitive way, and returns the actual file name
        /// </summary>
        /// <param name="fileName">The file name to search</param>
        /// <returns>The proper file name, case sensitiv-ed</returns>
        public static string FindFile(ZipArchive archive, string fileName)
        {
            var found = archive.Entries.FirstOrDefault(e => e.Name.Equals(fileName, StringComparison.InvariantCultureIgnoreCase));
            if (found != null)
            {
                return found.Name;
            }

            throw new FileNotFoundException($"Unable to find file {fileName} in archive");
        }

        /// <summary>
        /// Normalizes a path so it's understandable by System.IO.Path
        /// </summary>
        /// <param name="path">The path to normalize</param>
        /// <returns>The normalized path</returns>
        public static string NormalizePath(string path)
        {
            // RetroArch has this weird syntax where ":\" means "at the RA root"
            path = path.StartsWith(":") ? path[1..] : path;

            // Windows handles *nix slashes fine, the opposite is not true
            path = path.Replace("\\", "/");

            return path;
        }

        /// <summary>
        /// Reads the files in the specified folder
        /// </summary>
        /// <param name="game">The game name</param>
        /// <param name="folder">The folder to read</param>
        /// <param name="cfgFile">The config file</param>
        /// <param name="options">The options</param>
        /// <returns>The parsed files</returns>
        public static (MameLayFile lay, MameCfgFile cfg, byte[] bezel) ReadFiles(string game, string folder, string cfgFile, MameToRaOptions options)
        {
            byte[] bezel = null;

            Interaction.Log($"{game} Reading files from folder {folder}");

            // get layout and bezel
            var layFiles = Directory.GetFiles(folder, "default.lay");
            if (layFiles == null || !layFiles.Any()) { throw new FileNotFoundException($"Unable to find a default.lay file in {folder}"); }
            MameLayFile lay = DeserializeXmlFile<Model.MameLayFile>(layFiles.First());

            // check that LAY is useful
            if (!lay.Views.Any()) { throw new Exceptions.LayFileException("Unable to find a view in the LAY file"); }
            var view = MameProcessor.GetView(lay, options.UseFirstView);
            var bezelFileNameInLay = MameProcessor.GetBezelFile(lay, view);
            if (!string.IsNullOrEmpty(bezelFileNameInLay))
            {
                var bezelFilePath = Directory.GetFiles(folder, bezelFileNameInLay, new EnumerationOptions { MatchCasing = MatchCasing.CaseInsensitive });
                if (bezelFilePath == null || !bezelFilePath.Any()) { throw new FileNotFoundException($"Unable to find the bezel file {bezelFileNameInLay}"); }
                bezel = File.ReadAllBytes(bezelFilePath.First());
            }

            // get config file
            MameCfgFile cfg = GetConfigFile(cfgFile, game);

            return (lay, cfg, bezel);
        }

        /// <summary>
        /// Resolves a path
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns>The resolved path</returns>
        public static string ResolvePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) { return null; }

            path = path.Replace("~", Environment.GetFolderPath(Environment.SpecialFolder.UserProfile));
            path = Path.GetFullPath(path);
            return path;
        }

        /// <summary>
        /// Parses the specified config file
        /// </summary>
        /// <param name="cfgFile">The config file</param>
        /// <param name="game">The game name</param>
        /// <returns>The parsed config file</returns>
        private static MameCfgFile GetConfigFile(string cfgFile, string game)
        {
            // parse the config file if it exists
            if (!string.IsNullOrEmpty(cfgFile) && File.Exists(cfgFile))
            {
                Interaction.Log($"{game} MAME config file exists");
                return DeserializeXmlFile<Model.MameCfgFile>(cfgFile);
            }
            else
            {
                Interaction.Log($"{game} doesn't have a MAME config file");
                return null;
            }
        }
    }
}
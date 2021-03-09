using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Converter.Options;

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
				var t = new Thread(() => {
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

				var (lay, cfg, bezel) = ExtractFiles(game, f, cfgFile, options);

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
				var t = new Thread(() => {
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
		private static T DeserializeXmlFile<T>(Stream fileStream)
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
		private static (Model.LayFile lay, Model.CfgFile cfg, byte[] bezel) ExtractFiles(string game, string zipFile, string cfgFile, MameToRaOptions options)
		{
			Model.LayFile lay;
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
					lay = DeserializeXmlFile<Model.LayFile>(layStream);
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

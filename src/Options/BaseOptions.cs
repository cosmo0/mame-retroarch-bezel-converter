using BezelTools.Model;
using CommandLine;
using System;
using System.IO;
using System.Reflection;

namespace BezelTools.Options
{
    /// <summary>
    /// Command line base option arguments
    /// </summary>
    public abstract class BaseOptions
    {
        /// <summary>
        /// Gets the assembly directory path
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                string fullPath = Assembly.GetAssembly(typeof(Program)).Location;
                return Path.GetDirectoryName(fullPath);
            }
        }

        /// <summary>
        /// Gets or sets the path to the error lists file
        /// </summary>
        [Option('e', "error-file", Required = false, HelpText = "The path to the CSV file containing the list of errors")]
        public string ErrorFile { get; set; }

        /// <summary>
        /// Gets or sets the margins applied to the screen after conversion.
        /// </summary>
        [Option("margin", Required = false, HelpText = "Applies a margin to the screen (to hide a bit of it)", Default = 0)]
        public int Margin { get; set; } = 0;

        /// <summary>
        /// Gets or sets a path for debug purpose
        /// </summary>
        [Option('d', "output-debug", Required = false, HelpText = "The folder where debug overlays will be created", Default = "")]
        public string OutputDebug { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to overwrite existing files
        /// </summary>
        [Option("overwrite", Required = false, HelpText = "Overwrites existing files", Default = true)]
        public bool Overwrite { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether to scan the bezel for screen position or just convert LAY file.
        /// </summary>
        [Option("scan-bezel", Required = false, HelpText = "Scans the bezel file for transparent pixels to find the screen position ; otherwise, just convert the LAY file", Default = false)]
        public bool ScanBezelForScreenCoordinates { get; set; } = false;

        /// <summary>
        /// Gets or sets the target overlay resolution
        /// </summary>
        [Option("target-resolution", Required = false, HelpText = "The target resolution", Default = "1920x1080")]
        public string TargetResolution { get; set; }

        /// <summary>
        /// Gets the target resolution bounds
        /// </summary>
        public Bounds TargetResolutionBounds
        {
            get
            {
                var splitRes = TargetResolution.Split(new char[] { 'x', '*', ':', '/' });
                if (splitRes.Length < 2) { throw new ArgumentOutOfRangeException(nameof(TargetResolution), $"Unable to parse target resolution ({TargetResolution})"); }

                return new Bounds
                {
                    X = 0,
                    Y = 0,
                    Width = int.Parse(splitRes[0]),
                    Height = int.Parse(splitRes[1])
                };
            }
        }

        /// <summary>
        /// Gets or sets the number of threads on which to run the conversion
        /// </summary>
        [Option("threads", Required = false, HelpText = "Number of threads on which to run", Default = 1)]
        public int Threads { get; set; } = 1;
    }
}

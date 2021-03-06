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
        private string errorFile;
        private string outputDebug;

        /// <summary>
        /// Gets or sets the path to the error lists file
        /// </summary>
        [Option('e', "error-file", Required = false, HelpText = "The path to the CSV file containing the list of errors")]
        public string ErrorFile { get => errorFile; set => errorFile = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the margins applied to the screen after conversion.
        /// </summary>
        [Option("margin", Required = false, HelpText = "Applies a margin to the screen (to hide a bit of it)", Default = 0)]
        public int Margin { get; set; } = 0;

        /// <summary>
        /// Gets or sets a path for debug purpose
        /// </summary>
        [Option('d', "output-debug", Required = false, HelpText = "The folder where debug overlays will be created", Default = "")]
        public string OutputDebug { get => outputDebug; set => outputDebug = FileUtils.ResolvePath(value); }

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

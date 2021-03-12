using CommandLine;

namespace BezelTools.Options
{
    /// <summary>
    /// Base options for conversion
    /// </summary>
    /// <seealso cref="BezelTools.Options.BaseOptions" />
    public abstract class BaseConvertOptions : BaseOptions
    {
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
    }
}

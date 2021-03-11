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
    }
}

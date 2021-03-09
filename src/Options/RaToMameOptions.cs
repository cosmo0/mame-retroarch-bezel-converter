using CommandLine;
using System;
namespace Converter.Options
{
    /// <summary>
    /// Converts a Retroarch overlay to a MAME bezel
    /// </summary>
    [Verb("rtm", HelpText = "Converts overlays from Retroarch to MAME")]
    public class RaToMameOptions : BaseOptions
    {
        /// <summary>
        /// Gets or sets the path to the output
        /// </summary>
        [Option('o', "output", Required = true, HelpText = "The folder where the bezels will be created")]
        public string Output { get; set; }

        /// <summary>
        /// Gets or sets the path to the source Retroarch overlays configs
        /// </summary>
        [Option('c', "source-configs", Required = true, HelpText = "The folder where the Retroarch bezels configs are located")]
        public string SourceConfigs { get; set; }

        /// <summary>
        /// Gets or sets the path to the source Retroarch roms configs
        /// </summary>
        [Option('r', "source-roms", Required = true, HelpText = "The folder where the Retroarch roms configs are located")]
        public string SourceRoms { get; set; }

        /// <summary>
        /// Gets or sets the path to the game config template
        /// </summary>
        [Option('t', "template", Required = true, HelpText = "The path to the default.lay template")]
        public string Template { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the bezels will be zipped
        /// </summary>
        [Option('z', "zip", Required = false, HelpText = "Whether to zip the output bezels", Default = false)]
        public bool Zip { get; set; } = false;
    }
}

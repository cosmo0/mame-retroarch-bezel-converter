using CommandLine;

namespace BezelTools.Options
{
    /// <summary>
    /// Options for conversion from MAME to Retroarch
    /// </summary>
    [Verb("mtr", HelpText = "Converts overlays from MAME to Retroarch")]
    public class MameToRaOptions : BaseConvertOptions
    {
        private string outputOverlays;
        private string outputRoms;
        private string source;
        private string sourceConfigs;
        private string templateGameCfg;
        private string templateOverlayCfg;

        /// <summary>
        /// Gets or sets the path to the output overlays
        /// </summary>
        [Option('o', "output-overlays", Required = true, HelpText = "The folder where the overlays configs and images will be created")]
        public string OutputOverlays { get => outputOverlays; set => outputOverlays = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the output roms
        /// </summary>
        [Option('r', "output-roms", Required = true, HelpText = "The folder where the ROM configs will be created")]
        public string OutputRoms { get => outputRoms; set => outputRoms = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the source MAME bezels
        /// </summary>
        [Option('s', "source", Required = true, HelpText = "The folder where the MAME artworks are located")]
        public string Source { get => source; set => source = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the MAME configs folder
        /// </summary>
        [Option("source-configs", Required = false, HelpText = "The folder where the MAME configs are located", Default = "")]
        public string SourceConfigs { get => sourceConfigs; set => sourceConfigs = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the game config template
        /// </summary>
        [Option("template-game", Required = true, HelpText = "The path to the template for the game config")]
        public string TemplateGameCfg { get => templateGameCfg; set => templateGameCfg = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the overlay config template
        /// </summary>
        [Option("template-overlay", Required = true, HelpText = "The path to the template for the overlay config")]
        public string TemplateOverlayCfg { get => templateOverlayCfg; set => templateOverlayCfg = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets a value indicating whether to use the first view, if multiple are found
        /// </summary>
        [Option("use-fist-view", Required = false, HelpText = "Uses the first found view to generate an overlay", Default = true)]
        public bool UseFirstView { get; set; } = true;
    }
}

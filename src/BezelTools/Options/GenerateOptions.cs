using CommandLine;

namespace BezelTools.Options
{
    /// <summary>
    /// Generates overlay files from images
    /// </summary>
    [Verb("generate", HelpText = "Generates overlay files from images")]
    public class GenerateOptions : BaseOptions
    {
        private string imagesFolder;
        private string romsFolder;
        private string templateOverlay;
        private string templateRom;

        /// <summary>
        /// Gets or sets the path to the overlays configuration folder.
        /// </summary>
        [Option('i', "images", Required = true, HelpText = "The path to the images folder")]
        public string ImagesFolder { get => imagesFolder; set => imagesFolder = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets a value indicating whether existing files will be overwritten
        /// </summary>
        [Option("overwrite", Required = false, HelpText = "Whether to overwrite existing files", Default = false)]
        public bool Overwrite { get; set; } = false;

        /// <summary>
        /// Gets or sets the path to the roms folder.
        /// </summary>
        [Option('r', "roms-config", Required = true, HelpText = "The path to the rom configs folder")]
        public string RomsFolder { get => romsFolder; set => romsFolder = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the overlay template.
        /// </summary>
        [Option("template-overlay", Required = true, HelpText = "The path to the overlay config template file")]
        public string TemplateOverlay { get => templateOverlay; set => templateOverlay = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the rom template.
        /// </summary>
        [Option("template-rom", Required = true, HelpText = "The path to the rom config template file")]
        public string TemplateRom { get => templateRom; set => templateRom = FileUtils.ResolvePath(value); }
    }
}

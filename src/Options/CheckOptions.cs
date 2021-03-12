using CommandLine;

namespace BezelTools.Options
{
    /// <summary>
    /// Checks overlay files integrity
    /// </summary>
    [Verb("check", HelpText = "Checks overlay files integrity")]
    public class CheckOptions : BaseOptions
    {
        private string overlaysConfigFolder;
        private string romsConfigFolder;
        private string templateOverlay;
        private string templateRom;

        /// <summary>
        /// Gets or sets a value indicating whether to automatically fix overlays.
        /// </summary>
        [Option('f', "autofix", Group = "fix", Required = false, HelpText = "Whether to auto-fix the files", Default = false)]
        public bool AutoFix { get; set; } = false;

        /// <summary>
        /// Gets or sets the path to the overlay configuration in rom configuration (input_overlay).
        /// </summary>
        [Option('p', "input-overlay-path", Group = "fix", Required = true, HelpText = "When fixing configs, the path to the overlay that should be added in the rom config (input_overlay); ex: /opt/retropie/configs/all/retroarch/overlay/")]
        public string InputOverlayConfigPathInRomConfig { get; set; }

        /// <summary>
        /// Gets or sets the path to the overlays configuration folder.
        /// </summary>
        [Option('o', "overlays-config", Required = true, HelpText = "The path to the overlays configs folder")]
        public string OverlaysConfigFolder { get => overlaysConfigFolder; set => overlaysConfigFolder = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the roms configuration folder.
        /// </summary>
        [Option('r', "roms-config", Required = true, HelpText = "The path to the rom configs folder")]
        public string RomsConfigFolder { get => romsConfigFolder; set => romsConfigFolder = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the path to the overlay template.
        /// </summary>
        [Option("template-overlay", Group = "fix", Required = true, HelpText = "The path to the overlay config template file")]
        public string TemplateOverlay { get => templateOverlay; set => templateOverlay = FileUtils.ResolvePath(value); }

        /// <summary>
        /// Gets or sets the rom template.
        /// </summary>
        [Option("template-rom", Group = "fix", Required = true, HelpText = "The path to the rom config template file")]
        public string TemplateRom { get => templateRom; set => templateRom = FileUtils.ResolvePath(value); }
    }
}

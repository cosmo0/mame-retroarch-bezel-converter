﻿using CommandLine;
using Converter.Model;
using System;

namespace Converter.Options
{
    /// <summary>
    /// Options for conversion from MAME to Retroarch
    /// </summary>
    [Verb("mtr", HelpText = "Converts overlays from MAME to Retroarch")]
    public class MameToRaOptions : BaseOptions
    {
        /// <summary>
        /// Gets or sets the path to the output overlays
        /// </summary>
        [Option('o', "output-overlays", Required = true, HelpText = "The folder where the overlays configs and images will be created")]
        public string OutputOverlays { get; set; }

        /// <summary>
        /// Gets or sets the path to the output roms
        /// </summary>
        [Option('r', "output-roms", Required = true, HelpText = "The folder where the ROM configs will be created")]
        public string OutputRoms { get; set; }

        /// <summary>
        /// Gets or sets the path to the source MAME bezels
        /// </summary>
        [Option('s', "source", Required = true, HelpText = "The folder where the MAME artworks are located")]
        public string Source { get; set; }

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
        /// Gets or sets the path to the game config template
        /// </summary>
        [Option("template-game", Required = true, HelpText = "The path to the template for the game config")]
        public string TemplateGameCfg { get; set; }

        /// <summary>
        /// Gets or sets the path to the overlay config template
        /// </summary>
        [Option("template-overlay", Required = true, HelpText = "The path to the template for the overlay config")]
        public string TemplateOverlayCfg { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use the first view, if multiple are found
        /// </summary>
        [Option("use-fist-view", Required = false, HelpText = "Uses the first found view to generate an overlay", Default = true)]
        public bool UseFirstView { get; set; } = true;
    }
}
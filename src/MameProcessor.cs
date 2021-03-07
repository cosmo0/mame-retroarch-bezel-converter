﻿using System;
using System.Linq;
using Converter.Model;

namespace Converter
{
    /// <summary>
    /// Processor for a MAME artwork
    /// </summary>
    public class MameProcessor
    {
        private readonly LayFile.View view;
        private readonly Offset offset;
        private readonly string bezelFileName;
        private readonly Bounds sourceResolution;
        private readonly Bounds sourceScreenPosition;
        private readonly LayFile lay;

        /// <summary>
        /// Gets the bezel file name
        /// </summary>
        public string BezelFileName => bezelFileName;

        /// <summary>
        /// Gets the source resolution
        /// </summary>
        public Bounds SourceResolution => sourceResolution;

        /// <summary>
        /// Gets the source screen coordinates
        /// </summary>
        public Bounds SourceScreenPosition => sourceScreenPosition;

        /// <summary>
        /// Gets the offset, if any
        /// </summary>
        public Offset Offset => offset;

        /// <summary>
        /// Initializes a new MameProcessor instance
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="lay">The LAY file</param>
        /// <param name="offset">The offset, if any</param>
        /// <param name="processedView">The processed view</param>
        /// <param name="sourceResolution">The source resolution</param>
        /// <param name="sourceScreenCoordinates">The source screen coordinates</param>
        /// <param name="bezelFileName">The bezel file name</param>
        private MameProcessor(LayFile lay, Offset offset, LayFile.View processedView, Bounds sourceResolution, Bounds sourceScreenCoordinates, string bezelFileName)
        {
            this.sourceResolution = sourceResolution;
            this.sourceScreenPosition = sourceScreenCoordinates;
            this.lay = lay;
            this.view = processedView;
            this.offset = offset;
            this.bezelFileName = bezelFileName;
        }

        /// <summary>
        /// Gets the source screen coordinates
        /// </summary>
        /// <returns></returns>
        public static Bounds GetSourceScreenCoordinates(LayFile.View view)
        {
            if (view.Screens == null || !view.Screens.Any()) { throw new Exceptions.LayFileException($"No screen found in view {view.Name}"); }

            return view.Screens.First().Bounds;
        }

        /// <summary>
        /// Factory for the MAME processor
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="lay">The LAY file</param>
        /// <param name="cfg">The CFG file</param>
        /// <returns>The processor</returns>
        public static MameProcessor BuildProcessor(Options options, LayFile lay, CfgFile cfg)
        {
            // extract source data
            var view = GetView(lay, options.UseFirstView);
            var sourceResolution = GetSourceResolution(view);
            var bezelFile = GetBezelFile(lay, view);
            var sourceScreenCoordinates = GetSourceScreenCoordinates(view);
            var offset = GetOffset(cfg);

            return new MameProcessor(lay, offset, view, sourceResolution, sourceScreenCoordinates, bezelFile);
        }

        /// <summary>
        /// Gets the offset from the config file
        /// </summary>
        /// <param name="cfg">The CFG file</param>
        /// <returns>The offset</returns>
        private static Offset GetOffset(CfgFile cfg)
        {
            var screen = cfg?.SystemConfig?.VideoConfig?.VideoScreen;

            // check that at least an offset has a value
            if (screen != null &&
                (screen.HOffset != Offset.DEFAULT_OFFSET
                    || screen.HStretch != Offset.DEFAULT_STRETCH
                    || screen.VOffset != Offset.DEFAULT_OFFSET
                    || screen.VStretch != Offset.DEFAULT_STRETCH))
            {
                return new Offset
                {
                    HOffset = screen.HOffset,
                    VOffset = screen.VOffset,
                    HStretch = screen.HStretch == 0 ? Offset.DEFAULT_STRETCH : screen.HStretch,
                    VStretch = screen.VStretch == 0 ? Offset.DEFAULT_STRETCH : screen.VStretch
                };
            }

            return null;
        }

        /// <summary>
        /// Gets the source resolution for the specified view
        /// </summary>
        /// <param name="view">The view to process</param>
        /// <returns>The source resolution</returns>
        public static Bounds GetSourceResolution(Model.LayFile.View view)
        {
            var bezelOfView = view.Bezels.FirstOrDefault();
            if (bezelOfView == null) { throw new Exceptions.BezelNotFoundException($"Unable to find bezel for view {view.Name}"); }
            if (bezelOfView.Bounds.X != 0 || bezelOfView.Bounds.Y != 0) { throw new Exceptions.CoordinatesException($"Bezel of view {view.Name} does not have coordinates starting at (0,0)"); }
            return bezelOfView.Bounds;
        }

        /// <summary>
        /// Gets the bezel file name from the LAY file and the view
        /// </summary>
        /// <param name="lay">The LAY file</param>
        /// <param name="view">The processed view</param>
        /// <returns>The bezel file name</returns>
        public static string GetBezelFile(Model.LayFile lay, Model.LayFile.View view)
        {
            var bezelElementName = view.Bezels.First().ElementName;

            var element = lay.Elements.Where(e => e.Name == bezelElementName).FirstOrDefault();
            if (element == null) { throw new Exceptions.LayFileException($"Unable to find element with name {bezelElementName} in LAY file"); }
            if (element.Images == null || !element.Images.Any()) { throw new Exceptions.LayFileException($"No images in element {bezelElementName} in LAY file"); }

            return element.Images.First().File;
        }

        /// <summary>
        /// Gets the processed view
        /// </summary>
        /// <param name="lay">The LAY file</param>
        /// <param name="useFirstView">Whether to automatically use the first found view</param>
        /// <returns>The processed view</returns>
        public static LayFile.View GetView(LayFile lay, bool useFirstView)
        {
            LayFile.View view;
            if (lay.Views.Length > 1 && !useFirstView)
            {
                Console.WriteLine("Please choose which bezel you want");
                for (int i = 0; i < lay.Views.Length; i++)
                {
                    Console.WriteLine($"{i}: {lay.Views[i].Name}");
                }
                string chosenView = "x";
                int viewIndex;
                while (!int.TryParse(chosenView, out viewIndex))
                {
                    chosenView = Console.ReadLine();
                }

                view = lay.Views[viewIndex];
            }
            else
            {
                view = lay.Views[0];
            }

            return view;
        }
    }
}

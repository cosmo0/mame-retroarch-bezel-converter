using BezelTools.Model;
using BezelTools.Options;
using System;
using System.Linq;

namespace BezelTools
{
    /// <summary>
    /// Processor for a MAME artwork
    /// </summary>
    public class MameProcessor
    {
        private readonly string bezelFileName;
        private readonly Offset offset;
        private readonly Bounds sourceResolution;
        private readonly Bounds sourceScreenPosition;

        /// <summary>
        /// Initializes a new MameProcessor instance
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="lay">The LAY file</param>
        /// <param name="offset">The offset, if any</param>
        /// <param name="processedView">The processed view</param>
        /// <param name="sourceResolution">The source resolution</param>
        /// <param name="sourceScreenPosition">The source screen coordinates</param>
        /// <param name="bezelFileName">The bezel file name</param>
        private MameProcessor(Offset offset, Bounds sourceResolution, Bounds sourceScreenPosition, string bezelFileName)
        {
            this.sourceResolution = sourceResolution;
            this.sourceScreenPosition = sourceScreenPosition;
            this.offset = offset;
            this.bezelFileName = bezelFileName;
        }

        /// <summary>
        /// Gets the bezel file name
        /// </summary>
        public string BezelFileName => bezelFileName;

        /// <summary>
        /// Gets the offset, if any
        /// </summary>
        public Offset Offset => offset;

        /// <summary>
        /// Gets the source resolution
        /// </summary>
        public Bounds SourceResolution => sourceResolution;

        /// <summary>
        /// Gets the source screen coordinates
        /// </summary>
        public Bounds SourceScreenPosition => sourceScreenPosition;

        /// <summary>
        /// Factory for the MAME processor
        /// </summary>
        /// <param name="options">The options</param>
        /// <param name="lay">The LAY file</param>
        /// <param name="cfg">The CFG file</param>
        /// <returns>The processor</returns>
        public static MameProcessor BuildProcessor(MameToRaOptions options, MameLayFile lay, MameCfgFile cfg)
        {
            // extract source data
            var view = GetView(lay, options.UseFirstView);
            var sourceRes = GetSourceResolution(view);
            var bezelFile = GetBezelFile(lay, view);
            var sourceScreenCoordinates = GetSourceScreenCoordinates(view);
            var off = GetOffset(cfg);

            return new MameProcessor(off, sourceRes, sourceScreenCoordinates, bezelFile);
        }

        /// <summary>
        /// Gets the bezel file name from the LAY file and the view
        /// </summary>
        /// <param name="lay">The LAY file</param>
        /// <param name="view">The processed view</param>
        /// <returns>The bezel file name</returns>
        public static string GetBezelFile(MameLayFile lay, MameLayFile.View view)
        {
            var bezelElementName = view.Bezels.First().ElementName;

            var element = lay.Elements.FirstOrDefault(e => e.Name == bezelElementName);
            if (element == null) { throw new Exceptions.LayFileException($"Unable to find an <element> with name {bezelElementName} in LAY file"); }
            if (element.Images == null || !element.Images.Any()) { throw new Exceptions.LayFileException($"No images inside <element> {bezelElementName} in LAY file"); }

            return element.Images.First().File;
        }

        /// <summary>
        /// Gets the source resolution for the specified view
        /// </summary>
        /// <param name="view">The view to process</param>
        /// <returns>The source resolution</returns>
        public static Bounds GetSourceResolution(Model.MameLayFile.View view)
        {
            if (!view.Bezels.Any()) { throw new Exceptions.BezelNotFoundException($"Unable to find a <bezel> for the <view> {view.Name}"); }

            var bezelOfView = view.Bezels.FirstOrDefault(b => b.Bounds.X == 0 && b.Bounds.Y == 0);
            if (bezelOfView == null) { throw new Exceptions.CoordinatesException($"No <bezel> inside <view> {view.Name} has coordinates starting at (0,0): I don't know how to convert"); }
            return bezelOfView.Bounds;
        }

        /// <summary>
        /// Gets the source screen coordinates
        /// </summary>
        /// <returns></returns>
        public static Bounds GetSourceScreenCoordinates(MameLayFile.View view)
        {
            if (view.Screens == null || !view.Screens.Any()) { throw new Exceptions.LayFileException($"No screen found in view {view.Name}"); }
            if (view.Screens.Length > 1) { throw new Exceptions.LayFileException($"Unable to automatically process a multi-screen machine (RetroArch doesn't support it)"); }

            var screen = view.Screens.First().Bounds;

            // base bounds: screen bounds
            var bounds = screen.Clone();

            // add overlay and backdrop positions to find the largest possible box
            if (view.Overlays != null && view.Overlays.Any())
            {
                foreach (var o in view.Overlays)
                {
                    bounds = AddToBounds(bounds, o.Bounds);
                }
            }

            if (view.Backdrops != null && view.Backdrops.Any())
            {
                foreach (var b in view.Backdrops)
                {
                    bounds = AddToBounds(bounds, b.Bounds);
                }
            }

            return bounds;
        }

        /// <summary>
        /// Gets the processed view
        /// </summary>
        /// <param name="lay">The LAY file</param>
        /// <param name="useFirstView">Whether to automatically use the first found view</param>
        /// <returns>The processed view</returns>
        public static MameLayFile.View GetView(MameLayFile lay, bool useFirstView)
        {
            MameLayFile.View view;
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

        /// <summary>
        /// Adds bounds to get the largest possible box
        /// </summary>
        /// <param name="a">The first bound</param>
        /// <param name="b">The second bound</param>
        /// <returns>The largest possible bounds</returns>
        private static Bounds AddToBounds(Bounds a, Bounds b)
        {
            var result = a.Clone();

            // b is further to the left
            if (a.X > b.X)
            {
                var diff = a.X - b.X;
                result.X -= diff;
                result.Width += diff;
            }

            // b is wider
            if (a.Width < b.Width)
            {
                result.Width += b.Width - a.Width;
            }

            // b is further to the top
            if (a.Y > b.Y)
            {
                var diff = a.Y - b.Y;
                result.Y -= diff;
                result.Height += diff;
            }

            // b is taller
            if (a.Height < b.Height)
            {
                result.Height += b.Height - a.Height;
            }

            return result;
        }

        /// <summary>
        /// Gets the offset from the config file
        /// </summary>
        /// <param name="cfg">The CFG file</param>
        /// <returns>The offset</returns>
        private static Offset GetOffset(MameCfgFile cfg)
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
    }
}

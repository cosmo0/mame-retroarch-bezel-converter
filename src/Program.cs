using BezelTools.Options;
using CommandLine;
using System.IO;

namespace BezelTools
{
    public partial class Program
    {
        /// <summary>
        /// Main application entry point
        /// </summary>
        /// <param name="args">The command line arguments</param>
        public static void Main(string[] args)
        {
            Parser.Default.ParseArguments<MameToRaOptions, RaToMameOptions>(args)
                   .WithParsed<MameToRaOptions>((o) =>
                   {
                       InitMameToRa(o);

                       Importer.ConvertMameToRetroarch(o);
                   })
                   .WithParsed<RaToMameOptions>((o) =>
                   {
                       InitRaToMame(o);

                       Importer.ConvertRetroarchToMame(o);
                   });
        }

        /// <summary>
        /// Initializes the import from MAME to Retroarch
        /// </summary>
        /// <param name="options">The options</param>
        private static void InitMameToRa(MameToRaOptions options)
        {
            // check that input folder exists
            if (!Directory.Exists(options.Source)) { throw new DirectoryNotFoundException($"Unable to find directory {options.Source}"); }

            // create folders
            if (!Directory.Exists(options.OutputRoms)) { Directory.CreateDirectory(options.OutputRoms); }
            if (!Directory.Exists(options.OutputOverlays)) { Directory.CreateDirectory(options.OutputOverlays); }
            if (!string.IsNullOrEmpty(options.OutputDebug) && !Directory.Exists(options.OutputDebug)) { Directory.CreateDirectory(options.OutputDebug); }

            // check templates
            if (!File.Exists(options.TemplateGameCfg)) { throw new FileNotFoundException("Unable to find game config template", options.TemplateGameCfg); }
            if (!File.Exists(options.TemplateOverlayCfg)) { throw new FileNotFoundException("Unable to find overlay config template", options.TemplateOverlayCfg); }
        }

        /// <summary>
        /// Initializes the import from Retroarch to MAME
        /// </summary>
        /// <param name="options">The options</param>
        private static void InitRaToMame(RaToMameOptions options)
        {
            // check that input folder exists
            if (!Directory.Exists(options.SourceConfigs)) { throw new DirectoryNotFoundException($"Unable to find directory {options.SourceConfigs}"); }
            if (!Directory.Exists(options.SourceRoms)) { throw new DirectoryNotFoundException($"Unable to find directory {options.SourceRoms}"); }

            // create folders
            if (!Directory.Exists(options.Output)) { Directory.CreateDirectory(options.Output); }
            if (!string.IsNullOrEmpty(options.OutputDebug) && !Directory.Exists(options.OutputDebug)) { Directory.CreateDirectory(options.OutputDebug); }

            // check templates
            if (!File.Exists(options.Template)) { throw new FileNotFoundException("Unable to find LAY template", options.Template); }
        }
    }
}

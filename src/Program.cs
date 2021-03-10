using BezelTools.Options;
using CommandLine;
using System;
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
            try
            {
                Parser.Default.ParseArguments<MameToRaOptions, RaToMameOptions, CheckOptions>(args)
                       .WithParsed<MameToRaOptions>((o) =>
                       {
                           InitMameToRa(o);

                           Converter.ConvertMameToRetroarch(o);
                       })
                       .WithParsed<RaToMameOptions>((o) =>
                       {
                           InitRaToMame(o);

                           Converter.ConvertRetroarchToMame(o);
                       })
                       .WithParsed<CheckOptions>((o) =>
                       {
                           InitCheck(o);

                           Checker.Check(o);
                       });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error has occurred: {ex.Message}\n\n{ex.StackTrace}");
            }
        }

        /// <summary>
        /// Initializes the check.
        /// </summary>
        /// <param name="options">The options.</param>
        /// <exception cref="DirectoryNotFoundException">
        /// Unable to find rom directory {options.RomsConfig}
        /// or
        /// Unable to find rom directory {options.RomsConfig}
        /// </exception>
        /// <exception cref="FileNotFoundException">
        /// Unable to find rom config template
        /// or
        /// Unable to find overlay config template
        /// </exception>
        private static void InitCheck(CheckOptions options)
        {
            // check input folders
            if (!Directory.Exists(options.RomsConfigFolder)) { throw new DirectoryNotFoundException($"Unable to find rom directory {options.RomsConfigFolder}"); }
            if (!Directory.Exists(options.RomsConfigFolder)) { throw new DirectoryNotFoundException($"Unable to find rom directory {options.RomsConfigFolder}"); }

            // check auto-fix
            if (options.AutoFix)
            {
                if (!string.IsNullOrEmpty(options.TemplateRom) && !File.Exists(options.TemplateRom))
                {
                    throw new FileNotFoundException("Unable to find rom config template", options.TemplateRom);
                }

                if (!string.IsNullOrEmpty(options.TemplateOverlay) && !File.Exists(options.TemplateOverlay))
                {
                    throw new FileNotFoundException("Unable to find overlay config template", options.TemplateOverlay);
                }
            }
        }

        /// <summary>
        /// Initializes the import from MAME to Retroarch
        /// </summary>
        /// <param name="options">The options</param>
        /// <exception cref="DirectoryNotFoundException">Unable to find directory {options.Source}</exception>
        /// <exception cref="FileNotFoundException">
        /// Unable to find game config template
        /// or
        /// Unable to find overlay config template
        /// </exception>
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
        /// <exception cref="DirectoryNotFoundException">
        /// Unable to find directory {options.SourceConfigs}
        /// or
        /// Unable to find directory {options.SourceRoms}
        /// </exception>
        /// <exception cref="FileNotFoundException">Unable to find LAY template</exception>
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

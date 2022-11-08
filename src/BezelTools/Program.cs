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
                Parser.Default.ParseArguments<MameToRaOptions, RaToMameOptions, CheckOptions, GenerateOptions>(args)
                       .WithParsed<MameToRaOptions>((o) =>
                       {
                           InitCommon(o);
                           InitMameToRa(o);

                           Converter.ConvertMameToRetroarch(o);
                       })
                       .WithParsed<RaToMameOptions>((o) =>
                       {
                           InitCommon(o);
                           InitRaToMame(o);

                           Converter.ConvertRetroarchToMame(o);
                       })
                       .WithParsed<CheckOptions>((o) =>
                       {
                           InitCommon(o);
                           InitCheck(o);

                           Checker.Check(o);
                       })
                       .WithParsed<GenerateOptions>((o) => {
                           InitCommon(o);
                           InitGenerate(o);

                           Generator.Generate(o);
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
            bool err = false;

            // check input folders
            if (!Directory.Exists(options.RomsConfigFolder))
            {
                Console.WriteLine($"Unable to find rom directory {options.RomsConfigFolder}");
                err = true;
            }

            if (!Directory.Exists(options.OverlaysConfigFolder))
            {
                Console.WriteLine($"Unable to find overlays directory {options.OverlaysConfigFolder}");
                err = true;
            }

            // check auto-fix
            if (options.AutoFix)
            {
                if (string.IsNullOrEmpty(options.TemplateRom) || !File.Exists(options.TemplateRom))
                {
                    Console.WriteLine($"Unable to find rom config template {options.TemplateRom}");
                    err = true;
                }

                if (string.IsNullOrEmpty(options.TemplateOverlay) || !File.Exists(options.TemplateOverlay))
                {
                    Console.WriteLine($"Unable to find overlay config template {options.TemplateOverlay}");
                    err = true;
                }
            }

            if (err)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Initializes the generation
        /// </summary>
        /// <param name="options">The generation options</param>
        private static void InitGenerate(GenerateOptions options)
        {
            bool err = false;

            //check input folder
            if (!Directory.Exists(options.ImagesFolder))
            {
                Console.WriteLine($"Unable to find image folder {options.ImagesFolder}");
                err = true;
            }

            // check output folders
            if (!Directory.Exists(options.RomsFolder))
            {
                Console.WriteLine($"Unable to find rom directory {options.RomsFolder}");
                err = true;
            }

            // check templates
            if (string.IsNullOrEmpty(options.TemplateRom) || !File.Exists(options.TemplateRom))
            {
                Console.WriteLine($"Unable to find rom config template {options.TemplateRom}");
                err = true;
            }

            if (string.IsNullOrEmpty(options.TemplateOverlay) || !File.Exists(options.TemplateOverlay))
            {
                Console.WriteLine($"Unable to find overlay config template {options.TemplateOverlay}");
                err = true;
            }

            if (err)
            {
                Environment.Exit(1);
            }
        }

        /// <summary>
        /// Initializes common parameters
        /// </summary>
        /// <param name="options">The common parameters</param>
        private static void InitCommon(BaseOptions options)
        {
            if (!string.IsNullOrEmpty(options.OutputDebug) && !Directory.Exists(options.OutputDebug))
            {
                Directory.CreateDirectory(options.OutputDebug);
            }

            if (!string.IsNullOrEmpty(options.ErrorFile) && File.Exists(options.ErrorFile))
            {
                File.Delete(options.ErrorFile);
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
            var err = false;
            // check that input folder exists
            if (!Directory.Exists(options.Source))
            {
                Console.WriteLine($"Unable to find directory {options.Source}");
                err = true;
            }

            // create folders
            if (!Directory.Exists(options.OutputRoms))
            {
                Directory.CreateDirectory(options.OutputRoms);
            }

            if (!Directory.Exists(options.OutputOverlays))
            {
                Directory.CreateDirectory(options.OutputOverlays);
            }

            // check templates
            if (!File.Exists(options.TemplateGameCfg))
            {
                Console.WriteLine($"Unable to find game config template {options.TemplateGameCfg}");
                err = true;
            }

            if (!File.Exists(options.TemplateOverlayCfg))
            {
                Console.WriteLine($"Unable to find overlay config template {options.TemplateOverlayCfg}");
                err = true;
            }

            if (err)
            {
                Environment.Exit(1);
            }
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
            var err = false;

            // check that input folder exists
            if (!Directory.Exists(options.SourceConfigs))
            {
                Console.WriteLine($"Unable to find directory {options.SourceConfigs}");
                err = true;
            }

            if (!Directory.Exists(options.SourceRoms))
            {
                Console.WriteLine($"Unable to find directory {options.SourceRoms}");
                err = true;
            }

            // create folders
            if (!Directory.Exists(options.Output))
            {
                Directory.CreateDirectory(options.Output);
            }

            if (!string.IsNullOrEmpty(options.OutputDebug) && !Directory.Exists(options.OutputDebug))
            {
                Directory.CreateDirectory(options.OutputDebug);
            }

            // check templates
            if (!File.Exists(options.Template))
            {
                Console.WriteLine($"Unable to find LAY template {options.Template}");
                err = true;
            }

            if (err)
            {
                Environment.Exit(1);
            }
        }
    }
}

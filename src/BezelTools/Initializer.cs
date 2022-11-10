using BezelTools.Options;
using System.IO;

namespace BezelTools;

public static class Initializer
{
    /// <summary>
    /// Initializes the check.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <returns>Whether the initialization has been successful</returns>
    public static bool InitCheck(CheckOptions options)
    {
        bool err = false;

        // check input folders
        if (!Directory.Exists(options.RomsConfigFolder))
        {
            Interaction.Log($"Unable to find rom directory {options.RomsConfigFolder}");
            err = true;
        }

        if (!Directory.Exists(options.OverlaysConfigFolder))
        {
            Interaction.Log($"Unable to find overlays directory {options.OverlaysConfigFolder}");
            err = true;
        }

        // check auto-fix
        if (options.AutoFix)
        {
            if (string.IsNullOrEmpty(options.TemplateRom) || !File.Exists(options.TemplateRom))
            {
                Interaction.Log($"Unable to find rom config template {options.TemplateRom}");
                err = true;
            }

            if (string.IsNullOrEmpty(options.TemplateOverlay) || !File.Exists(options.TemplateOverlay))
            {
                Interaction.Log($"Unable to find overlay config template {options.TemplateOverlay}");
                err = true;
            }
        }

        return !err;
    }

    /// <summary>
    /// Initializes common parameters
    /// </summary>
    /// <param name="options">The common parameters</param>
    public static void InitCommon(BaseOptions options)
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
    /// Initializes the generation
    /// </summary>
    /// <param name="options">The generation options</param>
    /// <returns>Whether the initialization has been successful</returns>
    public static bool InitGenerate(GenerateOptions options)
    {
        bool err = false;

        //check input folder
        if (!Directory.Exists(options.ImagesFolder))
        {
            Interaction.Log($"Unable to find image folder {options.ImagesFolder}");
            err = true;
        }

        // check output folders
        if (!Directory.Exists(options.RomsFolder))
        {
            Interaction.Log($"Unable to find rom directory {options.RomsFolder}");
            err = true;
        }

        // check templates
        if (string.IsNullOrEmpty(options.TemplateRom) || !File.Exists(options.TemplateRom))
        {
            Interaction.Log($"Unable to find rom config template {options.TemplateRom}");
            err = true;
        }

        if (string.IsNullOrEmpty(options.TemplateOverlay) || !File.Exists(options.TemplateOverlay))
        {
            Interaction.Log($"Unable to find overlay config template {options.TemplateOverlay}");
            err = true;
        }

        return !err;
    }

    /// <summary>
    /// Initializes the import from MAME to Retroarch
    /// </summary>
    /// <param name="options">The options</param>
    /// <returns>Whether the initialization has been successful</returns>
    public static bool InitMameToRa(MameToRaOptions options)
    {
        var err = false;
        // check that input folder exists
        if (!Directory.Exists(options.Source))
        {
            Interaction.Log($"Unable to find directory {options.Source}");
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
            Interaction.Log($"Unable to find game config template {options.TemplateGameCfg}");
            err = true;
        }

        if (!File.Exists(options.TemplateOverlayCfg))
        {
            Interaction.Log($"Unable to find overlay config template {options.TemplateOverlayCfg}");
            err = true;
        }

        return !err;
    }

    /// <summary>
    /// Initializes the import from Retroarch to MAME
    /// </summary>
    /// <param name="options">The options</param>
    /// <returns>Whether the initialization has been successful</returns>
    public static bool InitRaToMame(RaToMameOptions options)
    {
        var err = false;

        // check that input folder exists
        if (!Directory.Exists(options.SourceConfigs))
        {
            Interaction.Log($"Unable to find directory {options.SourceConfigs}");
            err = true;
        }

        if (!Directory.Exists(options.SourceRoms))
        {
            Interaction.Log($"Unable to find directory {options.SourceRoms}");
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
            Interaction.Log($"Unable to find LAY template {options.Template}");
            err = true;
        }

        return !err;
    }
}
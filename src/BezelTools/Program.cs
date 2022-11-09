using BezelTools.Options;
using CommandLine;
using System;

namespace BezelTools;

/// <summary>
/// Program
/// </summary>
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
                       Initializer.InitCommon(o);
                       Initializer.InitMameToRa(o);

                       Converter.ConvertMameToRetroarch(o);
                   })
                   .WithParsed<RaToMameOptions>((o) =>
                   {
                       Initializer.InitCommon(o);
                       Initializer.InitRaToMame(o);

                       Converter.ConvertRetroarchToMame(o);
                   })
                   .WithParsed<CheckOptions>((o) =>
                   {
                       Initializer.InitCommon(o);
                       Initializer.InitCheck(o);

                       Checker.Check(o);
                   })
                   .WithParsed<GenerateOptions>((o) =>
                   {
                       Initializer.InitCommon(o);
                       Initializer.InitGenerate(o);

                       Generator.Generate(o);
                   });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error has occurred: {ex.Message}\n\n{ex.StackTrace}");
        }
    }
}
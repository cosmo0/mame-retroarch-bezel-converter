﻿using BezelTools.Options;
using CommandLine;
using System;

namespace BezelTools;

/// <summary>
/// Program
/// </summary>
public static class Program
{
    /// <summary>
    /// Main application entry point
    /// </summary>
    /// <param name="args">The command line arguments</param>
    public static void Main(string[] args)
    {
        Interaction.Log = (message) => { Console.WriteLine(message); };

        Interaction.Ask = (question, answers) =>
        {
            Console.WriteLine("Please choose which bezel you want");
            foreach (var a in answers)
            {
                Console.WriteLine(a);
            }

            string chosenAnswer = "x";
            int answerIndex;
            while (!int.TryParse(chosenAnswer, out answerIndex))
            {
                chosenAnswer = Console.ReadLine();
            }

            return answerIndex;
        };

        try
        {
            Parser.Default.ParseArguments<MameToRaOptions, RaToMameOptions, CheckOptions, GenerateOptions>(args)
                   .WithParsed<MameToRaOptions>((o) =>
                   {
                       Initializer.InitCommon(o);
                       if (!Initializer.InitMameToRa(o)) { return; }

                       Converter.ConvertMameToRetroarch(o);
                   })
                   .WithParsed<RaToMameOptions>((o) =>
                   {
                       Initializer.InitCommon(o);
                       if (!Initializer.InitRaToMame(o)) { return; }

                       Converter.ConvertRetroarchToMame(o);
                   })
                   .WithParsed<CheckOptions>((o) =>
                   {
                       Initializer.InitCommon(o);
                       if (!Initializer.InitCheck(o)) { return; }

                       Checker.Check(o);
                   })
                   .WithParsed<GenerateOptions>((o) =>
                   {
                       Initializer.InitCommon(o);
                       if (!Initializer.InitGenerate(o)) { return; }

                       Generator.Generate(o);
                   });
        }
        catch (Exception ex)
        {
            Interaction.Log($"An error has occurred: {ex.Message}\n\n{ex.StackTrace}");
        }
    }
}
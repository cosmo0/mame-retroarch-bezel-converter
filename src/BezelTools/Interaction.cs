namespace BezelTools;

/// <summary>
/// Interaction methods for console or GUI
/// </summary>
public static class Interaction
{
    /// <summary>
    /// Asks a question to the user
    /// </summary>
    /// <param name="question">The question to ask.</param>
    /// <param name="answers">The possible answers.</param>
    /// <returns>
    /// The index of the picked answer
    /// </returns>
    public delegate int AskDelegate(string question, string[] answers);

    /// <summary>
    /// Logs a message to the user
    /// </summary>
    /// <param name="message">The message.</param>
    public delegate void LogDelegate(string message);

    /// <summary>
    /// Gets or sets a method to ask something to the user
    /// </summary>
    public static AskDelegate Ask { get; set; }

    /// <summary>
    /// Gets or sets a method to log a message to the user
    /// </summary>
    public static LogDelegate Log { get; set; }
}
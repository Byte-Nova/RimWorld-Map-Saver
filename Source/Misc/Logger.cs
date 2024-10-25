using System;
using Verse;

public static class Logger
{
    public enum LogType { Message, Warning, Error }

    public enum LogImportance { Normal, Verbose }

    public static void Message(string message, LogImportance importance = LogImportance.Normal) { WriteToConsole(message, LogType.Message, importance); }

    public static void Warning(string message, LogImportance importance = LogImportance.Normal) { WriteToConsole(message, LogType.Warning, importance); }

    public static void Error(string message, LogImportance importance = LogImportance.Normal) { WriteToConsole(message, LogType.Error, importance); }

    private static void WriteToConsole(string text, LogType mode, LogImportance importance)
    {
        if (CheckIfShouldPrint(importance))
        {
            string toWrite = $"{Master.modName} > {text}";

            switch(mode)
            {
                case LogType.Message:
                    Log.Message(toWrite);
                    break;

                case LogType.Warning:
                    Log.Warning(toWrite);
                    break;

                case LogType.Error:
                    Log.Error(toWrite);
                    break;

                default:
                    throw new Exception($"{Master.modName} > Logger was passed invalid arguments");
            }
        }
    }

    private static bool CheckIfShouldPrint(LogImportance importance)
    {
        if (importance == LogImportance.Normal) return true;
        else if (importance == LogImportance.Verbose && ModWindow.modConfigs.verboseBool) return true;
        else return false;
    }
}
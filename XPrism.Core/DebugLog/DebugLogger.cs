using System.Diagnostics;

namespace XPrism.Core.DebugLog;

public static class DebugLogger {
    public static void LogInfo(string message) {
        Debug.WriteLine($"{DateTime.Now}-INFO :" + message);
    }

    public static void LogWarning(string message) {
        Console.ForegroundColor = ConsoleColor.Red;
        Debug.WriteLine($"{DateTime.Now}-Warning :" + message);
        Console.ResetColor();
    }

    public static void LogError(string message) {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Debug.WriteLine($"{DateTime.Now}-Error :" + message);
        Console.ResetColor();
    }
}
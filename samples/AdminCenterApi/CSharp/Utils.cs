using System;
using System.Text.Json;

class Utils
{
    internal static void ConsoleWriteLineAsJson(object obj)
    {
        string serializedObject = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(serializedObject);
    }
}
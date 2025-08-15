using System.Text.Json;

namespace SimpleAi.File;

public static class Settings
{
    public static double LearningRate = 0.1f;

    public static string FolderNetwork = "F:\\Technik\\Repos\\SimpleAi\\ExampleToSetupNetwork\\Network-XOR\\";//AppContext.BaseDirectory + @"\Network\";

    public const string FileNetworkNeurons = "Neurons.json";
    
    public const string FileNetworkWeights = "Weights.json";

    public static JsonSerializerOptions FilePrintOptions
    {
        get
        { 
            JsonSerializerOptions printOptions = new JsonSerializerOptions();
            printOptions.WriteIndented = true;
            return printOptions;
        }
    }

    public static string FileData = "Data.json";
    
    public static string FileTarget = "Target.json";
    
    public static string FileResult = "Result.json";
    

    public static void ConsoleInputSettings()
    {
        Console.WriteLine("-Settings-");
        Console.Write("Learning Rate: ");
        LearningRate = double.Parse(Console.ReadLine());
        Console.Write($"Folder Network: ");
        FolderNetwork = Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("Press any key to start...");
        Console.WriteLine("-Started-");
        Console.ReadKey();
    }
}
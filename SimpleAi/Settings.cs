namespace SimpleAi;

public static class Settings
{
    public static double LearningRate = 0.1f;
    /// <summary>
    /// A round is a single intervention through the process from a data set.
    /// </summary>
    public static int Rounds = 80;

    public static string FolderNetwork = AppContext.BaseDirectory + @"\Network\";

    public const string FileNetworkNeurons = "Neurons.json";
    
    public const string FileNetworkWeights = "Weights.json";
    
    public static string FileData = "Data.json";
    
    public static string FileTarget = "Target.json";
    
    public static string FileResult = "Result.json";
    

    public static void ConsoleInputSettings()
    {
        Console.WriteLine("-Settings-");
        Console.Write("Learning Rate: ");
        LearningRate = double.Parse(Console.ReadLine());
        Console.Write("Rounds: ");
        Rounds = int.Parse(Console.ReadLine());
        Console.Write($"Folder Network: ");
        FolderNetwork = Console.ReadLine();

        Console.WriteLine();
        Console.WriteLine("Press any key to start...");
        Console.WriteLine("-Started-");
        Console.ReadKey();
    }
}
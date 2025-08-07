namespace SimpleAi;

public static class Settings
{
    public static double LearingRate = 0.1f;
    /// <summary>
    /// A round is a single intervention through the process from a data set.
    /// </summary>
    public static int Rounds = 80;

    public static string FolderNetwork = AppContext.BaseDirectory + @"\Networks\";
    /// <summary>
    /// This is the main file if the for "UseSameFile" is true. It must be a ".json" file. The FileHandler can open or create this file.
    /// </summary>
    public static string FileCurrentNetworkLoad = "Test.json";

    public static string FileCurrentNetworkSave = "";

    public static void ConsoleInputSettings()
    {
        Console.WriteLine("-Settings-");
        Console.Write("Learning Rate: ");
        LearingRate = double.Parse(Console.ReadLine());
        Console.Write("Rounds: ");
        Rounds = int.Parse(Console.ReadLine());
        Console.Write($"Folder Network: {AppContext.BaseDirectory} + ");
        FolderNetwork = Console.ReadLine();
        Console.Write("File Current Network Load: ");
        FileCurrentNetworkLoad = Console.ReadLine();
        Console.Write("Use Same File (y/N): ");
        if (!(Console.ReadKey().KeyChar == 'y' || Console.ReadKey().KeyChar == 'Y'))
        {
            Console.Write("File Current Network Save: ");
            FileCurrentNetworkSave = Console.ReadLine();
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to start...");
        Console.ReadKey();
    }
}
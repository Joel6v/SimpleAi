using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAi
{
    public static class Settings
    {
        public static float MaxSizeBias = 1.0f;
        public static float RangeForWeightsGenerate = 1.2f;
        public static float LearingRateStart = 1f;
        public static float LearingRateEnd = 0.1f;
        public static float LearingRateDecreaseExponent = 1f;
        /// <summary>
        /// A round is a single intervention through the process from a data set.
        /// </summary>
        public static int Rounds = 80;

        public static string FolderNetwork = AppContext.BaseDirectory + "\\Networks\\";
        /// <summary>
        /// This is the main file, if the for "UseSameFile" is true. It must be a ".json" file. The FileHandler can open od create this file.
        /// </summary>
        public static string FileCurrentNetworkLoad = "Test.json";
        public static string FileCurrentNetworkSave = "";
        
        public static void ConsoleInputSettings()
        {
            Console.WriteLine("-Settings-");
            Console.Write("Max Size Bias: ");
            MaxSizeBias = float.Parse(Console.ReadLine());
            Console.Write("RangeForWeightsGenerate: ");
            RangeForWeightsGenerate = float.Parse(Console.ReadLine());
            Console.Write("LearningRateStart: ");
            LearingRateStart = float.Parse(Console.ReadLine());
            Console.Write("LearningRateEnd: ");
            LearingRateEnd = float.Parse(Console.ReadLine());
            Console.Write("LearningRateDecreaseExponent: ");
            LearingRateDecreaseExponent = float.Parse(Console.ReadLine());
            Console.Write("Rounds: ");
            Rounds = int.Parse(Console.ReadLine());
            Console.Write($"FolderNetwork: {AppContext.BaseDirectory} + ");
            FolderNetwork = Console.ReadLine();
            Console.Write("FielCurrentNetworkLoad: ");
            FileCurrentNetworkLoad = Console.ReadLine();
            Console.Write("UseSameFile (y/N): ");
            if (!(Console.ReadKey().KeyChar == 'y' || Console.ReadKey().KeyChar == 'Y'))
            {
                Console.Write("FileCurrentNetworkSave: ");
                FileCurrentNetworkSave = Console.ReadLine();
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to start...");
            Console.ReadKey();
        }
    }
}

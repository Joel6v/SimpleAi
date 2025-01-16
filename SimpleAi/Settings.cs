using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAi
{
    class Settings
    {
        public const float MaxSizeBias = 1.0f;
        public const float RangeForWeightsGenerate = 1.2f;
        public const float LearingRateStart = 1f;
        public const float LearingRateEnd = 0.1f;
        /// <summary>
        /// A round is a single intervention through the process from a data set.
        /// </summary>
        public const int Rounds = 24;

        public static string FolderNetwork = AppContext.BaseDirectory + "\\Networks\\";
        /// <summary>
        /// This is the main file, if the for "UseSameFile" is true. It must be a ".json" file. The FileHandler can open od create this file.
        /// </summary>
        public const string FileCurrentNetworkLoad = "Test.json";
        public const bool UseSameFile = true;
        public const string FileCurrentNetworkSave = "";
    }
}

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
        public const float LearingRateStart = 0.1f;
        public const float LearingRateEnd = 0.01f;
        /// <summary>
        /// A round is a single intervention through the process from a data set.
        /// </summary>
        public const int Rounds = 4;

        public const string FolderNetwork = "";
        public const string FileCurrentNetworkLoad = ""; //The master file name
        public const bool UseSameFile = true;
        public const string FileCurrentNetworkSave = "";
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;

namespace SimpleAi
{
    internal class FileHandler
    {
        public static void SaveNetwork(string path, List<List<Neuron>> network)
        {
            string json = JsonSerializer.Serialize(network);
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                }
            }
        }

        public static List<List<Neuron>> LoadNetwork(string path)
        {
            string json;
            using(FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using(StreamReader sr = new StreamReader(fs))
                {
                    json = sr.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("The json object is empty");
            }

            return JsonSerializer.Deserialize<List<List<Neuron>>>(json);
        }
    }
}

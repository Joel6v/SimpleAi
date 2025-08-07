using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Reflection.Emit;
using System.Xml.Linq;

namespace SimpleAi
{
    internal static class FileHandler
    {
        public static void SaveNetwork(List<Neuron> network)
        {
            string path = (Settings.FileCurrentNetworkSave == "") ? Settings.FolderNetwork + Settings.FileCurrentNetworkLoad : Settings.FolderNetwork + Settings.FileCurrentNetworkSave;

            string json = JsonSerializer.Serialize(network);
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                }
            }
        }

        public static List<Neuron> LoadNetwork()
        {
            string path = Settings.FolderNetwork + Settings.FileCurrentNetworkLoad;
            string json;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    json = sr.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("The json file is empty");
            }

            return JsonSerializer.Deserialize<List<Neuron>>(json);
        }

        public static List<List<Neuron>> InputConsoleNetwork()
        {
            List<List<Neuron>> network = new();
            while (true) 
            {
                Console.WriteLine("Enter Network Architecture");
                Console.WriteLine();

                Console.Write("Layer Amount: ");
                int amountLayer;
                if (!int.TryParse(Console.ReadLine(), out amountLayer)) 
                {
                    Console.WriteLine("Error: Input in wrong format");
                    continue;
                }

                for (int currentLayer = 0; currentLayer < amountLayer; currentLayer++)
                {
                    Console.Write($"Neurons Amount in Layer {currentLayer}: ");
                    int amountNeurons;
                    if (!int.TryParse(Console.ReadLine(), out amountNeurons))
                    {
                        Console.WriteLine("Error: Input in wrong format");
                        currentLayer--;
                        continue;
                    }

                    List<Neuron> neuronsListLayer = new();
                    Neuron neuronSingle;
                    for(int i = 0; i < amountNeurons; i++)
                    {
                        neuronSingle = new Neuron(currentLayer); //This must be splitted up otherwise the single same object will be saved and share its values
                        neuronsListLayer.Add(neuronSingle);
                    }

                    network.Add(neuronsListLayer);
                }

                for (int currentLayer = 1; currentLayer < amountLayer; currentLayer++)
                {
                    Console.WriteLine($"Weight from current layer {currentLayer} to layer before {currentLayer - 1}");
                    Console.WriteLine("Enter in this format: 0;1;2;3");
                    Console.WriteLine("Or to connect all enter: a");

                    for (int currentNeuron = 0; currentNeuron < network[currentLayer].Count; currentNeuron++)
                    {
                        Console.WriteLine($"Weights from neuron index {currentNeuron}");
                        Console.Write("Input: ");
                        string input = Console.ReadLine();

                        if (string.IsNullOrEmpty(input))
                        {
                            Console.WriteLine("Error: Input in wrong format");
                            currentNeuron--;
                            continue;
                        }else if(input.ToLower() == "a")
                        {
                            for(int i = 0; i < network[currentLayer - 1].Count; i++)
                            {
                                network[currentLayer][currentNeuron].Weights.Add(new Weight(network[currentLayer - 1][i]));
                            }
                        }
                        else
                        {
                            string[] inputSplited = input.Split(';');
                            try
                            {
                                for (int i = 0; i < inputSplited.Count(); i++)
                                {
                                    int toNeuron = Convert.ToInt32(inputSplited[i]);
                                    network[currentLayer][currentNeuron].Weights.Add(new Weight(network[currentLayer - 1][toNeuron]));
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                                currentNeuron--;
                                continue;
                            }
                        }
                    }
                }

                break;
            }

            return network;
        }
    }

    internal class JsonElementNeuron
    {
        public string Id { get; set; }
        
        public int IdJson { get; set; }
        
        public float Value { get; set; }

        public float Bias { get; set; }
        
        public int Layer { get; set; }

        public JsonElementNeuron(List<List<Neuron>> network, int neuronIndex)
        {
            
        }
    }
    
    internal class JsonElementWeight
    {
        public int IdJsonSource { get; set; }
        
        public int IdNeuronBefore { get; set; }
        
        public float ValueWeight { get; set; }

        public JsonElementWeight(List<List<Neuron>> network, int neuronIndex)
        {
            
        }
    }
}

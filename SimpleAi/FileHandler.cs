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
        public static void SaveNetwork(Network network)
        {
            List<Neuron> neurons = network.Create2dList();

            List<JsonNeuron> jsonNeurons = new();
            List<JsonWeight> jsonWeights = new();
            for (int i = 0; i < neurons.Count; i++)
            {
                List<JsonWeight> weightsBefore = new();
                List<JsonWeight> weightsAfter = new();
                for (int j = 0; j < neurons[i].WeightsBefore.Count; j++)
                {
                    weightsBefore.Add(new JsonWeight(neurons[i].WeightsBefore[j]));
                }
                for (int j = 0; j < neurons[i].WeightsAfter.Count; j++)
                {
                    weightsAfter.Add(new JsonWeight(neurons[i].WeightsAfter[j]));
                }
                jsonNeurons.Add(new JsonNeuron(neurons[i], weightsBefore, weightsAfter));
                jsonWeights.AddRange(weightsBefore);
            }

            string jsonStringNeurons = JsonSerializer.Serialize(jsonNeurons);
            using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkNeurons, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(jsonStringNeurons);
                }
            }
            
            string jsonStringWeights = JsonSerializer.Serialize(jsonWeights);
            using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkWeights, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(jsonStringWeights);
                }
            }
        }

        public static List<Neuron> LoadNetwork()
        {
            string jsonStringNeurons;
            using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkNeurons, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    jsonStringNeurons = sr.ReadToEnd();
                }
            }
            
            string jsonStringWeights;
            using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkWeights, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    jsonStringWeights = sr.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(jsonStringNeurons) || string.IsNullOrEmpty(jsonStringWeights))
            {
                string errorMessage = "Json files are empty";
                Console.WriteLine(errorMessage);
                throw new Exception(errorMessage);
            }
            
            List<JsonNeuron> jsonNeurons = JsonSerializer.Deserialize<List<JsonNeuron>>(jsonStringNeurons);
            
            List<JsonWeight> jsonWeightsOriginal = JsonSerializer.Deserialize<List<JsonWeight>>(jsonStringWeights);
            List<JsonWeight> jsonWeightsBefore = new();
            jsonWeightsBefore.AddRange(jsonWeightsOriginal);
            List<JsonWeight> jsonWeightsAfter = new();
            jsonWeightsAfter.AddRange(jsonWeightsOriginal);

            List<Neuron> neurons = new();
            Dictionary<JsonWeight, Neuron> jsonWeightsBeforeDictionary = new();
            Dictionary<JsonWeight, Neuron> jsonWeightsAfterDictionary = new();
            for (int i = 0; i < jsonNeurons.Count; i++)
            {
                neurons.Add(jsonNeurons[i].CreateNeuron());
                List<Weight> weightsBefore = new();
                List<Weight> weightsAfter = new();

                for (int j = 0; j < jsonNeurons[i].IdsWeightsBefore.Count; j++)
                {
                    for (int k = 0; k < jsonWeightsBefore.Count; k++)
                    {
                        if (jsonWeightsBefore[k].IdJson == jsonNeurons[i].IdsWeightsBefore[j])
                        {
                            if (jsonWeightsAfterDictionary.ContainsKey(jsonWeightsBefore[k]))
                            {
                                weightsBefore.Add(jsonWeightsBefore[k].CreateWeight(jsonWeightsAfterDictionary[jsonWeightsBefore[k]], neurons[i]));
                                jsonWeightsAfterDictionary.Remove(jsonWeightsBefore[k]);
                            }
                            else
                            {
                                jsonWeightsBeforeDictionary.Add(jsonWeightsBefore[k], neurons[i]);
                            }
                            jsonWeightsBefore.RemoveAt(k);
                            k--;
                        }
                    }
                }

                for (int j = 0; j < jsonNeurons[i].IdsWeightsAfter.Count; j++)
                {
                    for (int k = 0; k < jsonWeightsAfter.Count; k++)
                    {
                        if (jsonWeightsAfter[k].IdJson == jsonNeurons[i].IdsWeightsAfter[j])
                        {
                            jsonWeightsAfterDictionary.Add(jsonWeightsAfter[k], neurons[i]);
                            jsonWeightsAfter.RemoveAt(k);
                            k--;
                        }
                        
                        if (jsonWeightsAfter[k].IdJson == jsonNeurons[i].IdsWeightsAfter[j])
                        {
                            if (jsonWeightsBeforeDictionary.ContainsKey(jsonWeightsAfter[k]))
                            {
                                weightsAfter.Add(jsonWeightsAfter[k].CreateWeight(jsonWeightsBeforeDictionary[jsonWeightsAfter[k]], neurons[i]));
                                jsonWeightsBeforeDictionary.Remove(jsonWeightsBefore[k]);
                            }
                            else
                            {
                                jsonWeightsAfterDictionary.Add(jsonWeightsAfter[k], neurons[i]);
                            }
                            jsonWeightsAfter.RemoveAt(k);
                            k--;
                        }
                    }
                }
                neurons[^1].WeightsBefore = weightsBefore;
                neurons[^1].WeightsAfter = weightsAfter;
            }
            
            Dictionary<Neuron, JsonWeight> jsonWeightsBeforeDictionaryRe = new();
            Dictionary<Neuron, JsonWeight> jsonWeightsAfterDictionaryRe = new();
            for (int i = 0; i < jsonWeightsOriginal.Count; i++)
            {
                if (jsonWeightsBeforeDictionary.ContainsKey(jsonWeightsOriginal[i]))
                {
                    jsonWeightsBeforeDictionaryRe.Add(jsonWeightsBeforeDictionary[jsonWeightsOriginal[i]], jsonWeightsOriginal[i]);
                    jsonWeightsBeforeDictionary.Remove(jsonWeightsOriginal[i]);
                }else if(jsonWeightsAfterDictionary.ContainsKey(jsonWeightsOriginal[i]))
                {
                    jsonWeightsAfterDictionaryRe.Add(jsonWeightsAfterDictionary[jsonWeightsOriginal[i]], jsonWeightsOriginal[i]);
                    jsonWeightsAfterDictionary.Remove(jsonWeightsOriginal[i]);
                }
            }

            for (int i = 0; i < neurons.Count; i++)
            {
                if (jsonWeightsBeforeDictionaryRe.ContainsKey(neurons[i]))
                {
                    for (int j = 0; j < neurons[i].WeightsBefore.Count; j++)
                    {
                        if (jsonWeightsAfterDictionaryRe.ContainsKey(neurons[j]) && jsonWeightsAfterDictionaryRe[neurons[j]].IdNeuronBefore == neurons[i].Id)
                        {
                            Weight newWeight = jsonWeightsBeforeDictionaryRe[neurons[i]].CreateWeight(neurons[i], neurons[j]);
                            neurons[i].WeightsBefore.Add(newWeight);
                            neurons[j].WeightsAfter.Add(newWeight);
                            jsonWeightsBeforeDictionaryRe.Remove(neurons[i]);
                            jsonWeightsAfterDictionaryRe.Remove(neurons[j]);
                        }
                    }

                }
            }
            
            return neurons;
        }

        public static List<Neuron> InputConsoleNetwork()
        {
            List<Neuron> neurons = new();
            while (true) 
            {
                Console.WriteLine("Enter Network Architecture");
                Console.WriteLine();

                Console.Write("Layer Amount (zero based): ");
                if (!int.TryParse(Console.ReadLine(), out int amountLayer)) 
                {
                    Console.WriteLine("Error: Input in wrong format");
                    continue;
                }

                for (int currentLayer = 0; currentLayer <= amountLayer; currentLayer++)
                {
                    Console.Write($"Neurons Amount in Layer {currentLayer}: ");
                    int amountNeurons;
                    if (!int.TryParse(Console.ReadLine(), out amountNeurons))
                    {
                        Console.WriteLine("Error: Input in wrong format");
                        currentLayer--;
                        continue;
                    }
                    
                    Neuron neuronSingle;
                    for(int i = 0; i < amountNeurons; i++)
                    {
                        neuronSingle = new Neuron(); //This must be split up otherwise the single same object will be saved and share its values
                        neurons.Add(neuronSingle);
                        neurons[^1].Layer = currentLayer;
                    }
                }

                for (int currentLayer = 0; currentLayer <= amountLayer -1; currentLayer++)
                {
                    Console.WriteLine($"Weight from current layer {currentLayer} to layer after {currentLayer + 1}");
                    Console.WriteLine("Enter in this format: 0;1;2;3");
                    Console.WriteLine("Or to connect all enter: a");

                    for (int currentNeuron = 0; currentNeuron < neurons.Count; currentNeuron++)
                    {
                        if(neurons[currentNeuron].Layer != currentLayer) continue;
                        Console.WriteLine($"Weights from neuron index {currentNeuron}:{neurons[currentNeuron].Id}");
                        Console.Write("Input: ");
                        string input = Console.ReadLine();

                        if (string.IsNullOrEmpty(input))
                        {
                            Console.WriteLine("Error: Input in wrong format");
                            currentNeuron--;
                            continue;
                        }else if(input.ToLower() == "a")
                        {
                            for(int i = 0; i < neurons.Count; i++)
                            {
                                neurons[currentNeuron].Weights.Add(new Weight(network[currentLayer - 1][i]));
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

    internal class JsonNeuron
    {
        public string Id { get; set; }

        public double Bias { get; set; }
        
        public int Layer { get; set; }
        
        public List<string> IdsWeightsBefore { get; set; }
        
        public List<string> IdsWeightsAfter { get; set; }

        public JsonNeuron(Neuron neuron, List<JsonWeight> weightsBefore, List<JsonWeight> weightsAfter)
        {
            Id = neuron.Id;
            Bias = neuron.Bias;
            Layer = neuron.Layer;
            
            IdsWeightsBefore = new List<string>();
            for (int i = 0; i < weightsBefore.Count; i++)
            {
                IdsWeightsBefore.Add(weightsBefore[i].IdJson);
            }

            IdsWeightsAfter = new List<string>();
            for (int i = 0; i < weightsAfter.Count; i++)
            {
                IdsWeightsAfter.Add(weightsAfter[i].IdJson);
            }
        }

        public Neuron CreateNeuron()
        {
            return new Neuron(Id, Bias); //Weights must be set later
        }
    }
    
    internal class JsonWeight
    {
        public string IdJson { get; } = Guid.NewGuid().ToString();
        
        public string IdNeuronBefore { get; private set; }
        
        public string IdNeuronAfter { get; private set; }
        
        public double ValueWeight { get; private set; }

        public JsonWeight(Weight weight)
        {
            ValueWeight = weight.Value;
            IdNeuronBefore = weight.NeuronBefore.Id;
            IdNeuronAfter = weight.NeuronAfter.Id;
        }

        public Weight CreateWeight(Neuron neuronBefore, Neuron neuronAfter)
        {
            return new Weight(ValueWeight, neuronBefore, neuronAfter);
        }
    }
}

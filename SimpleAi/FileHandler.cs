using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.Json;
using System.Reflection.Emit;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace SimpleAi.File;

public static class FileHandler
{
    public static void SaveNetwork(Network network)
    {
        List<Neuron> neurons = network.Create2dList();

        List<JsonNeuron> jsonNeurons = new();
        List<JsonWeight> jsonWeights = new();
        Dictionary<Weight, JsonWeight> jsonWeightsBeforeDictionary = new();
        Dictionary<Weight, JsonWeight> jsonWeightsAfterDictionary = new();
        for (int i = 0; i < neurons.Count; i++)
        {
            List<JsonWeight> weightsBefore = new();
            List<JsonWeight> weightsAfter = new();
            
            for (int j = 0; j < neurons[i].WeightsBefore.Count; j++)
            {
                if (jsonWeightsAfterDictionary.ContainsKey(neurons[i].WeightsBefore[j]))
                {
                    weightsBefore.Add(jsonWeightsAfterDictionary[neurons[i].WeightsBefore[j]]);
                    jsonWeightsAfterDictionary.Remove(neurons[i].WeightsBefore[j]); //Remove from the dictionary because it is already in the list
                }
                else
                {
                    JsonWeight jsonWeight = new(neurons[i].WeightsBefore[j]);
                    jsonWeightsBeforeDictionary.Add(neurons[i].WeightsBefore[j], jsonWeight);
                    weightsBefore.Add(jsonWeight);
                }
            }
            
            for (int j = 0; j < neurons[i].WeightsAfter.Count; j++)
            {
                if (jsonWeightsBeforeDictionary.ContainsKey(neurons[i].WeightsAfter[j]))
                {
                    weightsAfter.Add(jsonWeightsBeforeDictionary[neurons[i].WeightsAfter[j]]);
                    jsonWeightsBeforeDictionary.Remove(neurons[i].WeightsAfter[j]);
                }
                else
                {
                    JsonWeight jsonWeight = new(neurons[i].WeightsAfter[j]);
                    jsonWeightsAfterDictionary.Add(neurons[i].WeightsAfter[j], jsonWeight);
                    weightsAfter.Add(jsonWeight);
                }
            }
            
            jsonNeurons.Add(new JsonNeuron(neurons[i], weightsBefore, weightsAfter));
            jsonWeights.AddRange(weightsAfter);
        }

        string jsonStringNeurons = JsonSerializer.Serialize(jsonNeurons, Settings.FilePrintOptions);
        using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkNeurons,
                   FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(jsonStringNeurons);
            }
        }

        string jsonStringWeights = JsonSerializer.Serialize(jsonWeights, Settings.FilePrintOptions);
        using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkWeights,
                   FileMode.OpenOrCreate, FileAccess.ReadWrite))
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
        using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkNeurons, FileMode.Open,
                   FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                jsonStringNeurons = sr.ReadToEnd();
            }
        }

        string jsonStringWeights;
        using (FileStream fs = new FileStream(Settings.FolderNetwork + Settings.FileNetworkWeights, FileMode.Open,
                   FileAccess.Read))
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
                            weightsBefore.Add(jsonWeightsBefore[k]
                                .CreateWeight(jsonWeightsAfterDictionary[jsonWeightsBefore[k]], neurons[i]));
                            //jsonWeightsAfterDictionary.Remove(jsonWeightsBefore[k]);
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
                        if (jsonWeightsBeforeDictionary.ContainsKey(jsonWeightsAfter[k]))
                        {
                            weightsAfter.Add(jsonWeightsAfter[k]
                                .CreateWeight(jsonWeightsBeforeDictionary[jsonWeightsAfter[k]], neurons[i]));
                            //jsonWeightsBeforeDictionary.Remove(jsonWeightsBefore[k]);
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
                jsonWeightsBeforeDictionaryRe.Add(jsonWeightsBeforeDictionary[jsonWeightsOriginal[i]],
                    jsonWeightsOriginal[i]);
                jsonWeightsBeforeDictionary.Remove(jsonWeightsOriginal[i]);
            }
            else if (jsonWeightsAfterDictionary.ContainsKey(jsonWeightsOriginal[i]))
            {
                jsonWeightsAfterDictionaryRe.Add(jsonWeightsAfterDictionary[jsonWeightsOriginal[i]],
                    jsonWeightsOriginal[i]);
                jsonWeightsAfterDictionary.Remove(jsonWeightsOriginal[i]);
            }
        }

        for (int i = 0; i < neurons.Count; i++)
        {
            if (jsonWeightsAfterDictionaryRe.ContainsKey(neurons[i]))
            {
                for (int j = 0; j < neurons.Count; j++)
                {
                    if (neurons[j].Id == jsonWeightsAfterDictionaryRe[neurons[i]].IdNeuronAfter)
                    {
                        Weight newWeight = jsonWeightsAfterDictionaryRe[neurons[i]]
                            .CreateWeight(neurons[i], neurons[j]);
                        neurons[i].WeightsAfter.Add(newWeight);
                        //neurons[j].WeightsAfter.Add(newWeight);
                        jsonWeightsAfterDictionaryRe.Remove(neurons[i]);
                    }
                }
            }
            else if (jsonWeightsBeforeDictionaryRe.ContainsKey(neurons[i]))
            {
                for (int j = 0; j < neurons.Count; j++)
                {
                    if (neurons[j].Id == jsonWeightsBeforeDictionaryRe[neurons[i]].IdNeuronBefore)
                    {
                        Weight newWeight = jsonWeightsBeforeDictionaryRe[neurons[i]]
                            .CreateWeight(neurons[j], neurons[i]);
                        neurons[i].WeightsBefore.Add(newWeight);
                        //neurons[j].WeightsAfter.Add(newWeight);
                        jsonWeightsBeforeDictionaryRe.Remove(neurons[i]);
                    }
                }
            }
        }

        return neurons;
    }

    public static List<Dictionary<string, double>> LoadDictionary(string path)
    {
        path = Settings.FolderNetwork + path;

        string jsonString;
        using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            using (StreamReader sr = new StreamReader(fs))
            {
                jsonString = sr.ReadToEnd();
            }
        }

        if (string.IsNullOrEmpty(jsonString))
        {
            string errorMessage = "Json file is empty";
            Console.WriteLine(errorMessage);
            throw new Exception(errorMessage);
        }

        return JsonSerializer.Deserialize<List<Dictionary<string, double>>>(jsonString);
    }

    public static void SaveDictionary(List<Dictionary<string, double>> dictionary, string path)
    {
        path = Settings.FolderNetwork + path;

        string jsonString = JsonSerializer.Serialize(dictionary, Settings.FilePrintOptions);
        using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        {
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.Write(jsonString);
            }
        }
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

    [JsonConstructor]
    public JsonNeuron(string id, double bias, int layer, List<string> idsWeightsBefore, List<string> idsWeightsAfter)
    {
        Id = id;
        Bias = bias;
        Layer = layer;
        IdsWeightsBefore = idsWeightsBefore;
        IdsWeightsAfter = idsWeightsAfter;
    }

    public Neuron CreateNeuron()
    {
        return new Neuron(Id, Bias, Layer); //Weights must be set later
    }
}

internal class JsonWeight
{
    public string IdJson { get; private set; }

    public string IdNeuronBefore { get; private set; }

    public string IdNeuronAfter { get; private set; }

    public double ValueWeight { get; private set; }

    public JsonWeight(Weight weight)
    {
        IdJson = weight.NeuronBefore.Id + weight.NeuronAfter.Id + "-" + Guid.NewGuid().ToString();
        ValueWeight = weight.Value;
        IdNeuronBefore = weight.NeuronBefore.Id;
        IdNeuronAfter = weight.NeuronAfter.Id;
    }

    [JsonConstructor]
    public JsonWeight(string idJson, string idNeuronBefore, string idNeuronAfter, double valueWeight)
    {
        IdJson = idJson;
        IdNeuronBefore = idNeuronBefore;
        IdNeuronAfter = idNeuronAfter;
        ValueWeight = valueWeight;
    }

    public Weight CreateWeight(Neuron neuronBefore, Neuron neuronAfter)
    {
        return new Weight(ValueWeight, neuronBefore, neuronAfter);
    }
}
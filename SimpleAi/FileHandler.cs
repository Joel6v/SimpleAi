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
    internal class FileHandler
    {
        public static void SaveNetwork(List<List<Neuron>> network)
        {
            string path = (Settings.UseSameFile) ? Settings.FolderNetwork + Settings.FileCurrentNetworkLoad : Settings.FolderNetwork + Settings.FileCurrentNetworkSave;

            string json = JsonSerializer.Serialize(network);
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
            {
                using (StreamWriter sw = new StreamWriter(fs))
                {
                    sw.Write(json);
                }
            }
        }

        public static List<List<Neuron>> LoadNetwork()
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

            return JsonSerializer.Deserialize<List<List<Neuron>>>(json);
        }

        public static List<List<Neuron>> LoadNetworkXml()
        {
            string path = Settings.FolderNetwork + Settings.FileCurrentNetworkLoad;
            string xml;
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    xml = sr.ReadToEnd();
                }
            }

            if (string.IsNullOrEmpty(xml))
            {
                throw new Exception("The xml file is empty");
            }

            List<XmlElementNeuron> elementsNeuron = new List<XmlElementNeuron>();
            List<XmlElementWeight> elementsWeight = new List<XmlElementWeight>();
            int lenghtElementName = 7;
            int indexFirst2Elements = xml.Substring(xml.IndexOf("mxCell") + lenghtElementName).IndexOf("mxCell") + lenghtElementName;

            elementsNeuron.Add((XmlElementNeuron)ConvertXmlElementToObject(xml, indexFirst2Elements));

            int startIndexLast = 0;
            for (int i = 0; true; i++)
            {
                Object obj = ConvertXmlElementToObject(xml, startIndexLast + 10);
                if (obj is XmlElementNeuron)
                {
                    elementsNeuron.Add((XmlElementNeuron)obj);
                }
                else if(obj is XmlElementWeight) 
                {
                    elementsWeight.Add((XmlElementWeight)obj);
                }
                else //obj is null == true
                {
                    break;
                }
            }

            return null;
        }

        private static List<List<Neuron>> ConvertObjectsListsToNetwork(List<XmlElementNeuron> elementsNeuron, List<XmlElementWeight> elementsWeight)
        {
            List<List<Neuron>> network = new List<List<Neuron>>();

            for (int j = 0; true; j++) 
            {
                for (int i = 0; i < elementsNeuron.Count; i++)
                {
                    if (elementsNeuron[i].Layer == j)
                    {
                        network[j].Add(elementsNeuron[i].ConvertToNeuron());
                    }
                }
            }           
        }

        private static Object ConvertXmlElementToObject(string xml, int startIndex)
        {
            int startIndexElement = xml.Substring(startIndex + 6).IndexOf("mxCell");
            if (startIndexElement != -1) { return null; }

            int startIndexId = xml.Substring(startIndexElement).IndexOf("id=\"") + 4;
            int endIndexId = xml.Substring(startIndexId).IndexOf('"');
            string id = xml.Substring(startIndexId, endIndexId - startIndexId);
            if (xml[endIndexId + 2] == 'v')
            {
                int startIndexLayer = endIndexId + 8;
                int layer = Convert.ToInt32(xml.Substring(startIndexLayer, xml.Substring(startIndexLayer).IndexOf('"') - startIndexLayer));
                
                return new XmlElementNeuron(id, startIndexElement, layer);
            }
            else
            {
                int startIndexSource = xml.Substring(endIndexId).IndexOf("source=\"") + 8;
                int endIndexSource = xml.Substring(startIndexId).IndexOf('"');
                string idSource = xml.Substring(startIndexSource, endIndexSource - startIndexSource);

                int startIndexTarget = endIndexSource + 9;
                string idTarget = xml.Substring(startIndexTarget, xml.Substring(startIndexTarget).IndexOf('"') - startIndexTarget);

                return new XmlElementWeight(idSource, idTarget, startIndexElement);
            }
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

    class XmlElementNeuron
    {
        public string Id { get; set; }

        public int StartIndex { get; set; }

        public int Layer {  get; set; }

        public XmlElementNeuron(string id, int startIndex, int layer)
        {
            Id = id;
            StartIndex = startIndex;
            Layer = layer;
        }

        public Neuron ConvertToNeuron()
        {
            return new Neuron(Layer);
        }
    }

    class XmlElementWeight
    {
        public string IdSource { get; set; }

        public string IdTarget { get; set; }

        public int StartIndex { get; set; }

        public XmlElementWeight(string idSource, string idTarget, int startIndex)
        {
            IdSource = idSource;
            IdTarget = idTarget;
            StartIndex = startIndex;
        }
    }
}

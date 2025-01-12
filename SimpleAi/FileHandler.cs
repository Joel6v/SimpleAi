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

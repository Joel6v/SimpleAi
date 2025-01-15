using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAi
{
    internal class Training
    {
        public static void GenerateData(List<List<Neuron>> network, int round)
        {
            int outputLayerIndex = network.Count - 1;
            //01
            //23
            if (round % 4 == 0)
            {
                //xx
                //00
                network[0][0].Value = 1;
                network[outputLayerIndex][0].CurrentIdealValue = 1f;
                network[0][1].Value = 1;
                network[outputLayerIndex][1].CurrentIdealValue = 0f;
                network[0][2].Value = 0;
                network[outputLayerIndex][2].CurrentIdealValue = 0f;
                network[0][3].Value = 0;
                network[outputLayerIndex][3].CurrentIdealValue = 0f;
            }
            else if (round % 4 == 1)
            {
                //00
                //xx
                network[0][0].Value = 0;
                network[outputLayerIndex][0].CurrentIdealValue = 0;
                network[0][1].Value = 0;
                network[outputLayerIndex][1].CurrentIdealValue = 1;
                network[0][2].Value = 1;
                network[outputLayerIndex][2].CurrentIdealValue = 0;
                network[0][3].Value = 1;
                network[outputLayerIndex][3].CurrentIdealValue = 0;
            }
            else if (round % 4 == 2)
            {
                //x0
                //x0
                network[0][0].Value = 1;
                network[outputLayerIndex][0].CurrentIdealValue = 0;
                network[0][1].Value = 0;
                network[outputLayerIndex][1].CurrentIdealValue = 0;
                network[0][2].Value = 1;
                network[outputLayerIndex][2].CurrentIdealValue = 1;
                network[0][3].Value = 0;
                network[outputLayerIndex][3].CurrentIdealValue = 0;
            }
            else
            {
                //x0
                //0x
                network[0][0].Value = 1;
                network[outputLayerIndex][0].CurrentIdealValue = 0;
                network[0][1].Value = 0;
                network[outputLayerIndex][1].CurrentIdealValue = 0;
                network[0][2].Value = 0;
                network[outputLayerIndex][2].CurrentIdealValue = 0;
                network[0][3].Value = 1;
                network[outputLayerIndex][3].CurrentIdealValue = 1;
            }
        }

        public static void Backpropagation(List<List<Neuron>> network, float learingRateCurrent)
        {
            for (int layer = network.Count - 1; layer >= 1; layer--)
            {
                for (int neuron = 0; neuron < network[layer].Count; neuron++)
                {
                    network[layer][neuron].CalculateChangeWeight(learingRateCurrent);
                }
            }
        }

        public static void OutputResult(List<List<Neuron>> network, int round)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Round: " + (round + 1));
            Console.ResetColor();
            int outputLayerIndex = network.Count - 1;
            for (int i = 0; i < network[outputLayerIndex].Count; i++)
            {
                Console.WriteLine("Neuron Number: " + i);
                Console.WriteLine("Neuron Ideal Value: " + network[outputLayerIndex][i].CurrentIdealValue);
                Console.WriteLine("Neuron Is Value: " + network[outputLayerIndex][i].Value);
                Console.WriteLine();
            }
        }
    }
}

using System.Net.NetworkInformation;

namespace SimpleAi
{
    internal class Program
    {
        static public void Main(string[] args)
        {


            Console.ReadKey();
        }
    }

    class Run
    {
        public static List<List<Neuron>> Network = new();
        private int OutputLayerIndex;

        private void RunManagerWithBackpropagation()
        {
            for (int i = 0; i <= 3; i++)
            {
                List<float> correctOutput = GenerateData(i);
                Forwardpropagation();
                OutputResult(i);
            }
        }

        private List<float> GenerateData(int round)
        {
            List<float> correctOuput;
            Random rnd = new Random();
            int dataSet = rnd.Next(0, 5);

            //01
            //23
            if (round % 4 == 0)
            {
                //xx
                //00
                Network[0][0].Value = 1;
                Network[0][1].Value = 1;
                Network[0][2].Value = 0;
                Network[0][3].Value = 0;
                correctOuput = [1, 0, 0, 0];
            }
            else if (round % 4 == 1)
            {
                //00
                //xx
                Network[0][0].Value = 0;
                Network[0][1].Value = 0;
                Network[0][2].Value = 1;
                Network[0][3].Value = 1;
                correctOuput = [0, 1, 0, 0];
            }
            else if (round % 4 == 2)
            {
                //x0
                //x0
                Network[0][0].Value = 1;
                Network[0][1].Value = 0;
                Network[0][2].Value = 1;
                Network[0][3].Value = 0;
                correctOuput = [0, 0, 1, 0];
            }
            else
            {
                //x0
                //0x
                Network[0][0].Value = 1;
                Network[0][1].Value = 0;
                Network[0][2].Value = 0;
                Network[0][3].Value = 1;
                correctOuput = [0, 0, 0, 1];
            }

            return correctOuput;
        }

        private void Forwardpropagation()
        {
            for (int layer = 1; layer <= OutputLayerIndex; layer++)
            {
                for (int i = 0; i < Network[layer].Count; i++)
                {
                    Network[layer][i].CalculateValue();
                }
            }
        }

        private void OutputResult(int round)
        {
            Console.WriteLine("Round: " + (round - 1));
            for (int i = 0; i < Network[OutputLayerIndex].Count; i++)
            {
                Console.WriteLine("Neuron " + i + " : " + Network[OutputLayerIndex][i].Value);
            }
        }
    }
}
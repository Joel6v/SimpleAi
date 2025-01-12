namespace SimpleAi
{
    internal class Program
    {
        static public void Main(string[] args)
        {
            RunLearing runLearing = new RunLearing();
            runLearing.RunManager();

            Console.ReadKey();
        }
    }

    class RunProduct
    {

    }

    class RunLearing
    {
        private static List<List<Neuron>> Network = new();
        private int OutputLayerIndex;

        public void RunManager()
        {
            Network = FileHandler.LoadNetwork();

            float relationLearningrateStartEnd = (Settings.LearingRateStart - Settings.LearingRateEnd) / Settings.Rounds;
            for (int i = 0; i < Settings.Rounds; i++)
            {
                GenerateData(i);
                Forwardpropagation();
                OutputResult(i);
                Backpropagation(Settings.LearingRateStart - relationLearningrateStartEnd * i);
            }

            FileHandler.SaveNetwork(Network);
        }

        private void GenerateData(int round)
        {
            //01
            //23
            if (round % 4 == 0)
            {
                //xx
                //00
                Network[0][0].Value = 1;
                Network[OutputLayerIndex][0].CurrentIdealValue = 1;
                Network[0][1].Value = 1;
                Network[OutputLayerIndex][1].CurrentIdealValue = 0;
                Network[0][2].Value = 0;
                Network[OutputLayerIndex][2].CurrentIdealValue = 0;
                Network[0][3].Value = 0;
                Network[OutputLayerIndex][3].CurrentIdealValue = 0;
            }
            else if (round % 4 == 1)
            {
                //00
                //xx
                Network[0][0].Value = 0;
                Network[OutputLayerIndex][0].CurrentIdealValue = 0;
                Network[0][1].Value = 0;
                Network[OutputLayerIndex][1].CurrentIdealValue = 1;
                Network[0][2].Value = 1;
                Network[OutputLayerIndex][2].CurrentIdealValue = 0;
                Network[0][3].Value = 1;
                Network[OutputLayerIndex][3].CurrentIdealValue = 0;
            }
            else if (round % 4 == 2)
            {
                //x0
                //x0
                Network[0][0].Value = 1;
                Network[OutputLayerIndex][0].CurrentIdealValue = 0;
                Network[0][1].Value = 0;
                Network[OutputLayerIndex][1].CurrentIdealValue = 0;
                Network[0][2].Value = 1;
                Network[OutputLayerIndex][2].CurrentIdealValue = 1;
                Network[0][3].Value = 0;
                Network[OutputLayerIndex][3].CurrentIdealValue = 0;
            }
            else
            {
                //x0
                //0x
                Network[0][0].Value = 1;
                Network[OutputLayerIndex][0].CurrentIdealValue = 0;
                Network[0][1].Value = 0;
                Network[OutputLayerIndex][1].CurrentIdealValue = 0;
                Network[0][2].Value = 0;
                Network[OutputLayerIndex][2].CurrentIdealValue = 0;
                Network[0][3].Value = 1;
                Network[OutputLayerIndex][3].CurrentIdealValue = 1;
            }
        }

        private void Forwardpropagation()
        {
            for (int layer = 1; layer <= OutputLayerIndex; layer++)
            {
                for (int neuron = 0; neuron < Network[layer].Count; neuron++)
                {
                    Network[layer][neuron].CalculateValue();
                }
            }
        }

        private void OutputResult(int round)
        {
            Console.WriteLine("Round: " + (round + 1));
            for (int i = 0; i < Network[OutputLayerIndex].Count; i++)
            {
                Console.WriteLine("Neuron Number: " + i);
                Console.WriteLine("Neuron Ideal Value: " + Network[OutputLayerIndex][i].CurrentIdealValue);
                Console.WriteLine("Neuron Is Value: " + Network[OutputLayerIndex][i].Value);
                Console.WriteLine();
            }
        }

        private void Backpropagation(float learingRateCurrent)
        {
            for(int layer = OutputLayerIndex; layer >= 1; layer--)
            {
                for(int neuron = 0;  neuron < Network[layer].Count; neuron++)
                {
                    Network[layer][neuron].CalculateChangeWeight(learingRateCurrent);
                }
            }
        }
    }
}
namespace SimpleAi
{
    internal class Program
    {
        static public void Main(string[] args)
        {
            Run run = new Run();
            run.RunTrainingManager();

            Console.ReadKey();
        }
    }

    class Run
    {
        private static List<List<Neuron>> Network = new();

        public void RunTrainingManager()
        {
            //Network = FileHandler.LoadNetwork();
            Network = FileHandler.InputConsoleNetwork();

            float relationLearningrateStartEnd = (Settings.LearingRateStart - Settings.LearingRateEnd) / Settings.Rounds;
            for (int i = 0; i < Settings.Rounds; i++)
            {
                Training.GenerateData(Network, i);
                Forwardpropagation();
                Training.OutputResult(Network ,i);
                Training.Backpropagation(Network ,Settings.LearingRateStart - relationLearningrateStartEnd * i);
            }

            //FileHandler.SaveNetwork(Network);
        }

        private void Forwardpropagation()
        {
            int outputLayerIndex = Network.Count - 1;
            for (int layer = 1; layer <= outputLayerIndex; layer++)
            {
                for (int neuron = 0; neuron < Network[layer].Count; neuron++)
                {
                    Network[layer][neuron].CalculateValue();
                }
            }
        }
    }
}
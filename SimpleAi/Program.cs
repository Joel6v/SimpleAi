using System.Net.NetworkInformation;

namespace SimpleAi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            //Run run = new Run();
            //run.RunTrainingManager();

            Console.ReadKey();
        }
    }

    public static class Run
    {
        public static void RunTrainingManager()
        {
            Dictionary<string, double> data = FileHandler.LoadDictionary(Settings.FileData);
            Dictionary<string, double> target = FileHandler.LoadDictionary(Settings.FileTarget);
            List<Neuron> neurons = FileHandler.LoadNetwork();
            
            Network network = new Network(neurons);
            for (int i = 0; i < data.Count; i++)
            {
                FileHandler.SaveDictionary(network.RunTraining(data, target), Settings.FileResult);
            }
            
            FileHandler.SaveNetwork(network);
        }

        public static void RunManager()
        {
            Dictionary<string, double> data = FileHandler.LoadDictionary(Settings.FileData);
            List<Neuron> neurons = FileHandler.LoadNetwork();
            
            Network network = new Network(neurons);
            for (int i = 0; i < data.Count; i++)
            {
                FileHandler.SaveDictionary(network.Run(data), Settings.FileResult);
            }
        }
    }
}
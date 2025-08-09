using SimpleAi.File;

namespace SimpleAi
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Run.RunTrainingManager();
            
        }
    }

    public static class Run
    {
        public static void RunTrainingManager()
        {
            List<Dictionary<string, double>> data = FileHandler.LoadDictionary(Settings.FileData);
            List<Dictionary<string, double>> target = FileHandler.LoadDictionary(Settings.FileTarget);
            List<Neuron> neurons = FileHandler.LoadNetwork();
            
            Network network = new Network(neurons);
            List<Dictionary<string, double>> result = new();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(network.RunTraining(data[i], target[i]));
            }
            FileHandler.SaveDictionary(result, Settings.FileResult);
            //FileHandler.SaveNetwork(network);
        }

        public static void RunManager()
        {
            List<Dictionary<string, double>> data = FileHandler.LoadDictionary(Settings.FileData);
            List<Neuron> neurons = FileHandler.LoadNetwork();
            
            Network network = new Network(neurons);
            List<Dictionary<string, double>> result = new();
            for (int i = 0; i < data.Count; i++)
            {
                result.Add(network.Run(data[i]));
            }
            FileHandler.SaveDictionary(result, Settings.FileResult);
        }
    }
}
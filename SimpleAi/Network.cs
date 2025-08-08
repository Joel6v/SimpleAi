namespace SimpleAi;

public class Network
{
    private List<List<Neuron>> Neurons { get; set; }
    
    public Network(List<Neuron> neurons)
    {
        Create3dList(neurons);
    }

    public List<Neuron> Create2dList()
    {
        List<Neuron> neurons = new();
        for (int i = 0; i < Neurons.Count; i++)
        {
            neurons.AddRange(Neurons[i]);
        }
        return neurons;
    }

    private void Create3dList(List<Neuron> neurons)
    {
        Neurons = new List<List<Neuron>>();
        for (int i = 0; i < neurons.Count; i++)
        {
            if (Neurons.Count <= neurons[i].Layer)
            {
                for (int j = 0; j < neurons[i].Layer - Neurons.Count - 1; j++)
                {
                    Neurons.Add(new List<Neuron>());
                }
            }
            Neurons[neurons[i].Layer].Add(neurons[i]);
        }
    }

    public Dictionary<string, double> Run(Dictionary<string, double> data)
    {
        RunNetworkForwardProp(data);
        
        Dictionary<string, double> result = new Dictionary<string, double>();
        for (int i = 0; i < Neurons[^1].Count; i++)
        {
            result.Add(Neurons[^1][i].Id, Neurons[^1][i].Value);
        }
        
        return result;
    }

    public Dictionary<string, double> RunTraining(Dictionary<string, double> data, Dictionary<string, double> target)
    {
        RunNetworkForwardProp(data);
        
        List<Neuron> outputNeurons = Neurons[^1];
        
        for (int i = 0; i < target.Count; i++)
        {
            for (int j = 0; j < outputNeurons.Count; j++)
            {
                if (outputNeurons[j].Id.Equals(target.ElementAt(i).Key))
                {
                    outputNeurons[j].Backpropagation(Settings.LearningRate, target.ElementAt(i).Value);
                    outputNeurons.RemoveAt(j);
                    j--;
                }
            }
        }
        
        for (int i = Neurons.Count - 2; i >= 1; i++)
        {
            for (int j = 0; j < Neurons[i].Count; j++)
            {
                Neurons[i][j].Backpropagation(Settings.LearningRate);
            }
        }
        
        Dictionary<string, double> result = new Dictionary<string, double>();
        for (int i = 0; i < Neurons[^1].Count; i++)
        {
            result.Add(Neurons[^1][i].Id, Neurons[^1][i].Value);
        }
        return result;
    }
    
    private void RunNetworkForwardProp(Dictionary<string, double> data)
    {
        List<Neuron> inputNeurons = Neurons[0];
        
        for (int i = 0; i < data.Count; i++) //only the first layer because otherwise it would be overridden
        {
            for (int j = 0; j < inputNeurons.Count; j++)
            {
                if (inputNeurons[j].Id.Equals(data.ElementAt(i).Key))
                {
                    inputNeurons[j].Value = data.ElementAt(i).Value;
                    inputNeurons.RemoveAt(j);
                    j--;
                }
            }
        }
        
        for (int i = 1; i < Neurons.Count; i++)
        {
            for (int j = 0; j < Neurons[i].Count; j++)
            {
                Neurons[i][j].CalculateOutput();
            }
        }
    }

    private void RunNetworkForwardProp(Dictionary<string, double> data, int rounds)
    {
        RunNetworkForwardProp(data);

        for (int i = 1; i < rounds; i++) //Starts at 1 because one round is already done at RunNetworkFor...
        {
            for (int j = 0; j < Neurons.Count; j++)
            {
                for (int k = 0; k < Neurons[j].Count; k++)
                {
                    Neurons[j][k].CalculateOutput();
                }
            }
        }
    }
}
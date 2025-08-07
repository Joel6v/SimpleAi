namespace SimpleAi;

public class Network
{
    private List<List<Neuron>> Neurons { get; set; }
    
    public Network(List<Neuron> neurons)
    {
        Create3dList(neurons);
    }

    private List<Neuron> Create2dList()
    {
        
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

    public Dictionary<string, double> Run(double learningRate, Dictionary<string, double> data, Dictionary<string, double> target)
    {
        RunNetworkForwardProp(data);
        
        List<Neuron> outputNeurons = Neurons[^1];
        
        for (int i = 0; i < target.Count; i++)
        {
            for (int j = 0; j < outputNeurons.Count; j++)
            {
                if (outputNeurons[j].Id.Equals(target.ElementAt(i).Key))
                {
                    outputNeurons[j].Backpropagation(learningRate, target.ElementAt(i).Value);
                    outputNeurons.RemoveAt(j);
                    j--;
                }
            }
        }
        
        for (int i = 0; i < Neurons.Count -1; i++)
        {
            for (int j = 0; j < Neurons[i].Count; j++)
            {
                Neurons[i][j].Backpropagation(learningRate);
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
        
        for (int i = 0; i < Neurons.Count; i++)
        {
            for (int j = 0; j < Neurons[i].Count; j++)
            {
                Neurons[i][j].CalculateOutput();
            }
        }
    }
}
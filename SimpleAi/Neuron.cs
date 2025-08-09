namespace SimpleAi;

public class Neuron
{
    public string Id { get; private set; }

    public double Value { get; set; } = 0;

    public double Error { get; private set; } = 0;
    
    public double Bias { get; private set; }

    public List<Weight> WeightsBefore { get; set; } = new List<Weight>();
    
    public List<Weight> WeightsAfter { get; set; } = new List<Weight>();
    
    public int Layer { get; set; }
    
    public Neuron(string id, double bias, int layer)
    {
        Id = id;
        Bias = bias;
        Layer = layer;
    }

    public Neuron(string id, int layer)
    {
        Id = id;
        Bias = new Random().NextDouble() * 2 - 1;
        Layer = layer;
    }

    public Neuron(int layer)
    {
        Id = Guid.NewGuid().ToString();
        Bias = new Random().NextDouble() * 2 - 1;
        Layer = layer;
    }

    public void SetWeights(List<Weight> weightsBefore, List<Weight> weightsAfter)
    {
        WeightsBefore = weightsBefore;
        WeightsAfter = weightsAfter;
        //CalculateLayer(network);
    }
    
    private void Relu()
    {
        Value = Math.Max(0, Value);
    }
    
    private void Sigmoid() //Is not so fast as the Relu, but it is more accurate.
    {
        Value = 1 / (1 + Math.Exp(-Value));
    }

    public void CalculateOutput()
    {
        Value = Bias;
        for (int i = 0; i < WeightsBefore.Count; i++)
        {
            Value += WeightsBefore[i].Value * WeightsBefore[i].NeuronBefore.Value;
        }
        Sigmoid();
    }

    private void CalculateNewWeightsBias(double learningRate)
    {
        double gradient = Error * learningRate * Value * (1 - Value); //The biggest result at Value = 0.5
        Bias += gradient;
        for (int i = 0; i < WeightsBefore.Count; i++)
        {
            WeightsBefore[i].Value += gradient * WeightsBefore[i].NeuronBefore.Value;
        }
    }

    /// <summary>
    /// Calculates the error of the value with the errors from the layers after (direction forward prop)
    /// </summary>
    public void Backpropagation(double learningRate)
    {
        Error = 0;

        for (int i = 0; i < WeightsAfter.Count; i++)
        {
            Error += WeightsAfter[i].Value * WeightsAfter[i].NeuronAfter.Error;
        }
        
        CalculateNewWeightsBias(learningRate);
    }

    /// <summary>
    /// Calculates the error of the value with the target value
    /// </summary>
    /// <param name="targetValue"></param>
    public void Backpropagation(double learningRate, double targetValue)
    {
        Error = targetValue - Value;
        
        CalculateNewWeightsBias(learningRate);
    }

    public void CalculateLayer(List<Neuron> network)
    {
        int higestLayerNeuronsBefore = 0;
        for (int i = 0; i < WeightsBefore.Count; i++)
        {
            if (WeightsBefore[i].NeuronBefore.Layer > higestLayerNeuronsBefore)
            {
                higestLayerNeuronsBefore = WeightsBefore[i].NeuronBefore.Layer;
            }
        }
        
        int lowestLayerNeuronsAfter = 0;
        for (int i = 0; i < WeightsAfter.Count; i++)
        {
            if (WeightsAfter[i].NeuronAfter.Layer < lowestLayerNeuronsAfter)
            {
                lowestLayerNeuronsAfter = WeightsAfter[i].NeuronAfter.Layer;
            }
        }

        if (higestLayerNeuronsBefore >= lowestLayerNeuronsAfter -1)
        {
            //Makes sure that all neurons will be updated
            int layersToAdd = (lowestLayerNeuronsAfter - 2) - higestLayerNeuronsBefore;
            for (int i = 0; i < network.Count; i++)
            {
                if (network[i].Layer == lowestLayerNeuronsAfter)
                {
                    for (int j = 0; j < network[i].WeightsAfter.Count; j++)
                    {
                        if (network[i].WeightsAfter[j].NeuronAfter == this)
                        {
                            network[i].AddLayer(layersToAdd);
                        }
                    }
                }
            }
        }
        Layer = higestLayerNeuronsBefore + 1;
    }

    public void AddLayer(int layersToAdd)
    {
        for (int i = 0; i < WeightsAfter.Count; i++)
        {
            if (WeightsAfter[i].NeuronAfter.Layer - Layer == 1)
            {
                WeightsAfter[i].NeuronAfter.AddLayer(layersToAdd);
            }
        }
        Layer += layersToAdd;
    }
}

public class Weight
{
    public double Value { get; set; }
    
    /// <summary>
    /// The neurons this Weight is connected to before its source neuron (direction forward prop)
    /// </summary>
    public Neuron NeuronBefore { get; set; }
    
    public Neuron NeuronAfter { get; set; }

    public Weight(double value, Neuron neuronBefore, Neuron neuronAfter)
    {
        Value = value;
        NeuronBefore = neuronBefore;
        NeuronAfter = neuronAfter;
    }
    
    public Weight(Neuron neuronBefore, Neuron neuronAfter)
    {
        Value = new Random().NextDouble() * 2 - 1;
        NeuronBefore = neuronBefore;
        NeuronAfter = neuronAfter;
    }
}
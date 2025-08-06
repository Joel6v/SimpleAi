using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAi
{
    class Neuron
    {
        public float Value { get; set; }

        public float Bias { get; set; }

        public float CurrentIdealValue { get; set; }

        /// <summary>
        /// The number of neurons after this one.
        /// This number is only used in the Backpropagation process.
        /// </summary>
        public int NeuronsAfterCount { get; set; } = 0;

        /// <summary>
        /// This is connection to the neurons before this one, 
        /// to determine the value of this neuron in the Forwardpropagation.
        /// </summary>
        public List<Weight> Weights { get; set; }

        public int Layer { get; private set; }

        public Neuron(float bias, List<Weight> weights)
        {
            Bias = bias;
            Weights = weights;
        }

        public Neuron(List<Weight> weights)
        {
            Bias = 0;
            Weights = weights;
        }

        public Neuron(int layer)
        {
            Layer = layer;
            Weights = new();
        }

        private void Relu()
        {
            Value = Value + Bias;
            if (Value < 0)
            {
                Value = 0;
            }
            else if (Value > 1)
            {
                Value = 1;
            }
        }
        private void Sigmoid() //Is not so fast as the Relu, but it is more accurate.
        {
            Value = 1 / (1 + (float)Math.Exp(-Value));
        }

        /// <summary>
        /// To update the Layer value from the neuron. 
        /// Only use it if this neuron is the only one for which the value needs to be updated, otherwise the calculation will be incorrect.
        /// </summary>
        public void CalculateLayerSingleUpdate()
        {
            if (Weights == null)
            {
                Layer = 0;
            }
            else
            {
                Layer = ((List<Weight>)Weights.OrderBy(weight => weight.NeuronBefore.Layer))[Weights.Count - 1].NeuronBefore.Layer;
            }
        }

        public void CalculateValue()
        {
            Value = 0;
            for (int i = 0; i < Weights.Count; i++)
            {
                Value += Weights[i].ValueWeight * Weights[i].NeuronBefore.Value;
            }
            
            Value += Bias;

            Relu();
        }

        public void CalculateChangeWeight(float learingRateCurrent)
        {
            CurrentIdealValue = (NeuronsAfterCount != 0) ? CurrentIdealValue / NeuronsAfterCount : CurrentIdealValue;
            NeuronsAfterCount = 0;

            float differenceIdealValue = Math.Abs(CurrentIdealValue - Value) * (CurrentIdealValue - Value);
            float avgNeuronBeforeValue = 0;
            for (int i = 0; i < Weights.Count; i++)
            {
                //For the Weights
                Weights[i].ValueWeight += Weights[i].ValueWeight * differenceIdealValue * Weights[i].NeuronBefore.Value * learingRateCurrent;
                
                //For the Bias
                avgNeuronBeforeValue += Weights[i].NeuronBefore.Value;  
                
                //Neuron before this one
                Weights[i].NeuronBefore.CurrentIdealValue += differenceIdealValue * Weights[i].ValueWeight;
                Weights[i].NeuronBefore.NeuronsAfterCount += 1;
            }
            avgNeuronBeforeValue /= Weights.Count;
            Bias += Bias * differenceIdealValue * avgNeuronBeforeValue * learingRateCurrent;
        }
    }

    class Weight
    {
        public float ValueWeight { get; set; }
        /// <summary>
        /// Here is a reference to the neuron before this one.
        /// </summary>
        public Neuron NeuronBefore { get; set; }

        public Weight(float valueWeight, Neuron neuronBefore)
        {
            ValueWeight = valueWeight;
            NeuronBefore = neuronBefore;
        }

        public Weight(Neuron neuronBefore)
        {
            GenerateValue();
            NeuronBefore = neuronBefore;
        }

        private void GenerateValue()
        {
            Random rnd = new Random();
            float halfValue = (float)rnd.NextDouble() * Settings.RangeForWeightsGenerate / 2;
            ValueWeight = (rnd.Next(0, 2) == 0) ? -1*halfValue : halfValue;
        }
    }
}

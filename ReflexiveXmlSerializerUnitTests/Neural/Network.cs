using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNet.Serialization
{
    public class Network
    {
        public string Name { get; set; }
        public List<Neuron> Neurons { get; set; }
        public double? TrueError { get; set; }
        public List<Point> ErrorHistory { get; set; }
        public List<int> HiddensLayout { get; set; }

        public double InitialWeightInterval1 { get; set; }
        public double InitialWeightInterval2 { get; set; }
        public double LearningRate { get; set; }
        public double LearningRateDecay { get; set; }
        public double Momentum { get; set; }
        public double MomentumDecay { get; set; }
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(Network)))
            {
                Network b = (Network)obj;
                CollectionAssert.AreEqual(this.Neurons, b.Neurons);
                CollectionAssert.AreEqual(this.ErrorHistory, b.ErrorHistory);
                CollectionAssert.AreEqual(this.HiddensLayout, b.HiddensLayout);
                return this.Name.Equals(b.Name);
            }
            else
            {
                return false;
            }
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNet.Serialization
{
    public enum NeuronType { Input, Hidden, Output, Constant }

    public class Neuron
    {
        public int Id { get; set; }
        public double ErrorTerm { get; set; }
        public bool Fed { get; set; }
        public NeuronType Type { get; set; }
        public double Value { get; set;}
        public List<WeightedLink> Upsteam {get;set;}
        public List<WeightedLink> Downstream { get; set; }
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(Neuron)))
            {
                Neuron b = (Neuron)obj;
                CollectionAssert.AreEqual(this.Downstream, b.Downstream);
                CollectionAssert.AreEqual(this.Upsteam, b.Upsteam);
                return this.Id.Equals(b.Id) &&
                    this.ErrorTerm.Equals(b.ErrorTerm) &&
                    this.Fed.Equals(b.Fed) &&
                    this.Type.Equals(b.Type) &&
                    this.Value.Equals(b.Value);
            }
            else
            {
                return false;
            }
        }
    }
}

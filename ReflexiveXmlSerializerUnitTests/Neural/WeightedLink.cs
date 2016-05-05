using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNet.Serialization
{
    public class WeightedLink
    {
        public int Child { get; set; }
        public double Weight { get; set; }
        public override bool Equals(object obj)
        {
            if(obj.GetType().Equals(typeof(WeightedLink)))
            {
                WeightedLink b = (WeightedLink)obj;
                return this.Child.Equals(b.Child) && this.Weight.Equals(b.Weight);
            }
            else
            {
                return false;
            }
        }
    }
}

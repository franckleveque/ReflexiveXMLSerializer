using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NeuralNet.Serialization
{
    public class Point
    {
        public double X { get; set; }
        public double Y { get; set; }
        public override bool Equals(object obj)
        {
            if (obj.GetType().Equals(typeof(Point)))
            {
                Point b = (Point)obj;
                return this.X.Equals(b.X) && this.Y.Equals(b.Y);
            }
            else
            {
                return false;
            }
        }
    }
}

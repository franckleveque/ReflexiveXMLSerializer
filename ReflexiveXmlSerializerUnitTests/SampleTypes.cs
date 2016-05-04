using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReflexiveXmlSerializerUnitTests
{
    public class SimpleSample
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class SimpleArraySample
    {
        public int Id { get; set; }
        public string[] Mails { get; set; }
    }

    public class ObjectArraySample
    {
        public int Id { get; set; }
        public SimpleSample[] Friends { get; set; }
    }
}

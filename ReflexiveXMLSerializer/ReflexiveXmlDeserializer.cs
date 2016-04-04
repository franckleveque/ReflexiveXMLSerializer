using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ReflexiveXMLSerializer
{
    public class ReflexiveXmlDeserializer
    {
        private Dictionary<Type, List<Tuple<PropertyInfo, bool, string, Type>>> references;
        private Type root;
        public ReflexiveXmlDeserializer(Type objectType)
        {
            references = new Dictionary<Type, List<Tuple<PropertyInfo, bool, string, Type>>>();
            root = objectType;
        }

        private void AddTypeToReferences(Type refToAdd)
        {
            if (!references.ContainsKey(refToAdd))
            {
                foreach (PropertyInfo curProp in refToAdd.GetProperties())
                {
                    if (!Utility.IsBaseType(curProp.PropertyType))
                    {
                        AddTypeToReferences(curProp.PropertyType);
                    }

                    references[refToAdd].Add(Tuple.Create(curProp, false, Utility.GetElementName(curProp).Name.ToString(), curProp.PropertyType));
                }
            }
        }

        public object Deserialize(string xml)
        {
            XDocument rootDoc = XDocument.Load(new StringReader(xml));
            XElement elem = rootDoc.Root;
            
            // We are in root object corresponding to the new type
            return DeserializeObject(root, elem);
        }

        private object DeserializeObject(Type objectType,XElement elem)
        {
            object result = objectType.GetConstructor(null).Invoke(null);
            if (Utility.IsBaseType(objectType))
            {
                result = Convert.ChangeType(elem.Value, objectType);
            }
            else
            {
                foreach (var c in references[root])
                {
                    // Serialize elements
                    if (c.Item2)
                    {
                        // Attribute property, normally a base type
                        c.Item1.SetValue(result, Convert.ChangeType(elem.Attribute(c.Item3), c.Item1.PropertyType));
                    }
                    else
                    {
                        Type propType = c.Item4;
                            if (propType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
                            {
                                // Instanciate a list of these elements
                                List<object> buffer = new List<object>();
                                foreach (var d in elem.Elements(c.Item3))
                                {
                                    buffer.Add(this.DeserializeObject(c.Item4, d));
                                }

                                // c.Item1.SetValue(result, buffer);
                            }
                            else
                            {
                                c.Item1.SetValue(result, this.DeserializeObject(c.Item4, elem.Element(c.Item3)));
                            }
                    }

                }
            }
            return result;
        }
    }
}

﻿using System;
using System.Collections;
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
            AddTypeToReferences(root);
        }

        private void AddTypeToReferences(Type refToAdd)
        {
            if (!references.ContainsKey(refToAdd))
            {
                references.Add(refToAdd, new List<Tuple<PropertyInfo, bool, string, Type>>());
                foreach (PropertyInfo curProp in refToAdd.GetProperties())
                {
                    if (!Utility.IsBaseType(curProp.PropertyType))
                    {
                        if (curProp.PropertyType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
                        {
                            // We have a collection
                            var typeToCheck = curProp.PropertyType.GetElementType();
                            if (!Utility.IsBaseType(typeToCheck))
                            {
                                AddTypeToReferences(typeToCheck);
                            }
                        }
                        else
                        {
                            AddTypeToReferences(curProp.PropertyType);
                        }
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
            object result = Activator.CreateInstance(objectType);
            if (Utility.IsBaseType(objectType))
            {
                result = Convert.ChangeType(elem.Value, objectType);
            }
            else
            {
                foreach (var c in references[objectType])
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
                        if (!Utility.IsBaseType(propType) && propType.GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
                        {
                            // Instanciate a list of these elements
                            Type elementType = propType.GetElementType();
                            List<object> buffer = new List<object>();
                            foreach (var d in elem.Element(c.Item3).Elements())
                            {
                                if (Utility.IsBaseType(elementType))
                                {
                                    buffer.Add(Convert.ChangeType(d.Value, elementType));
                                }
                                else
                                {
                                    buffer.Add(this.DeserializeObject(elementType, d));
                                }
                            }


                            MethodInfo castMethod = typeof(Enumerable).GetMethod("Cast")
                                .MakeGenericMethod(new System.Type[] { elementType });
                            MethodInfo toArrayMethod = null;
                            if (propType.IsArray)
                            {
                                toArrayMethod = typeof(Enumerable).GetMethod("ToArray")
                                .MakeGenericMethod(new System.Type[] { elementType });
                            }
                            else
                            {
                                toArrayMethod = typeof(Enumerable).GetMethod("ToList")
                                    .MakeGenericMethod(new System.Type[] { elementType });
                            }
                            var castedObjectEnum = castMethod.Invoke(null, new object[] { buffer });
                            c.Item1.SetValue(result, toArrayMethod.Invoke(null, new object[] { castedObjectEnum }));

                        }
                        else
                        {
                            c.Item1.SetValue(result, Convert.ChangeType(elem.Element(c.Item3).Value, propType));
                        }
                    }

                }
            }
            return result;
        }
    }
}

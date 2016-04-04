using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ReflexiveXMLSerializer
{
    /// <summary>
    /// Class used to serialize objects in xml using reflection
    /// </summary>
    public class ReflexiveXmlSerializer
    {
        #region Fields
       
        /// <summary>
        /// Keep trac of references serialized to avoid cyclic problems
        /// </summary>
        private Reference references = new Reference();

        #endregion Fields

        #region Methods

        /// <summary>
        /// Serialize a given object
        /// </summary>
        /// <param name="objToSerialize">Object to be serialized</param>
        /// <returns>Serialized string of object</returns>
        public string Serialize(object objToSerialize)
        {
            return this.SerializeObject(objToSerialize).ToString();
        }

        /// <summary>
        /// Serialize a given object
        /// </summary>
        /// <param name="objToSerialize">Object to be serialized</param>
        /// <param name="rootName">Name of root element</param>
        /// <returns>Serialized string of object</returns>
        public string Serialize(object objToSerialize, string rootName)
        {
            return this.SerializeObject(objToSerialize, new XElement(rootName)).ToString();
        }

        /// <summary>
        /// Serialize an object and and it to the root element
        /// </summary>
        /// <param name="objToSerialize">Object to be serialized</param>
        /// <param name="root">Root element of seriallization</param>
        /// <returns>XElement representing object serialization</returns>
        private XElement SerializeObject(object objToSerialize, XElement root)
        {
            if (!this.references.Contains(objToSerialize))
            {
                if (Utility.IsBaseType(objToSerialize.GetType()))
                {
                    root.Add(objToSerialize);
                }
                else
                {
                    this.references.Add(objToSerialize);
                    if (objToSerialize.GetType().GetInterfaces().Contains(typeof(System.Collections.IEnumerable)))
                    {
                        this.SerrializeArray(objToSerialize, root);
                    }
                    else
                    {
                        PropertyInfo[] properties = objToSerialize.GetType().GetProperties();

                        foreach (var curProp in properties)
                        {
                            this.SerializeProperty(objToSerialize, root, curProp);
                        }
                    }
                }

                return root;
            }
            else
            {
                throw new Exception("Cyclic reference in object tree");
            }
        }

        /// <summary>
        /// Serialize an object 
        /// </summary>
        /// <param name="objToSerialize">Object to be serialized</param>
        /// <returns>XElement representing object serialization</returns>
        private XElement SerializeObject(object objToSerialize)
        {
            return this.SerializeObject(objToSerialize, Utility.GetRootElementName(objToSerialize.GetType()));
        }

        /// <summary>
        /// Serialize a property of the object and add it to the root element
        /// </summary>
        /// <param name="objToSerialize">Object to be serialized</param>
        /// <param name="root">Root element of seriallization</param>
        /// <param name="curProp">Current property of object being serialized</param>
        private void SerializeProperty(object objToSerialize, XElement root, PropertyInfo curProp)
        {
            if (!Utility.IsPropertyToBeIgnored(curProp))
            {
                object value = curProp.GetValue(objToSerialize, null);
                if (value != null)
                {
                    root.Add(this.SerializeObject(curProp.GetValue(objToSerialize, null), Utility.GetElementName(curProp)));
                }
            }
        }

        /// <summary>
        /// Serrialize the elements of an IEnumerable element
        /// </summary>
        /// <param name="objToSerialize">Object to serialize</param>
        /// <param name="root">Root element of serialization</param>
        private void SerrializeArray(object objToSerialize, XElement root)
        {
            System.Collections.IEnumerable t = objToSerialize as System.Collections.IEnumerable;
            if (t != null)
            {
                foreach (object u in t)
                {
                    if (Utility.IsBaseType(u.GetType()))
                    {
                        if (u != null)
                        {
                            root.Add(new XElement(u.GetType().Name, u));
                        }
                        else
                        {
                            root.Add(new XElement(u.GetType().Name));
                        }
                    }
                    else
                    {
                        root.Add(this.SerializeObject(u, new XElement(Utility.GetRootElementName(u.GetType()))));
                    }
                }
            }
        }

        #endregion Methods

        #region Nested Types

        /// <summary>
        /// Class used to manage references of objects
        /// </summary>
        internal class Reference
        {
            #region Fields

            /// <summary>
            /// Dictionnary of type references already serialized
            /// </summary>
            private Dictionary<Type, List<object>> references = new Dictionary<Type, List<object>>();

            #endregion Fields

            #region Methods

            /// <summary>
            /// Add a new reference for the specified type
            /// </summary>
            /// <param name="type">Type of reference</param>
            /// <param name="hashKey">Reference to be added</param>
            public void Add(object hashKey)
            {
                Type type = hashKey.GetType();
                if (!this.references.ContainsKey(type))
                {
                    this.references.Add(type, new List<object>());
                }

                if (!this.references[type].Contains(hashKey))
                {
                    this.references[type].Add(hashKey);
                }
            }

            /// <summary>
            /// Check if the specified reference for the specified type exists
            /// </summary>
            /// <param name="type">Type of reference</param>
            /// <param name="hashKey">Reference to be checked</param>
            /// <returns>True if reference exists</returns>
            public bool Contains(object hashKey)
            {
                bool result = false;
                Type type = hashKey.GetType();
                if (this.references.ContainsKey(type))
                {
                    result = (from c in this.references[type]
                              where object.ReferenceEquals(c, hashKey)
                              select c).Any();
                }

                return result;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}
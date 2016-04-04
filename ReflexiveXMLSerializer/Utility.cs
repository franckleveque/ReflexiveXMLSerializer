using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ReflexiveXMLSerializer
{
    public static class Utility
    {
        /// <summary>
        /// Shortcut to XmlElementAttribute
        /// </summary>
        private static Type elementNameTag = typeof(XmlElementAttribute);

        /// <summary>
        /// Shortcut to XmlIgnoreAttributeType
        /// </summary>
        private static Type ignoreTag = typeof(XmlIgnoreAttribute);

        /// <summary>
        /// Shortcut to XmlRootAttribute
        /// </summary>
        private static Type rootNameTag = typeof(XmlRootAttribute);

        /// <summary>
        /// Shortcut to XmlRootAttribute
        /// </summary>
        private static Type typeNameTag = typeof(XmlTypeAttribute);

        /// <summary>
        /// Check if given type is a base type or not
        /// </summary>
        /// <param name="prop">Type to be checked</param>
        /// <returns>True if type is among base type</returns>
        public static bool IsBaseType(Type prop)
        {
            return prop.Equals(typeof(bool)) ||
                prop.Equals(typeof(byte)) ||
                prop.Equals(typeof(char)) ||
                prop.Equals(typeof(decimal)) ||
                prop.Equals(typeof(double)) ||
                prop.Equals(typeof(float)) ||
                prop.Equals(typeof(int)) ||
                prop.Equals(typeof(long)) ||
                prop.Equals(typeof(sbyte)) ||
                prop.Equals(typeof(short)) ||
                prop.Equals(typeof(string)) ||
                prop.Equals(typeof(uint)) ||
                prop.Equals(typeof(ulong)) ||
                prop.Equals(typeof(ushort)) ||
                prop.Equals(typeof(DateTime));
        }

        /// <summary>
        /// Get element name of property
        /// </summary>
        /// <param name="curProp">Property from which to retrieve element name</param>
        /// <returns>Name of Element</returns>
        public static XElement GetElementName(PropertyInfo curProp)
        {
            XmlElementAttribute[] result = curProp.GetCustomAttributes(elementNameTag, true).OfType<XmlElementAttribute>().ToArray();
            string buffer = curProp.Name;
            if (result.Count() == 1)
            {
                buffer = result[0].ElementName;
            }

            XElement prop = new XElement(buffer);
            return prop;
        }

        /// <summary>
        /// Get element name of object type
        /// </summary>
        /// <param name="objToSerialize">Type from which to retrieve element name</param>
        /// <returns>Name of Element</returns>
        public static XElement GetRootElementName(Type objToSerialize)
        {
            XmlRootAttribute[] rootAttributes = objToSerialize.GetCustomAttributes(rootNameTag, true).OfType<XmlRootAttribute>().ToArray();
            string buffer = GetXmlTypeName(objToSerialize);
            if (rootAttributes.Count() == 1)
            {
                buffer = rootAttributes[0].ElementName;
            }
            else
            {
                XmlTypeAttribute[] typeAttributes = objToSerialize.GetCustomAttributes(typeNameTag, true).OfType<XmlTypeAttribute>().ToArray();
                if (typeAttributes.Count() == 1)
                {
                    buffer = typeAttributes[0].TypeName;
                }
            }

            XElement prop = new XElement(buffer);
            return prop;
        }

        /// <summary>
        /// Retrieve an Xml compatible type name for the given type
        /// </summary>
        /// <param name="type">Type of which to retrieve name</param>
        /// <returns>Xml compatible type name</returns>
        public static string GetXmlTypeName(Type type)
        {
            if (type.IsGenericType)
            {
                string buffer = type.Name.Split("`".ToCharArray())[0] + "Of";
                Type[] internalTypes = type.GetGenericArguments();

                foreach (Type curType in internalTypes)
                {
                    buffer += GetXmlTypeName(curType);
                }

                return buffer.Replace("<", string.Empty).Replace(">", string.Empty);
            }
            else if (type.IsArray)
            {
                return "ArrayOf" + GetXmlTypeName(type.GetElementType()).Replace("<", string.Empty).Replace(">", string.Empty);
            }
            else
            {
                return type.Name.Replace("<", string.Empty).Replace(">", string.Empty);
            }
        }

        /// <summary>
        /// Check if property of object have to be ignored during serialization
        /// </summary>
        /// <param name="prop">Property to be checked</param>
        /// <returns>True if property have to be ignored</returns>
        public static bool IsPropertyToBeIgnored(PropertyInfo prop)
        {
            if (prop.GetIndexParameters().Count() > 0)
            {
                return true;
            }
            else
            {
                // Check XmlIgnoreAttribute
                object[] result = prop.GetCustomAttributes(ignoreTag, true);
                return result.Count() > 0;
            }
        }
    }
}

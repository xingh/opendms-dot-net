﻿/* Copyright 2011 the OpenDMS.NET Project (http://sites.google.com/site/opendmsnet/)
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.IO;
using System.Reflection;

namespace Common.NetworkPackage
{
    /// <summary>
    /// A customized XML based serializer.
    /// </summary>
    public class XmlSerializer
    {
        /// <summary>
        /// Serializes the specified object using the specified XML writer.
        /// </summary>
        /// <param name="xmlWriter">The XML writer.</param>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The XML writer argument.</returns>
        public static System.Xml.XmlWriter Serialize(System.Xml.XmlWriter xmlWriter, object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");
            
            if (xmlWriter.Settings.Encoding != System.Text.Encoding.UTF8)
                throw new FormatException("Encoding must be UTF8");

            if (obj.GetType() == typeof(DictionaryBase<string, object>))
            {
                xmlWriter = ((DictionaryBase<string, object>)obj).Serialize(xmlWriter);
            }
            else if (obj.GetType() == typeof(DictionaryEntry<string, object>))
            {
                xmlWriter = ((DictionaryEntry<string, object>)obj).Serialize(xmlWriter);
            }
            else if (obj.GetType() == typeof(FormProperty))
            {
                xmlWriter = ((FormProperty)obj).Serialize(xmlWriter);
            }
            else if (obj.GetType() == typeof(Storage.MetaAsset))
            {
                xmlWriter = ((Storage.MetaAsset)obj).Serialize(xmlWriter);
            }
            else if (obj.GetType() == typeof(MetaFormProperty))
            {
                xmlWriter = ((MetaFormProperty)obj).Serialize(xmlWriter);
            }
            else
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(obj.GetType());
                ser.Serialize(xmlWriter, obj);
                //MemoryStream ms = new MemoryStream();
                //ser.Serialize(ms, obj);
                //str = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            }
            return xmlWriter;
        }

        /// <summary>
        /// Deserializes the specified target from the content of the argument str.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to target for deserialization.</typeparam>
        /// <param name="target">The target object to populate.</param>
        /// <param name="str">The string containing content to deserialize.</param>
        /// <returns>A deserialized and instantited T</returns>
        public static T Deserialize<T>(object target, string str)
        {
            System.Xml.XmlReader xmlReader = System.Xml.XmlReader.Create(
                new MemoryStream(System.Text.Encoding.UTF8.GetBytes(str)), 
                new System.Xml.XmlReaderSettings() { IgnoreWhitespace = true });
            target = (T)Deserialize<T>(target, xmlReader);
            xmlReader.Close();
            return (T)target;
        }

        /// <summary>
        /// Deserializes the specified target from the content of the argument xmlReader.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> to target for deserialization.</typeparam>
        /// <param name="target">The target object to populate.</param>
        /// <param name="xmlReader">The XML reader.</param>
        /// <returns>A deserialized and instantited T</returns>
        public static T Deserialize<T>(object target, System.Xml.XmlReader xmlReader)
        {
            if (target.GetType() == typeof(DictionaryEntry<string, object>))
            {
                if(target == null) target = new DictionaryEntry<string, object>();
                xmlReader = ((DictionaryEntry<string, object>)target).Deserialize(xmlReader);
            }
            else if (target.GetType() == typeof(Storage.MetaAsset))
            {
                if (target == null) target = new Storage.MetaAsset();
                xmlReader = ((Storage.MetaAsset)target).Deserialize(xmlReader);
            }
            else
            {
                System.Xml.Serialization.XmlSerializer ser = new System.Xml.Serialization.XmlSerializer(target.GetType());
                try
                {
                    target = (T)ser.Deserialize(xmlReader);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
            return (T)target;
        }

        /// <summary>
        /// Detects and automatically deserializes multiple data types within the xmlReader.
        /// </summary>
        /// <param name="xmlReader">The XML reader.</param>
        /// <returns>A deserialized and instantited object.</returns>
        public static object SimpleDeserialize(System.Xml.XmlReader xmlReader)
        {
            System.Xml.Serialization.XmlSerializer valueSerializer = null;

            switch (xmlReader.LocalName)
            {
                case "boolean":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(bool));
                    return valueSerializer.Deserialize(xmlReader);
                case "string":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(string));
                    return valueSerializer.Deserialize(xmlReader);
                case "guid":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(Guid));
                    return valueSerializer.Deserialize(xmlReader);
                case "object":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(object));
                    return valueSerializer.Deserialize(xmlReader);
                case "dateTime":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(DateTime));
                    return valueSerializer.Deserialize(xmlReader);
                case "int":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(int));
                    return valueSerializer.Deserialize(xmlReader);
                case "unsignedInt":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(uint));
                    return valueSerializer.Deserialize(xmlReader);
                case "long":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(long));
                    return valueSerializer.Deserialize(xmlReader);
                case "unsignedLong":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(ulong));
                    return valueSerializer.Deserialize(xmlReader);
                case "double":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(double));
                    return valueSerializer.Deserialize(xmlReader);
                case "ListOfString":
                    return XmlSerializer.Deserialize<List<string>>(new List<string>(), xmlReader);
                case "ArrayOfString":
                    string[] a = new string[1];
                    return XmlSerializer.Deserialize<string[]>(a, xmlReader);
                case "ErrorCode":
                    valueSerializer = new System.Xml.Serialization.XmlSerializer(typeof(NetworkPackage.ServerResponse.ErrorCode));
                    return valueSerializer.Deserialize(xmlReader);
                default:
                    throw new NotSupportedException();
            }

        }
    }
}
// --------------------------------------------------------------------------------------------------------------------
// <copyright file="XmlExtensions.cs" company="Usama Nada">
//   No Copyright .. Copy, Share, and Evolve.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Framework.Core.Extensions
{
    #region usings

    using System;

    using System.Linq;
    using System.Xml;

    #endregion

    /// <summary>
    ///     Xml Extensions
    /// </summary>
    public static class XmlExtensions
    {
        /// <summary>
        /// Appends a CData section to a XML node and prefills the provided data
        /// </summary>
        /// <param name="cData">
        /// The CData section value
        /// </param>
        /// <param name="sourceNode">
        /// The parent node
        /// </param>
        public static void AppendCDataSectionTo(this string cData, XmlNode sourceNode)
        {
            var xmlDocument = sourceNode as XmlDocument;
            var document = xmlDocument ?? sourceNode.OwnerDocument;
            if (document != null)
            {
                var node = document.CreateCDataSection(cData);
                sourceNode.AppendChild(node);
            }
        }

        /// <summary>
        /// Appends a child to a XML node
        /// </summary>
        /// <param name="childNode">
        /// The name of the child node
        /// </param>
        /// <param name="sourceNode">
        /// The parent node
        /// </param>
        public static void AppendChildNodeTo(this string childNode, XmlNode sourceNode)
        {
            var xmlDocument = sourceNode as XmlDocument;
            var document = xmlDocument ?? sourceNode.OwnerDocument;
            if (document != null)
            {
                XmlNode node = document.CreateElement(childNode);
                sourceNode.AppendChild(node);
            }
        }

        /// <summary>
        /// Appends a child to a XML node
        /// </summary>
        /// <param name="childNode">
        /// The name of the child node
        /// </param>
        /// <param name="sourceNode">
        /// The parent node
        /// </param>
        /// <param name="namespaceUri">
        /// The node namespace
        /// </param>
        public static void AppendChildNodeTo(this string childNode, XmlNode sourceNode, string namespaceUri)
        {
            var xmlDocument = sourceNode as XmlDocument;
            var document = xmlDocument ?? sourceNode.OwnerDocument;
            if (document != null)
            {
                XmlNode node = document.CreateElement(childNode, namespaceUri);
                sourceNode.AppendChild(node);
            }
        }

        /// <summary>
        /// Appends a CData section to a XML node
        /// </summary>
        /// <param name="source">
        /// The parent node
        /// </param>
        /// <returns>
        /// The created CData Section
        /// </returns>
        public static XmlCDataSection CreateCDataSection(this XmlNode source)
        {
            return source.CreateCDataSection(string.Empty);
        }

        /// <summary>
        /// Appends a CData section to a XML node and prefills the provided data
        /// </summary>
        /// <param name="source">
        /// The parent node
        /// </param>
        /// <param name="cData">
        /// The CData section value
        /// </param>
        /// <returns>
        /// The created CData Section
        /// </returns>
        public static XmlCDataSection CreateCDataSection(this XmlNode source, string cData)
        {
            var xmlDocument = source as XmlDocument;
            var document = xmlDocument ?? source.OwnerDocument;
            if (document != null)
            {
                var node = document.CreateCDataSection(cData);
                source.AppendChild(node);
                return node;
            }

            return null;
        }

        /// <summary>
        /// Appends a child to a XML node
        /// </summary>
        /// <param name="source">
        /// The parent node
        /// </param>
        /// <param name="childNode">
        /// The name of the child node
        /// </param>
        /// <returns>
        /// The newly created XML node
        /// </returns>
        public static XmlNode CreateChildNode(this XmlNode source, string childNode)
        {
            var xmlDocument = source as XmlDocument;
            var document = xmlDocument ?? source.OwnerDocument;
            if (document != null)
            {
                XmlNode node = document.CreateElement(childNode);
                source.AppendChild(node);
                return node;
            }

            return null;
        }

        /// <summary>
        /// Appends a child to a XML node
        /// </summary>
        /// <param name="source">
        /// The parent node
        /// </param>
        /// <param name="childNode">
        /// The name of the child node
        /// </param>
        /// <param name="namespaceUri">
        /// The node namespace
        /// </param>
        /// <returns>
        /// The newly cerated XML node
        /// </returns>
        public static XmlNode CreateChildNode(this XmlNode source, string childNode, string namespaceUri)
        {
            var xmlDocument = source as XmlDocument;
            var document = xmlDocument ?? source.OwnerDocument;
            if (document != null)
            {
                XmlNode node = document.CreateElement(childNode, namespaceUri);
                source.AppendChild(node);
                return node;
            }

            return null;
        }

        /// <summary>
        /// Creates an Xml NamespaceManager for an XML document by looking
        ///     at all of the namespaces defined on the document root element.
        /// </summary>
        /// <param name="doc">
        /// The XmlDom instance to attach the namespacemanager to
        /// </param>
        /// <param name="defaultNamespace">
        /// The prefix to use for prefix-less nodes (which are not supported if any namespaces are
        ///     used in XmlDoc).
        /// </param>
        /// <returns>
        /// The <see cref="XmlNamespaceManager"/>.
        /// </returns>
        public static XmlNamespaceManager CreateXmlNamespaceManager(this XmlDocument doc, string defaultNamespace)
        {
            var nsmgr = new XmlNamespaceManager(doc.NameTable);
            if (doc.DocumentElement != null)
            {
                foreach (XmlAttribute attr in doc.DocumentElement.Attributes)
                {
                    if (attr.Prefix == "xmlns")
                    {
                        nsmgr.AddNamespace(attr.LocalName, attr.Value);
                    }

                    if (attr.Name == "xmlns")
                    {
                        // default namespace MUST use a prefix
                        nsmgr.AddNamespace(defaultNamespace, attr.Value);
                    }
                }
            }

            return nsmgr;
        }

        /// <summary>
        /// Gets an attribute value
        ///     If the value is empty, uses the specified default value
        /// </summary>
        /// <param name="source">
        /// The node to retreive the value from
        /// </param>
        /// <param name="attributeName">
        /// The Name of the attribute
        /// </param>
        /// <param name="defaultValue">
        /// The default value to be returned if no matching attribute exists
        /// </param>
        /// <returns>
        /// The attribute value
        /// </returns>
        public static string GetAttribute(this XmlNode source, string attributeName, string defaultValue = null)
        {
            if (source.Attributes != null)
            {
                var attribute = source.Attributes[attributeName];
                return attribute?.InnerText ?? defaultValue;
            }

            return null;
        }

        /// <summary>
        /// Gets an attribute value converted to the specified data type
        ///     If the value is empty, uses the specified default value
        /// </summary>
        /// <typeparam name="T">
        /// The desired return data type
        /// </typeparam>
        /// <param name="source">
        /// The node to evaluate
        /// </param>
        /// <param name="attributeName">
        /// The Name of the attribute
        /// </param>
        /// <param name="defaultValue">
        /// The default value to be returned if no matching attribute exists
        /// </param>
        /// <returns>
        /// The attribute value
        /// </returns>
        public static T GetAttribute<T>(this XmlNode source, string attributeName, T defaultValue = default(T))
        {
            var value = GetAttribute(source, attributeName);

            return !string.IsNullOrEmpty(value) ? value.To(defaultValue) : defaultValue;
        }

        /// <summary>
        /// Returns the value of a nested CData section
        /// </summary>
        /// <param name="source">
        /// The parent node
        /// </param>
        /// <returns>
        /// The CData section content
        /// </returns>
        public static string GetCDataSection(this XmlNode source)
        {
            return source.ChildNodes.OfType<XmlCDataSection>().Select(e => e.Value).FirstOrDefault();
        }

        /// <summary>
        /// Returns an integer value from an attribute
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="attributeName">
        /// </param>
        /// <param name="defaultValue">
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetXmlAttributeInt(this XmlNode node, string attributeName, int defaultValue)
        {
            var val = GetXmlAttributeString(node, attributeName);
            if (val == null)
            {
                return defaultValue;
            }

            return XmlConvert.ToInt32(val);
        }

        /// <summary>
        /// Gets an attribute by name
        /// </summary>
        /// <param name="node">
        /// </param>
        /// <param name="attributeName">
        /// </param>
        /// <returns>
        /// value or null if not available
        /// </returns>
        public static string GetXmlAttributeString(this XmlNode node, string attributeName)
        {
            var att = node.Attributes?[attributeName];

            return att?.InnerText;
        }

        /// <summary>
        /// Retrieves a result bool from an XPATH query. false if not found.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="xPath">
        /// The x path.
        /// </param>
        /// <param name="ns">
        /// The ns.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public static bool GetXmlBool(this XmlNode node, string xPath, XmlNamespaceManager ns)
        {
            var val = GetXmlString(node, xPath, ns);
            if (val == null)
            {
                return false;
            }

            if (val == "1" || val == "true" || val == "True")
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Retrieves a result DateTime from an XPATH query. 1/1/1900  if not found.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="xPath">
        /// The x path.
        /// </param>
        /// <param name="ns">
        /// The ns.
        /// </param>
        /// <returns>
        /// The <see cref="DateTime"/>.
        /// </returns>
        public static DateTime GetXmlDateTime(this XmlNode node, string xPath, XmlNamespaceManager ns)
        {
            var dtVal = new DateTime(1900, 1, 1, 0, 0, 0);

            var val = GetXmlString(node, xPath, ns);
            if (val == null)
            {
                return dtVal;
            }

            try
            {
                dtVal = XmlConvert.ToDateTime(val, XmlDateTimeSerializationMode.Utc);
            }
            catch
            {
                // ignored
            }

            return dtVal;
        }

        /// <summary>
        /// Retrieves a result int value from an XPATH query. 0 if not found.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="xPath">
        /// The x path.
        /// </param>
        /// <param name="ns">
        /// The ns.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        public static int GetXmlInt(this XmlNode node, string xPath, XmlNamespaceManager ns)
        {
            var val = GetXmlString(node, xPath, ns);
            if (val == null)
            {
                return 0;
            }

            int result;
            int.TryParse(val, out result);

            return result;
        }

        /// <summary>
        /// Retrieves a result string from an XPATH query. Null if not found.
        /// </summary>
        /// <param name="node">
        /// The node.
        /// </param>
        /// <param name="xPath">
        /// The x path.
        /// </param>
        /// <param name="ns">
        /// The ns.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetXmlString(this XmlNode node, string xPath, XmlNamespaceManager ns)
        {
            var selNode = node.SelectSingleNode(xPath, ns);

            return selNode?.InnerText;
        }

        /// <summary>
        /// Creates or updates an attribute with the passed object
        /// </summary>
        /// <param name="source">
        /// The node to evaluate
        /// </param>
        /// <param name="name">
        /// The attribute name
        /// </param>
        /// <param name="value">
        /// The attribute value
        /// </param>
        public static void SetAttribute(this XmlNode source, string name, object value)
        {
            SetAttribute(source, name, value?.ToString());
        }

        /// <summary>
        /// Creates or updates an attribute with the passed value
        /// </summary>
        /// <param name="source">
        /// The node to evaluate
        /// </param>
        /// <param name="name">
        /// The attribute name
        /// </param>
        /// <param name="value">
        /// The attribute value
        /// </param>
        public static void SetAttribute(this XmlNode source, string name, string value)
        {
            if (source?.Attributes != null)
            {
                var attribute = source.Attributes[name, source.NamespaceURI];

                if (attribute == null)
                {
                    if (source.OwnerDocument != null)
                    {
                        attribute = source.OwnerDocument.CreateAttribute(name, source.OwnerDocument.NamespaceURI);
                    }

                    if (attribute != null)
                    {
                        source.Attributes.Append(attribute);
                    }
                }

                if (attribute != null)
                {
                    attribute.InnerText = value;
                }
            }
        }
    }
}
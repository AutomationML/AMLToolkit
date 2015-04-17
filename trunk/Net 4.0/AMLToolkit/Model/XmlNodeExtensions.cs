// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="XmlNodeExtensions.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Xml;
using CAEX_ClassModel;

namespace AMLToolkit.Model
{
    internal static class XmlNodeExtensions
    {
        #region Internal Methods

        /// <summary>
        ///    creates CAEX object out of an XML node
        /// </summary>
        /// <param name="node">XML node</param>
        /// <returns>CAEXBasicObject</returns>
        internal static CAEXBasicObject CreateCAEXWrapper(this XmlNode node)
        {
            if (!CAEXTypeDict.Instance.ContainsKey(node.Name))
                throw new ArgumentException("Don't know how to wrap XML node of type " + CAEXTypeDict.Instance[node.Name], "node");
            Type t = CAEXTypeDict.Instance[node.Name];
            var constructor = t.GetConstructor(new[] { typeof(XmlNode) });
            if (constructor == null)
                throw new ArgumentException(t.FullName + " doesn't have a constructor with a single argument of type XmlNode");
            if (node == null)
                throw new ArgumentNullException("node");
            CAEXBasicObject result = (CAEXBasicObject)constructor.Invoke(new object[] { node });
            return result;
        }

        /// <summary>
        ///    Getting the full path from document root to the specified XmlNode. As name
        ///    of an XmlNode is the value of the contained attribute "name" taken.
        ///    Hierarchies are separated via the path separator (slash '/'). The name of
        ///    the document root element CAEXFile is not contained in the path.
        /// </summary>
        /// <param name="xmlNode">The XmlNode to get the full path for</param>
        /// <returns>
        ///    The full path from the document root element to the given XmlNode. The path
        ///    contains of values of the "name" attribute of the XmlNodes. If no such
        ///    attribute exists in the current node, the xml tag name is used instead.
        /// </returns>
        internal static string getFullNodePath(this XmlNode xmlNode)
        {
            XmlNode currentElement = xmlNode;
            XmlAttribute nameAttribute = (xmlNode != null && xmlNode.Attributes != null) ? xmlNode.Attributes["Name"] : null;
            string pathString = (nameAttribute != null) ? nameAttribute.Value : "";

            if (!string.IsNullOrEmpty(pathString))
            {
                while (currentElement.ParentNode != null)
                {
                    currentElement = currentElement.ParentNode;
                    if (currentElement != null && currentElement.Attributes != null)
                    {
                        if ((nameAttribute = currentElement.Attributes["Name"]) != null)
                        {
                            pathString = nameAttribute.Value + CAEXPathBuilder.ObjectSeparator + pathString;
                        }
                    }
                }
            }
            return pathString;
        }

        #endregion Internal Methods
    }
}
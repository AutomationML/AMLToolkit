// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="XmlElementExtension.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CAEX_ClassModel;

/// <summary>
///    The XmlExtension namespace provides some methods to access CAEX Elements via an
///    XmlElement. This Methods don't need a CAEXWrapper.
/// </summary>
namespace AMLToolkit.Model
{
    /// <summary>
    ///    Class XmlElementExtension provides some Extension Methods to access CAEX
    ///    Elements via an XmlElement. This Methods don't need a CAEXWrapper.
    /// </summary>
    public static class XmlElementExtension
    {
        #region public Methods

        /// <summary>
        /// Gets the ancestor with the specified XmlNode Name
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="name">The name.</param>
        /// <returns>XmlElement.</returns>
        public static XmlElement GetAncestor(this XmlElement element, string name)
        {
            var navigator = element.CreateNavigator();
            var iterator = navigator.SelectAncestors(name, "", false);
            if (iterator.MoveNext())
                return iterator.Current.UnderlyingObject as XmlElement;
            else
                return null;
        }

        /// <summary>
        ///    Alls the elements.
        /// </summary>
        /// <param name="element">  The element.</param>
        /// <param name="Name">     The name.</param>
        /// <param name="matchSelf">if set to <c>true</c> [match self].</param>
        /// <returns>IEnumerable&lt;XmlElement&gt;.</returns>
        public static IEnumerable<XmlElement> AllElements(this XmlElement element, string Name, bool matchSelf)
        {
            var navigator = element.CreateNavigator();
            var iterator = navigator.SelectDescendants(Name, string.Empty, matchSelf);

            while (iterator.MoveNext())
                yield return iterator.Current.UnderlyingObject as XmlElement;
        }

        /// <summary>
        ///    Returns the CAEX ID of a the XmlElement if such an Attribute exists
        /// </summary>
        /// <param name="element">The XmlElement.</param>
        /// <returns>The Name or string.empty if the ID-Attribute is missing</returns>
        public static string CAEXIDOfElement(this XmlElement element)
        {
            if (element != null && element.HasAttribute("ID"))
                return element.Attributes["ID"].Value;

            return string.Empty;
        }

        /// <summary>
        ///    Returns the CAEX Name of a the XmlElement if such an Attribute exists
        /// </summary>
        /// <param name="element">The XmlElement.</param>
        /// <returns>The Name or string.empty if the Name-Attribute is missing</returns>
        public static string CAEXNameOfElement(this XmlElement element)
        {
            if (element != null && element.HasAttribute("Name"))
                return element.Attributes["Name"].Value;

            return string.Empty;
        }

        /// <summary>
        ///    Returns the CAEX Name of a parent XmlElement if such an Attribute exists
        /// </summary>
        /// <param name="element">The XmlElement.</param>
        /// <returns>
        ///    The Name or string.empty if the Parent or the Name-Attribute is missing
        /// </returns>
        public static string CAEXNameOfParent(this XmlElement element)
        {
            if (element != null && element.ParentNode is XmlElement)
            {
                return ((XmlElement)element.ParentNode).CAEXNameOfElement();
            }
            return string.Empty;
        }

        /// <summary>
        /// get the Mappings path to role reference.
        /// </summary>
        /// <param name="mappingAttribute">The mapping attribute.</param>
        /// <returns>System.String.</returns>
        public static string MappingPathToRoleReference(this XmlElement mappingAttribute)
        {
            string roleAttribute = string.Empty;

            if (mappingAttribute.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING)
            {
                roleAttribute = mappingAttribute.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_ROLE_ATTRIBUTE_NAME);
            }
            else if (mappingAttribute.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING)
            {
                roleAttribute = mappingAttribute.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_ROLE_INTERFACE_NAME);
            }

            // a role attribute is set in the mapping, try to find the corresponding role
            // for this attribute
            if (!string.IsNullOrEmpty(roleAttribute))
            {
                XmlElement MappingObject = mappingAttribute.ParentNode as XmlElement;
                XmlElement MappingParent = (MappingObject != null) ? MappingObject.ParentNode as XmlElement : null;
                List<XmlElement> RoleReferenceList = new List<XmlElement>();

                if (MappingParent != null)
                {
                    if (MappingParent.IsSupportedRoleClass())
                    {
                        RoleReferenceList.Add(MappingParent);
                    }
                    else if (MappingParent.IsInternalElement())
                    {
                        var nodeList = MappingParent.SelectNodes(string.Format("./{0} | ./{1}", CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING, CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING));
                        if (nodeList.Count > 0)
                            RoleReferenceList.AddRange(nodeList.Cast<XmlElement>());
                    }

                    // now there are one or multiple roles in the list
                    if (RoleReferenceList.Count() == 1)
                    {
                        return RoleReferenceList[0].RoleReference();
                    }

                    // if there are more than one role references, the role Attribute has
                    // to be considered, if it contains a role name
                    else
                    {
                        foreach (var roleReference in RoleReferenceList)
                        {
                            var rolePath = roleReference.RoleReference();
                            if (!string.IsNullOrEmpty(rolePath))
                            {
                                if (roleAttribute.StartsWith(System.IO.Path.GetFileName(rolePath) + "."))
                                    return rolePath;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }


        /// <summary>
        /// return the Mapping Path to a system unit class reference.
        /// </summary>
        /// <param name="mappingAttribute">The mapping attribute.</param>
        /// <returns>System.String.</returns>
        public static string MappingPathToSystemUnitClassReference(this XmlElement mappingAttribute)
        {
            XmlElement MappingObject = mappingAttribute.ParentNode as XmlElement;
            XmlElement MappingParent = (MappingObject != null) ? MappingObject.ParentNode as XmlElement : null;
            List<XmlElement> RoleReferenceList = new List<XmlElement>();

            if (MappingParent != null)
            {
                if (MappingParent.IsSupportedRoleClass() && MappingParent.ParentNode is XmlElement)
                {
                    return ((XmlElement)MappingParent.ParentNode).CAEXPath();
                }
                else if (MappingParent.IsInternalElement())
                {
                    return MappingParent.CAEXIDOfElement();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Determines whether [has child nodes] [the specified child element name].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="childElementName">Name of the child element.</param>
        /// <returns><c>true</c> if [has child nodes] [the specified child element name]; otherwise, <c>false</c>.</returns>
        public static bool HasChildNodes(this XmlElement element, string childElementName)
        {
            var navigator = element.CreateNavigator();
            var iterator = navigator.SelectChildren(childElementName, string.Empty);
            return iterator.MoveNext();
        }

        /// <summary>
        /// Determines whether [is attribute name mapping] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if [is attribute name mapping] [the specified element]; otherwise, <c>false</c>.</returns>
        public static bool IsAttributeNameMapping(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING;
        }

        /// <summary>
        /// Determines whether [is interface name mapping] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if [is interface name mapping] [the specified element]; otherwise, <c>false</c>.</returns>
        public static bool IsInterfaceNameMapping(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING;
        }

        /// <summary>
        ///    Gets th RoleReference for a mapping.
        /// </summary>
        /// <param name="mappingAttribute">The mapping attribute.</param>
        /// <returns>XmlElement.</returns>
        public static XmlElement RoleReferenceToMapping(this XmlElement mappingAttribute)
        {
            string roleAttribute = string.Empty;

            if (mappingAttribute.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING)
            {
                roleAttribute = mappingAttribute.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_ROLE_ATTRIBUTE_NAME);
            }
            else if (mappingAttribute.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING)
            {
                roleAttribute = mappingAttribute.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_ROLE_INTERFACE_NAME);
            }

            // a role attribute is set in the mapping, try to find the corresponding role
            // for this attribute
            if (!string.IsNullOrEmpty(roleAttribute))
            {
                XmlElement MappingObject = mappingAttribute.ParentNode as XmlElement;
                XmlElement MappingParent = (MappingObject != null) ? MappingObject.ParentNode as XmlElement : null;
                List<XmlElement> RoleReferenceList = new List<XmlElement>();

                if (MappingParent != null)
                {
                    if (MappingParent.IsSupportedRoleClass())
                    {
                        RoleReferenceList.Add(MappingParent);
                    }
                    else if (MappingParent.IsInternalElement())
                    {
                        var nodeList = MappingParent.SelectNodes(string.Format("./{0} | ./{1}", CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING, CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING));
                        if (nodeList.Count > 0)
                            RoleReferenceList.AddRange(nodeList.Cast<XmlElement>());
                    }

                    // now there are one or multiple roles in the list
                    if (RoleReferenceList.Count() == 1)
                    {
                        return RoleReferenceList[0];
                    }

                    // if there are more than one role references, the role Attribute has
                    // to be considered, if it contains a role name
                    else
                    {
                        foreach (var roleReference in RoleReferenceList)
                        {
                            var rolePath = roleReference.RoleReference();
                            if (!string.IsNullOrEmpty(rolePath))
                            {
                                if (roleAttribute.StartsWith(System.IO.Path.GetFileName(rolePath) + "."))
                                    return roleReference;
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///    Gets the SystemUnitClass-Reference for a mapping.
        /// </summary>
        /// <param name="mappingAttribute">The mapping attribute.</param>
        /// <returns>XmlElement.</returns>
        public static XmlElement SystemUnitClassReferenceToMapping(this XmlElement mappingAttribute)
        {
            //string sucAttribute = string.Empty;

            //if (mappingAttribute.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING)
            //{
            //    sucAttribute = mappingAttribute.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_SYSTEM_UNIT_ATTRIBUTE_NAME);
            //}
            //else if (mappingAttribute.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING)
            //{
            //    sucAttribute = mappingAttribute.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_SYSTEM_UNIT_INTERFACE_NAME);
            //}

            //// a system Unit class attribute is set in the mapping, try to find the corresponding systemUnitClass for this attribute
            //if (!string.IsNullOrEmpty(sucAttribute))
            //{
            XmlElement MappingObject = mappingAttribute.ParentNode as XmlElement;
            XmlElement MappingParent = (MappingObject != null) ? MappingObject.ParentNode as XmlElement : null;

            if (MappingParent != null)
            {
                if (MappingParent.IsSupportedRoleClass())
                {
                    return (XmlElement)MappingParent.ParentNode;
                }
                else if (MappingParent.IsInternalElement())
                {
                    return MappingParent;
                }
            }
            //}

            return null;
        }

        /// <summary>
        ///    Returns the CAEX path of this object, search able with FindFastByID or
        ///    FindFastByPath methods. Examples: "plant/unit/tank" or GUID:interface. If
        ///    the full hierarchical Path is required, use the method HierarchyPath
        ///    instead which delivers the full node path.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    For classes, it returns the XML full Node Path. For Interface Instances, it
        ///    returns GUID:InterfaceName.
        /// </returns>
        public static string CAEXPath(this XmlElement element)
        {
            //check if Obj is Interface instance
            if (element.Name == CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING)
            {
                string path = "";

                XmlElement parent = element.ParentNode as XmlElement;
                if (parent != null)
                {
                    if (parent.Name == CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING || parent.Name == CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING)
                    {
                        var idAttribute = (parent.HasAttributes) ? parent.Attributes["ID"] : null;

                        if (idAttribute != null && !string.IsNullOrEmpty(idAttribute.Value))
                        {
                            path = internalLinkReferencePath(parent, element);
                            return path;
                        }
                    }
                    else
                    {
                        path = parent.CAEXPath() + CAEXPathBuilder.InterfaceSeparator + PathPart(element);
                        return path;
                    }
                }
            }
            return element.getFullNodePath();
        }

        /// <summary>
        ///    gets the reference path for an Interface-Reference in an internalLink
        /// </summary>
        /// <param name="interfaceParent">  
        ///    The interface Parent (SystemUnitClass or publicElement)
        /// </param>
        /// <param name="externalInterface">The external Interface.</param>
        /// <returns>System.String.</returns>
        public static string internalLinkReferencePath(System.Xml.XmlElement interfaceParent, System.Xml.XmlElement externalInterface)
        {
            string APath = PathPart(interfaceParent, true);
            string BPath = PathPart(externalInterface, false);

            if (BPath.StartsWith(CAEXPathBuilder.PathPartBegin))
                APath = CAEXPathBuilder.PathPartBegin + APath;
            if (BPath.EndsWith(CAEXPathBuilder.PathPartEnd))
                APath = APath + CAEXPathBuilder.PathPartEnd;

            return APath + CAEXPathBuilder.InterfaceSeparator + BPath;
        }

        /// <summary>
        ///    transforms the name of the defined element to a path Part (special
        ///    characters are escaped).
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="UseId">  
        ///    if set to <c>true</c> use the Elements ID, otherwise use its name.
        /// </param>
        /// <returns>System.String.</returns>
        public static string PathPart(XmlElement element, bool UseId = false)
        {
            if (!UseId)
            {
                var nameAttribute = element.HasAttributes ? element.Attributes["Name"] : null;
                string name = (nameAttribute != null) ? nameAttribute.Value : "";

                return PathPartName(name);
            }
            else
            {
                var idAttribute = element.HasAttributes ? element.Attributes["ID"] : null;

                if (idAttribute == null)
                {
                    idAttribute = element.OwnerDocument.CreateAttribute("ID");
                    element.Attributes.Append(idAttribute);
                }

                if (string.IsNullOrEmpty(idAttribute.Value))
                    idAttribute.Value = Guid.NewGuid().ToString(CAEXObject.GUID_FORMAT);
                else if (idAttribute.Value[0] == '{')
                    idAttribute.Value = idAttribute.Value.Trim('{', '}');

                return idAttribute.Value;
            }
        }

        /// <summary>
        ///    transforms a name to a path Part (special characters are escaped).
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>System.String.</returns>
        public static string PathPartName(string name)
        {
            if (name.Contains(CAEXPathBuilder.PathPartBegin))
            {
                name = name.Replace(CAEXPathBuilder.PathPartBegin, CAEXPathBuilder.PathPartBeginInName);
            }

            if (name.Contains(CAEXPathBuilder.PathPartEnd))
            {
                name = name.Replace(CAEXPathBuilder.PathPartEnd, CAEXPathBuilder.PathPartEndInName);
            }

            if (name.Contains(CAEXPathBuilder.AliasSeparator) ||
                name.Contains(CAEXPathBuilder.InterfaceSeparator) ||
                name.Contains(CAEXPathBuilder.AttributeSeparator) ||
                name.Contains(CAEXPathBuilder.ObjectSeparator))
            {
                name = CAEXPathBuilder.PathPartBegin + name + CAEXPathBuilder.PathPartEnd;
            }
            return name;
        }

        /// <summary>
        ///    get all child Elements of the specified element with the specified name.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="Name">   The name.</param>
        /// <returns>IEnumerable&lt;XmlElement&gt;.</returns>
        public static IEnumerable<XmlElement> ChildElements(this XmlElement element, string Name)
        {
            var navigator = element.CreateNavigator();
            var iterator = navigator.SelectChildren(Name, string.Empty);

            while (iterator.MoveNext())
                yield return iterator.Current.UnderlyingObject as XmlElement;
        }

        /// <summary>
        ///    The ChildElements in the specified 'baseElement', which have the defined
        ///    ElementName are cloned, if there is not yet a child Element in the
        ///    specified 'cloneElement' with that ElementName and an equal AttributeValue
        ///    for the Attribute, specified with the 'AttributeName'.
        /// </summary>
        /// <param name="cloneElement"> The clone element.</param>
        /// <param name="baseElement">  The base element.</param>
        /// <param name="ElementName">  Name of the element.</param>
        /// <param name="AttributeName">Name of the attribute.</param>
        /// <returns>IEnumerable&lt;XmlElement&gt;.</returns>
        public static IEnumerable<XmlElement> CloneWhenNotDerived(this XmlElement cloneElement, XmlElement baseElement, string ElementName, string AttributeName)
        {
            foreach (XmlElement child in baseElement.ChildElements(ElementName))
            {
                if (!child.IsDerived(cloneElement, ElementName, AttributeName))
                {
                    yield return (XmlElement)child.Clone();
                }
            }
        }

        /// <summary>
        ///    Gets the attribute value.
        /// </summary>
        /// <param name="element">      The element.</param>
        /// <param name="AttributeName">The attribute name.</param>
        /// <returns>System.String.</returns>
        public static string GetAttributeValue(this XmlElement element, string AttributeName)
        {
            if (element.HasAttribute(AttributeName))
            {
                return element.Attributes[AttributeName].Value;
            }
            return string.Empty;
        }

        /// <summary>
        ///    Gets the referenced caex class from a class library, which is any of the
        ///    Libraries with the defined LibraryTagName in the Document
        /// </summary>
        /// <param name="element">            The element.</param>
        /// <param name="referencedClassPath">The referenced class path.</param>
        /// <param name="LibraryTagName">     The CAEX TagName of the Library.</param>
        /// <returns>The referenced XmlNode, if it is found or null if not.</returns>
        public static XmlNode GetReferencedCAEXClassFromClassLibrary(this XmlElement element, string referencedClassPath, string LibraryTagName)
        {
            // if tables are managed by an application, a fast access to the referenced
            // object is possible
            if (CAEXDocument.CurrentDocument != null && CAEXDocument.CurrentDocument.Tables.MangedByApplication)
            {
                if (CAEXDocument.CurrentDocument.Tables.PathTable.ContainsKey(referencedClassPath))
                {
                    return CAEXDocument.CurrentDocument.Tables.PathTable[referencedClassPath].FirstOrDefault();
                }
            }

            // if no tables are managed, the element is searched with an xpath search
            // string, defined by the name attributes
            var pathParts = referencedClassPath.Split(new char[] { CAEXPathBuilder.ObjectSeparator });
            string searchString = ".";
            string libName = string.Empty;

            if (pathParts.Length > 1)
            {
                libName = pathParts[0];

                // the library node is omitted, because we'll start the search with a
                // Library Node which is faster than searching the whole document
                for (int i = 1; i < pathParts.Length; i++)
                {
                    searchString = searchString + "/*[@Name='" + pathParts[i] + "']";
                }
            }

            // if no path separator is found, the Class has to be the ParentNode.
            else
            {
                var nameOfParent = element.CAEXNameOfParent();
                if (nameOfParent == pathParts[0])
                    return element.ParentNode;
                else
                    return null;
            }

            var caexFileNode = element.OwnerDocument.DocumentElement;
            var navigator = caexFileNode.CreateNavigator();
            var ClassLibraries = navigator.SelectChildren(LibraryTagName, string.Empty);

            while (ClassLibraries.MoveNext())
            {
                var ClassLib = ClassLibraries.Current.UnderlyingObject as XmlElement;
                if (ClassLib.GetAttributeValue("Name") == libName)
                {
                    // if there is another library with the same name, the search has to
                    // be continued if the node couldn't be selected in the first lib
                    var node = ClassLib.SelectSingleNode(searchString);
                    if (node != null)
                        return node;
                }
            }
            return null;
        }

        /// <summary>
        ///    Get the publicElementReferences if this is an internalLink
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        public static IEnumerable<string> publicElementReference(this XmlElement element)
        {
            yield return element.publicElementReferenceIninternalLink(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REF_PARTNER_SIDE_A);
            yield return element.publicElementReferenceIninternalLink(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REF_PARTNER_SIDE_B);
        }

        /// <summary>
        ///    Get the publicElementReferences if this is an internalLink
        /// </summary>
        /// <param name="element">                 The element.</param>
        /// <param name="nameOfReferenceAttribute">The Attribute Name of the Reference-Attribute</param>
        /// <returns>System.String.</returns>
        public static string publicElementReferenceIninternalLink(this XmlElement element, string nameOfReferenceAttribute)
        {
            if (element.IsInternalLink())
            {
                var reference = element.GetAttributeValue(nameOfReferenceAttribute);
                if (reference != string.Empty && reference.Contains(InternalLinkType.LINK_SEPARATOR))
                {
                    var names = CAEXPathBuilder.PathPartObjectNames(reference).ToArray();
                    if (names.Count() > 1)
                        return names[0];
                }
            }
            return string.Empty;
        }

        /// <summary>
        ///    Determines whether the Element is a CAEXClass: RoleClass, SystemUnitClass
        ///    or InterfaceClass
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if the specified element is a CAEXClass; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsCAEXClass(this XmlElement element)
        {
            return element.IsRoleClass() || element.IsInterfaceClass() || element.IsSystemUnitClass();
        }

        /// <summary>
        ///    Determines whether the specified elementWithDerivedAttributes has a child
        ///    Element, which has the same ElementName and AttributeValue for the named
        ///    Attribute as the specified elementWithAttribute
        /// </summary>
        /// <param name="elementWithAttribute">        
        ///    The element With Attribute which is tested for derivations.
        /// </param>
        /// <param name="elementWithDerivedAttributes">The element With Derived Attributes.</param>
        /// <param name="ElementName">                 Name of the element.</param>
        /// <param name="AttributeName">               Name of the attribute.</param>
        /// <returns>
        ///    <c>true</c> if the specified attribute element is derived; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsDerived(this XmlElement elementWithAttribute, XmlElement elementWithDerivedAttributes, string ElementName, string AttributeName)
        {
            string name = elementWithAttribute.GetAttributeValue(AttributeName);
            if (!string.IsNullOrEmpty(name))
                foreach (XmlElement attribute in elementWithDerivedAttributes.ChildElements(ElementName))
                {
                    if (attribute.GetAttributeValue(AttributeName) == name)
                        return true;
                }
            return false;
        }

        /// <summary>
        ///    Determines whether [is external interface] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is external interface] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsExternalInterface(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING;
        }

        /// <summary>
        /// Determines whether [is external reference] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns><c>true</c> if [is external reference] [the specified element]; otherwise, <c>false</c>.</returns>
        public static bool IsExternalReference(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.EXTERNALREFERENCE_STRING;
        }

#if ICMirror
        /// <summary>
        ///    Determines whether [is ic mirror] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is ic mirror] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsICMirror(this XmlElement element)
        {
            if (element.HasAttribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH))
            {
                return element.Attributes[CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH].Value.IsGUID();
            }
            return false;
        }
#endif

        /// <summary>
        ///    Determines whether [is ie mirror] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is ie mirror] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsIEMirror(this XmlElement element)
        {
            if (element.HasAttribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH))
            {
                return element.Attributes[CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH].Value.IsGUID();
            }
            return false;
        }

        /// <summary>
        ///    Determines whether [is interface class] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is interface class] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInterfaceClass(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING;
        }

        /// <summary>
        ///    Determines whether the specified element is an InternalElement.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if the specified element is an InternalElement; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInternalElement(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING;
        }

        /// <summary>
        ///    Determines whether the specified element is an internal link.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if the specified element is an internal link; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsInternalLink(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.INTERNALLINK_STRING;
        }

        /// <summary>
        ///    Determines whether [is role class] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is role class] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRoleClass(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.ROLECLASS_STRING;
        }

        /// <summary>
        ///    Determines whether [is role requirement] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is role requirement] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsRoleRequirement(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING;
        }

        /// <summary>
        ///    Determines whether [is supported role class] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is supported role class] [the specified element];
        ///    otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSupportedRoleClass(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING;
        }

        /// <summary>
        ///    Determines whether [is mapping element] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is mapping element] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsMappingElement(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING ||
                   element.Name == CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING;
        }

        /// <summary>
        ///    Determines whether [is system unit class] [the specified element].
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>
        ///    <c>true</c> if [is system unit class] [the specified element]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSystemUnitClass(this XmlElement element)
        {
            return element.Name == CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING;
        }

        /// <summary>
        ///    Get the ID of the Master publicElement if this is a Mirror.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        public static string publicElementMasterID(this XmlElement element)
        {
            if (element.HasAttribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH))
            {
                if (element.Attributes[CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH].Value.IsGUID())
                    return element.Attributes[CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH].Value;
            }
            return string.Empty;
        }

#if IC_Mirror
        /// <summary>
        ///    Get the ID of the Master ExternalInterface if this is a Mirror.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        public static string InterfaceClassMasterID(this XmlElement element)
        {
            if (element.HasAttribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH))
            {
                if (element.Attributes[CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH].Value.IsGUID())
                    return element.Attributes[CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH].Value;
            }
            return string.Empty;
        }
#endif

        /// <summary>
        ///    Get the RoleReference if this is a RoleRequirement or a SupportedRoleClass
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        public static string RoleReference(this XmlElement element)
        {
            if (element.IsRoleRequirement())
            {
                return element.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASEROLECLASSPATH);
            }

            if (element.IsSupportedRoleClass())
            {
                return element.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFROLECLASSPATH);
            }
            return string.Empty;
        }

        /// <summary>
        ///    Sets the attribute value.
        /// </summary>
        /// <param name="element">      The element.</param>
        /// <param name="AttributeName">The attribute name.</param>
        /// <param name="value">        The value.</param>
        public static void SetAttributeValue(this XmlElement element, string AttributeName, string value)
        {
            if (element.HasAttribute(AttributeName))
            {
                element.Attributes[AttributeName].Value = value;
            }
        }

        /// <summary>
        ///    Sets the or create attribute value.
        /// </summary>
        /// <param name="element">      The element.</param>
        /// <param name="AttributeName">The attribute name.</param>
        /// <param name="value">        The value.</param>
        public static void SetOrCreateAttributeValue(this XmlElement element, string AttributeName, string value)
        {
            if (!element.HasAttribute(AttributeName))
            {
                element.Attributes.Append(element.OwnerDocument.CreateAttribute(AttributeName));
            }
            element.Attributes[AttributeName].Value = value;
        }

        /// <summary>
        ///    Get the RoleReference if this is a RoleRequirement or a SupportedRoleClass
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>System.String.</returns>
        public static string SystemUnitClassReference(this XmlElement element)
        {
            if (element.IsInternalElement())
            {
                return element.GetAttributeValue(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH);
            }
            return string.Empty;
        }

        #endregion public Methods
    }
}
// ***********************************************************************
// Assembly         : AMLEditor
// Author           : Josef Prinz
// Created          : 01-23-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 01-22-2015
// ***********************************************************************
// <copyright file="TagnameFindViewModel.cs" organization="AutomationML e.V.">
//     Copyright (c) AutomationML e.V.  All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************

using Aml.Editor.MVVMBase;
using Aml.Engine.CAEX;
using Aml.Engine.Xml.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

/// <summary>
/// The ViewModels namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class TagnameFindViewModel.
/// </summary>
public class TagnameFindViewModel : ViewModelBase
{
    #region Public Constructors

    //static TagnameFindViewModel()
    //{
    //    Criticals.Add('Ü', "&Uuml;");
    //    Criticals.Add('ä', "&auml;");
    //    Criticals.Add('Ä', "&Auml;");
    //    Criticals.Add('ö', "&ouml;");
    //    Criticals.Add('Ö', "&Ouml;");
    //    Criticals.Add('ü', "&uuml;");
    //}

    /// <summary>
    ///     Initializes a new instance of the <see cref="TagnameFindViewModel" /> class.
    /// </summary>
    /// <param name="tagName">Name of the tag.</param>
    /// <param name="isObject"></param>
    public TagnameFindViewModel(string tagName, bool isObject)
    {
        TagName = tagName;
        DisplayName = tagName;
        IsObject = isObject;
        IsChecked = isObject;
    }

    #endregion Public Constructors

    #region Internal Properties

    internal bool IsObject { get; set; }

    #endregion Internal Properties

    #region Internal Methods

    /// <summary>
    ///     Query construct to search for an element in the tree, using the provided values.
    /// </summary>
    /// <param name="root"></param>
    /// <param name="onlyWords">if set to <c>true</c> [only words].</param>
    /// <param name="isCaseSensitive">if set to <c>true</c> [is case sensitive].</param>
    /// <param name="searchText">The search text.</param>
    /// <returns>System.String.</returns>
    internal IEnumerable<XElement> Query(XElement root, bool onlyWords, bool isCaseSensitive, string searchText)
    {
        Func<string, string, bool, bool> compare = onlyWords ? Equivalence : Contains;
        return string.IsNullOrEmpty(searchText)
            ? ([])
            : TagName switch
            {
                ILReference or
                    CAEX_CLASSModel_TagNames.INTERNALLINK_STRING =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.INTERNALLINK_STRING)).Where(lo =>
                        compare(lo?.Attribute("Name")?.Value, searchText, !isCaseSensitive)),

                CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING or
                    CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING or
                    CAEX_CLASSModel_TagNames.ROLECLASS_STRING or
                    CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING or
                    CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING or
                    CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING =>
                    root.Descendants(root.XName(TagName))
                        .Where(lo => compare(lo?.Attribute("Name")?.Value, searchText, !isCaseSensitive)),

                RRReference or
                    CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASEROLECLASSPATH)?.Value,
                            searchText, !isCaseSensitive)),

                SRReference or CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFROLECLASSPATH)?.Value, searchText,
                            !isCaseSensitive)),

                IEClassReference =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH)?.Value,
                            searchText, !isCaseSensitive)),

                EIClassReference =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH)?.Value, searchText,
                            !isCaseSensitive)),

                SUCInheritenceRel =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH)?.Value, searchText,
                            !isCaseSensitive)),

                RCInheritenceRel =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.ROLECLASS_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH)?.Value, searchText,
                            !isCaseSensitive)),

                ICInheritenceRel =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH)?.Value, searchText,
                            !isCaseSensitive)),

                ATInheritenceRel =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING)).Where(lo =>
                        compare(lo?.Attribute(CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFATTRIBUTETYPE)?.Value, searchText,
                            !isCaseSensitive)),


                ATvalue =>
                    root.Descendants(root.XName(CAEX_CLASSModel_TagNames.ATTRIBUTE_STRING)).Where(lo =>
                        lo.Element(lo.XName(CAEX_CLASSModel_TagNames.ATTRIBUTE_VALUE_STRING))?.Value == searchText),

                _ => []
            };
    }

    //        /// <summary>
    //        ///     xes the path query.
    //        /// </summary>
    //        /// <param name="onlyWords">if set to <c>true</c> [only words].</param>
    //        /// <param name="isCaseSensitive">if set to <c>true</c> [is case sensitive].</param>
    //        /// <param name="searchText">The search text.</param>
    //        /// <returns>System.String.</returns>
    //        internal string XPathQuery(bool onlyWords, bool isCaseSensitive, string searchText)
    //        {
    //            if (!isCaseSensitive)
    //            {
    //                searchText = searchText.ToUpper();
    //            }

    //            searchText = Purge(searchText);

    //            if (onlyWords)
    //            {
    //                searchText = ".//{0}[{1} = '" + searchText + "']";
    //            }
    //            else
    //            {
    //                searchText = ".//{0}[contains ({1}, '" + searchText + "')]";
    //            }

    //            if (!isCaseSensitive)
    //            {
    //                searchText = string.Format(searchText, "{0}",
    //                    @"translate({1},'abcdefghijklmnopqrstuvwxyz','ABCDEFGHIJKLMNOPQRSTUVWXYZ')");
    //            }

    //            return TagName switch
    //            {
    //                ILReference or CAEX_CLASSModel_TagNames.INTERNALLINK_STRING => string.Format(searchText,
    //                                        Queryname(CAEX_CLASSModel_TagNames.INTERNALLINK_STRING),
    //                                        "@Name"),
    //                CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING => string.Format(searchText,
    //       Queryname(CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING),
    //       "@Name"),
    //                CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING => string.Format(searchText,
    //       Queryname(CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING),
    //       "@Name"),
    //                CAEX_CLASSModel_TagNames.ROLECLASS_STRING => string.Format(searchText,
    //       Queryname(CAEX_CLASSModel_TagNames.ROLECLASS_STRING),
    //       "@Name"),
    //                CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING => string.Format(searchText,
    //       Queryname(CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING),
    //       "@Name"),
    //                CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING => string.Format(searchText,
    //       Queryname(CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING),
    //       "@Name"),
    //                CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING => string.Format(searchText,
    //       Queryname(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING),
    //       "@Name"),
    //                RRReference or CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING => string.Format(searchText,
    //       Queryname(CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING),
    //       "@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASEROLECLASSPATH),
    //                SRReference or CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING => string.Format(searchText,
    //Queryname(CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING),
    //"@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFROLECLASSPATH),
    //                IEClassReference => string.Format(searchText,
    //Queryname(CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING),
    //"@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASESYSTEMUNITPATH),
    //                EIClassReference => string.Format(searchText,
    //Queryname(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING),
    //"@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH),
    //                SUCInheritenceRel => string.Format(searchText,
    //Queryname(CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING),
    //"@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH),
    //                RCInheritenceRel => string.Format(searchText,
    //Queryname(CAEX_CLASSModel_TagNames.ROLECLASS_STRING),
    //"@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH),
    //                ICInheritenceRel => string.Format(searchText,
    //Queryname(CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING),
    //"@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFBASECLASSPATH),
    //                ATInheritenceRel => string.Format(searchText,
    //Queryname(CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING),
    //"@" + CAEX_CLASSModel_TagNames.ATTRIBUTE_NAME_REFATTRIBUTETYPE),
    //                _ => "",
    //            };
    //        }

    #endregion Internal Methods

    #region Public Fields

    /// <summary>
    ///     The AttributeType inheritence relation
    /// </summary>
    public const string ATInheritenceRel = "AttributeType reference to AttributeType (inheritance)";


    /// <summary>
    ///     The AttributeType inheritence relation
    /// </summary>
    public const string ATvalue = "Attribute value";

    /// <summary>
    ///     The ExternalInterface class reference
    /// </summary>
    public const string EIClassReference = "Element with reference to InterfaceClass";

    /// <summary>
    ///     The InterfaceClass inheritence relation
    /// </summary>
    public const string ICInheritenceRel = "InterfaceClass reference to InterfaceClass (inheritance)";

    /// <summary>
    ///     The InternalElement class reference
    /// </summary>
    public const string IEClassReference = "Element with reference to SystemUnitClass";

    /// <summary>
    ///     The InternalLink reference
    /// </summary>
    public const string ILReference = "Element with InternalLink";

    /// <summary>
    ///     The RoleClass inheritence relation
    /// </summary>
    public const string RCInheritenceRel = "RoleClass reference to RoleClass (inheritance)";

    /// <summary>
    ///     The RoleRequirements reference
    /// </summary>
    public const string RRReference = "Element with RoleRequirement";

    /// <summary>
    ///     The SupporteRoleClass reference
    /// </summary>
    public const string SRReference = "Element with SupportedRoleClass";

    /// <summary>
    ///     The SystemUnitClass inheritence relation
    /// </summary>
    public const string SUCInheritenceRel = "SystemUnitClass reference to SystemUnitClass (inheritance)";

    #endregion Public Fields

    #region Private Fields

    /// <summary>
    ///     <see cref="DisplayName" />
    /// </summary>
    private string _displayName;

    /// <summary>
    ///     <see cref="IsChecked" />
    /// </summary>
    private bool _isChecked;

    /// <summary>
    ///     <see cref="TagName" />
    /// </summary>
    private string _tagname;

    #endregion Private Fields

    #region Public Events

    ///// <summary>
    ///// Tritt ein, wenn sich ein Eigenschaftswert ändert.
    ///// </summary>
    //public event PropertyChangedEventHandler PropertyChanged;

    #endregion Public Events

    #region Public Properties

    /// <summary>
    ///     Gets and sets the DisplayName
    /// </summary>
    public string DisplayName
    {
        get => _displayName;
        set => Set(ref _displayName, value);
    }

    /// <summary>
    ///     Gets and sets the IsChecked
    /// </summary>
    public bool IsChecked
    {
        get => _isChecked;
        set => Set(ref _isChecked, value);
    }

    /// <summary>
    ///     Gets and sets the TagName
    /// </summary>
    /// <value>The name of the tag.</value>
    public string TagName
    {
        get => _tagname;
        set => Set(ref _tagname, value);
    }

    #endregion Public Properties

    #region Private Methods

    //private static string Purge(string text)
    //{
    //    return text;
    //}

    //private static string RemoveDiacritics(string accentedStr)
    //{
    //    byte[] tempBytes;
    //    tempBytes = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(accentedStr);
    //    string asciiStr = System.Text.Encoding.UTF8.GetString(tempBytes);

    //    return asciiStr;
    //}

    private bool Contains(string s1, string s2, bool ignoreCase)
    {
        return !string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2)
&& (ignoreCase ? s1.Contains(s2, StringComparison.OrdinalIgnoreCase) : s1.Contains(s2));
    }

    private bool Equivalence(string s1, string s2, bool ignoreCase)
    {
        return !string.IsNullOrEmpty(s1) && !string.IsNullOrEmpty(s2) && string.Compare(s1, s2, ignoreCase) == 0;
    }

    ///// <summary>
    ///// Called when [property changed].
    ///// </summary>
    ///// <param name="PropertyName">Name of the property.</param>
    //private void OnPropertyChanged(string PropertyName)
    //{
    //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
    //}

    private static string Queryname(string tagName) => "child::node()[local-name()='" + tagName + "']";

    #endregion Private Methods
}
// ***********************************************************************
// Assembly :
// Aml.Toolkit Author : Josef Prinz Created : 03-09-2015
//
// Last Modified By : Josef Prinz Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLLayout.cs" company="inpro">
//    Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary>
//    </summary>
// ***********************************************************************

using System.Collections.ObjectModel;
using System.Linq;
using Aml.Editor.MVVMBase;

/// <summary>
///    The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel;

/// <summary>
///     Class AMLLayout provides Properties which effect the display of AmlNodes <see cref="AMLNodeViewModel" />
/// </summary>
public class AMLLayout : ViewModelBase
{
    #region Public Enums

    /// <summary>
    ///     Enum classifying the jump mode for internallink line crossings
    /// </summary>
    public enum LineJumpModeEnum
    {
        /// <summary>
        ///     No jump in line crossing
        /// </summary>
        None,

        /// <summary>
        ///     jump over using an arc in line crossing
        /// </summary>
        JumpOver,

        /// <summary>
        ///     jump using a gap at line crossings
        /// </summary>
        JumpGap
    }

    #endregion Public Enums

    #region Public Constructors

    /// <summary>
    ///     Initializes static members of the <see cref="AMLLayout" /> class. The
    ///     Default behavior is, that all CAEX-Elements are visible.
    /// </summary>
    static AMLLayout()
    {
        DefaultLayout = new AMLLayout
        {
            NamesOfVisibleElements = new ObservableCollection<string>(
                AMLTreeViewTemplate.CompleteInstanceHierarchyTree.Concat(
                    AMLTreeViewTemplate.SimpleSystemUnitClassLibTree.Concat(
                        AMLTreeViewTemplate.SimpleRoleClassLibTree.Concat(
                            AMLTreeViewTemplate.InterfaceClassLibTree)))),
            MaxNameWidth = double.NaN,
            ShowAdditionalInformation = true,
            ShowClassReference = true,
            ShowRoleReference = true
        };
    }

    #endregion Public Constructors

    #region Public Methods

    /// <summary>
    ///     Clones a new layout from the default layout.
    /// </summary>
    /// <returns></returns>
    public static AMLLayout CloneFromDefault() =>
        new()
        {
            NamesOfVisibleElements = new ObservableCollection<string>(
                AMLTreeViewTemplate.CompleteInstanceHierarchyTree.Concat(
                    AMLTreeViewTemplate.SimpleSystemUnitClassLibTree.Concat(
                        AMLTreeViewTemplate.SimpleRoleClassLibTree.Concat(
                            AMLTreeViewTemplate.InterfaceClassLibTree)))),
            ShowAdditionalInformation = true,
            ShowClassReference = true,
            MaxNameWidth = double.NaN,
            ShowRoleReference = true
        };

    #endregion Public Methods

    #region Public Fields

    /// <summary>
    ///     Layout property, used to display the interface grouping node.
    /// </summary>
    public const string SET_INTERFACE_GROUPING = "ShowVirtualInterfaceGroupNode";

    /// <summary>
    ///     Layout property, used to display the role grouping node.
    /// </summary>
    public const string SET_ROLE_GROUPING = "ShowVirtualRoleGroupNode";

    #endregion Public Fields

    #region Private Fields

    /// <summary>
    ///     <see cref="AlwaysExpandLink" />
    /// </summary>
    private bool _alwaysExpandLink;

    /// <summary>
    ///     <see cref="DrawColoredLines" />
    /// </summary>
    private bool _drawColoredLines;

    /// <summary>
    ///     <see cref="DrawDashedLines" />
    /// </summary>
    private bool _drawDashedLines;

    /// <summary>
    ///     <see cref="HideExpander" />
    /// </summary>
    private bool _hideExpander;

    /// <summary>
    ///     <see cref="HideInfo" />
    /// </summary>
    private bool _hideInfo;

    /// <summary>
    ///     <see cref="HorizontalLineOffset" />
    /// </summary>
    private int _horizontalLineOffset;

    /// <summary>
    ///     <see cref="JumpMode" />
    /// </summary>
    private LineJumpModeEnum _jumpMode;

    /// <summary>
    ///     <see cref="MaxNameWidth" />
    /// </summary>
    private double _maxNameWidth;

    /// <summary>
    ///     <see cref="NamesOfVisibleElements" />
    /// </summary>
    private ObservableCollection<string> _namesOfVisibleElements;

    /// <summary>
    ///     <see cref="ShowAdditionalInformation" />
    /// </summary>
    private bool _showAdditionalInformation;

    /// <summary>
    ///     <see cref="ResolveExternals" />
    /// </summary>
    private bool _resolveExternals;

    /// <summary>
    ///     <see cref="ShowAttributeValue" />
    /// </summary>
    private bool _showAttributeValue;

    /// <summary>
    ///     <see cref="ShowClassReference" />
    /// </summary>
    private bool _showClassReference;

    /// <summary>
    ///     <see cref="ShowFullClassPath" />
    /// </summary>
    private bool _showFullClassPath;

    /// <summary>
    ///     <see cref="ShowInterfaceGrouping" />
    /// </summary>
    private bool _showInterfaceGrouping;

    /// <summary>
    ///     <see cref="ShowLinkLines" />
    /// </summary>
    private bool _showLinkLInes;

    /// <summary>
    ///     <see cref="ShowRoleGrouping" />
    /// </summary>
    private bool _showRoleGrouping;

    /// <summary>
    ///     <see cref="ShowRoleReference" />
    /// </summary>
    private bool _showRoleReference;

    /// <summary>
    ///     <see cref="ShowRoleReqNodes" />
    /// </summary>
    private bool _showRoleReqNodes;

    /// <summary>
    ///     <see cref="VerticalLineOffset" />
    /// </summary>
    private double _verticalLineOffset;

    private bool _showMirrorTree;
    private bool _showLinkCardinality;

    #endregion Private Fields

    #region Public Properties

    /// <summary>
    ///     The default layout, which sets the visual properties of the layout to
    ///     default values
    /// </summary>
    public static AMLLayout DefaultLayout { get; set; }

    /// <summary>
    ///     Gets and sets the AlwaysExpandLink
    /// </summary>
    public bool AlwaysExpandLink
    {
        get => _alwaysExpandLink;
        set => Set(ref _alwaysExpandLink, value);
    }

    /// <summary>
    ///     Gets and sets the DrawColoredLines
    /// </summary>
    public bool DrawColoredLines
    {
        get => _drawColoredLines;
        set => Set(ref _drawColoredLines, value);
    }


    /// <summary>
    ///     Gets and sets the DrawDashedLines
    /// </summary>
    public bool DrawDashedLines
    {
        get => _drawDashedLines;
        set => Set(ref _drawDashedLines, value);
    }

    /// <summary>
    ///     Gets and sets the HideExpander
    /// </summary>
    public bool HideExpander
    {
        get => _hideExpander;
        set
        {
            if (Set(ref _hideExpander, value))
            {
            }
        }
    }

    /// <summary>
    ///     Gets and sets the HideInfo
    /// </summary>
    public bool HideInfo
    {
        get => _hideInfo;
        set => Set(ref _hideInfo, value);
    }

    /// <summary>
    ///     Gets and sets the HorizontalLineOffset
    /// </summary>
    public int HorizontalLineOffset
    {
        get => _horizontalLineOffset;
        set => Set(ref _horizontalLineOffset, value);
    }

    /// <summary>
    ///     Gets and sets the JumpMode
    /// </summary>
    public LineJumpModeEnum JumpMode
    {
        get => _jumpMode;
        set => Set(ref _jumpMode, value);
    }

    /// <summary>
    ///     Gets and sets the MaxNameWidth
    /// </summary>
    public double MaxNameWidth
    {
        get => _maxNameWidth;
        set => Set(ref _maxNameWidth, value);
    }

    /// <summary>
    ///     Gets and sets the NamesOfVisibleElements. This Collection may be used as a
    ///     filter to show/hide Elements with specific names. If the Collection
    ///     contains a name, the Element gets visible. If the Name is removed from the
    ///     collection, the element is hidden.
    /// </summary>
    /// <value>The names of visible elements.</value>
    public ObservableCollection<string> NamesOfVisibleElements
    {
        get => _namesOfVisibleElements;
        set => Set(ref _namesOfVisibleElements, value);
    }

    /// <summary>
    ///     Gets and sets the ShowAdditionalInformation
    /// </summary>
    public bool ShowAdditionalInformation
    {
        get => _showAdditionalInformation;
        set => Set(ref _showAdditionalInformation, value);
    }

    /// <summary>
    ///     Gets or sets a value indicating whether externals should be resolved in tree views.
    /// </summary>
    /// <value>
    ///     <c>true</c> if [resolve externals]; otherwise, <c>false</c>.
    /// </value>
    public bool ResolveExternals
    {
        get => _resolveExternals;
        set => Set(ref _resolveExternals, value);
    }

    /// <summary>
    ///     Gets and sets the ShowAttributeValue
    /// </summary>
    public bool ShowAttributeValue
    {
        get => _showAttributeValue;
        set => Set(ref _showAttributeValue, value);
    }

    /// <summary>
    ///     Gets and sets the ShowClassReference. Setting this Option to true, makes
    ///     the ClassReference of an Element visible <see cref="AMLNodeWithClassReference.ClassReference" />
    /// </summary>
    /// <value>
    ///     <c>true</c> the ClassReference becomes visible and invisible if <c>false</c>.
    /// </value>
    public bool ShowClassReference
    {
        get => _showClassReference;
        set => Set(ref _showClassReference, value);
    }



    /// <summary>
    ///  <see cref="ShowClassVersion"/>
    /// </summary>    
    private bool _showClassVersion;

    /// <summary>
    ///  Gets and sets the ShowClassVersion
    /// </summary>
    public bool ShowClassVersion
    {
        get => _showClassVersion;
        set => Set(ref _showClassVersion, value);
    }


    /// <summary>
    ///     Gets and sets the ShowFullClassPath
    /// </summary>
    public bool ShowFullClassPath
    {
        get => _showFullClassPath;
        set => Set(ref _showFullClassPath, value);
    }

    /// <summary>
    ///     Gets and sets the ShowInterfaceGrouping
    /// </summary>
    public bool ShowInterfaceGrouping
    {
        get => _showInterfaceGrouping;
        set => Set(ref _showInterfaceGrouping, value);
    }

    /// <summary>
    ///     Gets and sets the ShowLinkLines
    /// </summary>
    public bool ShowLinkLines
    {
        get => _showLinkLInes;
        set => Set(ref _showLinkLInes, value);
    }

    internal void Update(AMLLayout layout)
    {
        ShowAdditionalInformation = layout.ShowAdditionalInformation;
        ShowAttributeValue = layout.ShowAttributeValue;
        ShowClassReference = layout.ShowClassReference;
        ShowClassVersion = layout.ShowClassVersion;
        ShowFullClassPath = layout.ShowFullClassPath;
        ShowInterfaceGrouping = layout.ShowInterfaceGrouping;
        ShowLinkLines = layout.ShowLinkLines;
        ShowRoleGrouping = layout.ShowRoleGrouping;
        ShowRoleReference = layout.ShowRoleReference;
        ShowRoleReqNodes = layout.ShowRoleReqNodes;
        VerticalLineOffset = layout.VerticalLineOffset;
        HorizontalLineOffset = layout.HorizontalLineOffset;
        DrawColoredLines = layout.DrawColoredLines;
        DrawDashedLines = layout.DrawDashedLines;
        AlwaysExpandLink = layout.AlwaysExpandLink;
        HideExpander = layout.HideExpander;
        HideInfo = layout.HideInfo;
        ResolveExternals = layout.ResolveExternals;
        JumpMode = layout.JumpMode;
        NamesOfVisibleElements = layout.NamesOfVisibleElements;
        MaxNameWidth = layout.MaxNameWidth;
    }

    /// <summary>
    ///     Gets and sets the ShowRoleGrouping
    /// </summary>
    public bool ShowRoleGrouping
    {
        get => _showRoleGrouping;
        set => Set(ref _showRoleGrouping, value);
    }

    /// <summary>
    ///     Gets and sets the ShowRoleReference. Setting this Option to true, makes the
    ///     ClassReference of an Element visible <see cref="AMLNodeWithClassAndRoleReference.RoleReference" />
    /// </summary>
    /// <value>
    ///     <c>true</c> the RoleReference becomes visible and invisible if <c>false</c>.
    /// </value>
    public bool ShowRoleReference
    {
        get => _showRoleReference;
        set => Set(ref _showRoleReference, value);
    }

    /// <summary>
    ///     Gets and sets the ShowRoleReqNodes
    /// </summary>
    public bool ShowRoleReqNodes
    {
        get => _showRoleReqNodes;
        set => Set(ref _showRoleReqNodes, value);
    }

    /// <summary>
    ///     Gets and sets the ShowMirrorTree
    /// </summary>
    public bool ShowMirrorTree
    {
        get => _showMirrorTree;
        set => Set(ref _showMirrorTree, value);
    }

    /// <summary>
    ///     Gets and sets the ShowLinkCardianlity
    /// </summary>
    public bool ShowLinkCardinality
    {
        get => _showLinkCardinality;
        set => Set(ref _showLinkCardinality, value);
    }


    /// <summary>
    ///     Gets and sets the VerticalLineOffset
    /// </summary>
    public double VerticalLineOffset
    {
        get => _verticalLineOffset;
        set => Set(ref _verticalLineOffset, value);
    }


    /// <summary>
    ///     Copies this instance.
    /// </summary>
    /// <returns></returns>
    public AMLLayout Copy()
    {
        var layout = (AMLLayout)MemberwiseClone();
        layout.NamesOfVisibleElements = NamesOfVisibleElements;

        return layout;
    }

    #endregion Public Properties
}
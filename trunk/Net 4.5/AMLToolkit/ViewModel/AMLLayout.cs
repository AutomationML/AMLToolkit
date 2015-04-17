// ***********************************************************************
// Assembly         : AMLToolkit
// Author           : Josef Prinz
// Created          : 03-09-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 03-09-2015
// ***********************************************************************
// <copyright file="AMLLayout.cs" company="inpro">
//     Copyright (c) AutomationML e.V.. All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System.Collections.ObjectModel;
using System.Linq;
using CAEX_ClassModel;
using GalaSoft.MvvmLight;

/// <summary>
/// The ViewModel namespace.
/// </summary>
namespace AMLToolkit.ViewModel
{
    /// <summary>
    /// Class AMLLayout provides Properties which effect the display of AmlNodes <see cref="AMLNodeViewModel"/>
    /// </summary>
    public class AMLLayout : ViewModelBase
    {
        #region Public Fields

        /// <summary>
        /// The default layout, which sets the visual properties of the layout to default values
        /// </summary>
        public static AMLLayout DefaultLayout;

        #endregion Public Fields

        #region Private Fields

        /// <summary>
        /// <see cref="NamesOfVisibleElements" />
        /// </summary>
        private ObservableCollection<string> _namesOfVisibleElements;

        /// <summary>
        /// <see cref="ShowClassReference" />
        /// </summary>
        private bool _showClassReference;

        /// <summary>
        /// <see cref="ShowRoleReference" />
        /// </summary>
        private bool _showRoleReference;

        #endregion Private Fields

        #region Public Constructors

        /// <summary>
        /// Initializes static members of the <see cref="AMLLayout"/> class. The Default behavior is, that all CAEX-Elements are visible.
        /// </summary>
        static AMLLayout()
        {
            DefaultLayout = new AMLLayout();
            DefaultLayout.NamesOfVisibleElements = new ObservableCollection<string>(
                AMLTreeViewTemplate.CompleteInstanceHierarchyTree.Concat(
                AMLTreeViewTemplate.SimpleSystemUnitClassLibTree.Concat(
                AMLTreeViewTemplate.SimpleRoleClassLibTree.Concat(
                AMLTreeViewTemplate.InterfaceClassLibTree))));
            DefaultLayout.ShowClassReference = true;
            DefaultLayout.ShowRoleReference = true;
        }

        #endregion Public Constructors

        #region Public Properties

        /// <summary>
        /// Gets and sets the NamesOfVisibleElements. This Collection may be used as a filter to show/hide Elements with 
        /// specific names. If the Collection contains a name, the Element gets visible. If the Name is removed from the
        /// collection, the element is hidden. <see cref="AMLNodeViewModel.ApplyFilterWithName"/>
        /// </summary>
        /// <value>The names of visible elements.</value>
        public ObservableCollection<string> NamesOfVisibleElements
        {
            get
            {
                return _namesOfVisibleElements;
            }
            set
            {
                if (_namesOfVisibleElements != value)
                {
                    _namesOfVisibleElements = value; base.RaisePropertyChanged(() => NamesOfVisibleElements);
                }
            }
        }

        /// <summary>
        /// Gets and sets the ShowClassReference. Setting this Option to true, makes the ClassReference
        /// of an Element visible <see cref="AMLNodeWithClassReference.ClassReference"/> 
        /// </summary>
        /// <value><c>true</c> the ClassReference becomes visible and invisible if <c>false</c>.</value>
        public bool ShowClassReference
        {
            get
            {
                return _showClassReference;
            }
            set
            {
                if (_showClassReference != value)
                {
                    _showClassReference = value; base.RaisePropertyChanged(() => ShowClassReference);
                }
            }
        }

        /// <summary>
        /// Gets and sets the ShowRoleReference. Setting this Option to true, makes the ClassReference
        /// of an Element visible <see cref="AMLNodeWithClassAndRoleReference.RoleReference"/> 
        /// </summary>
        /// <value><c>true</c> the RoleReference becomes visible and invisible if <c>false</c>.</value>
        public bool ShowRoleReference
        {
            get
            {
                return _showRoleReference;
            }
            set
            {
                if (_showRoleReference != value)
                {
                    _showRoleReference = value; base.RaisePropertyChanged(() => ShowRoleReference);
                }
            }
        }

        #endregion Public Properties
    }
}
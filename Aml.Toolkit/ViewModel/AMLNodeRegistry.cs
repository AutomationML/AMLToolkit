// ***********************************************************************
// Assembly         : Aml.Toolkit
// Author           : Josef Prinz
// Created          : 03-10-2015
//
// Last Modified By : Josef Prinz
// Last Modified On : 01-18-2017
// ***********************************************************************
// <copyright file="AMLNodeRegistry.cs" company="AutomationML e.V.">
//     Copyright © AutomationML e.V. 2015
// </copyright>
// <summary></summary>
// ***********************************************************************

using Aml.Engine.CAEX;
using System;
using System.Collections.Generic;
using System.Xml.Linq;

/// <summary>
///  The ViewModel namespace.
/// </summary>
namespace Aml.Toolkit.ViewModel
{
    /// <summary>
    ///     Class AMLNodeRegistry contains constructor data for nodes
    /// </summary>
    public class AMLNodeRegistry : Dictionary<string, NodeCreator>
    {
        #region Private Fields

        private static AMLNodeRegistry m_Singleton;

        #endregion Private Fields

        #region Private Constructors

        /// <summary>
        ///     Prevents a default instance of the <see cref="AMLNodeRegistry" /> class from
        ///     being created.
        /// </summary>
        private AMLNodeRegistry()
        {
        }

        #endregion Private Constructors

        #region Public Properties

        /// <summary>
        ///     Gets the static instance of the AMLNodeRegistry
        /// </summary>
        /// <value>
        ///     The instance.
        /// </value>
        public static AMLNodeRegistry Instance
        {
            get
            {
                if (m_Singleton != null)
                {
                    return m_Singleton;
                }

                var baseType = typeof(AMLNodeViewModel).GetConstructor(new[]
                    {typeof(AMLTreeViewModel), typeof(AMLNodeViewModel), typeof(XElement), typeof(bool)});
                var classAndRoleRefType = typeof(AMLNodeWithClassAndRoleReference).GetConstructor(new[]
                    {typeof(AMLTreeViewModel), typeof(AMLNodeViewModel), typeof(XElement), typeof(bool)});
                var classRefType = typeof(AMLNodeWithClassReference).GetConstructor(new[]
                    {typeof(AMLTreeViewModel), typeof(AMLNodeViewModel), typeof(XElement), typeof(bool)});
                var noNameType = typeof(AMLNodeWithoutName).GetConstructor(new[]
                    {typeof(AMLTreeViewModel), typeof(AMLNodeViewModel), typeof(XElement), typeof(bool)});
                var mappingType = typeof(AMLNodeMappingElement).GetConstructor(new[]
                    {typeof(AMLTreeViewModel), typeof(AMLNodeViewModel), typeof(XElement), typeof(bool)});
                var attType = typeof(AMLNodeAttribute).GetConstructor(new[]
                    {typeof(AMLTreeViewModel), typeof(AMLNodeViewModel), typeof(XElement), typeof(bool)});
                var inheritType = typeof(AMLNodeInheritable).GetConstructor(new[]
                    {typeof(AMLTreeViewModel), typeof(AMLNodeViewModel), typeof(XElement), typeof(bool)});

                m_Singleton = new AMLNodeRegistry
                {
                    {CAEX_CLASSModel_TagNames.ATTRIBUTE_STRING, NodeCreator.GetCreator(attType)},
                    // typeof(AMLNodeViewModel).GetConstructor(new[] {
                    // typeof(AMLTreeViewModel), typeof(AMLNodeAttribute),
                    // typeof(XElement), typeof(bool) }));

                    // registration of base Types
                    {CAEX_CLASSModel_TagNames.INSTANCEHIERARCHY_STRING, NodeCreator.GetCreator(baseType)},
                    {CAEX_CLASSModel_TagNames.SYSTEMUNITCLASSLIB_STRING, NodeCreator.GetCreator(baseType)},
                    {CAEX_CLASSModel_TagNames.ROLECLASSLIB_STRING, NodeCreator.GetCreator(baseType)},
                    {CAEX_CLASSModel_TagNames.INTERFACECLASSLIB_STRING, NodeCreator.GetCreator(baseType)},
                    {CAEX_CLASSModel_TagNames.INTERNALLINK_STRING, NodeCreator.GetCreator(baseType)},
                    {CAEX_CLASSModel_TagNames.ATTRIBUTETYPELIB_STRING, NodeCreator.GetCreator(baseType)},

                    // registration of class and Role Reference Types
                    {CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING, NodeCreator.GetCreator(classAndRoleRefType)},

                    // registration of class Reference Types
                    {CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING, NodeCreator.GetCreator(classAndRoleRefType)},
                    {CAEX_CLASSModel_TagNames.ATTRIBUTETYPE_STRING, NodeCreator.GetCreator(classRefType)},
                    {CAEX_CLASSModel_TagNames.ROLECLASS_STRING, NodeCreator.GetCreator(classRefType)},
                    {CAEX_CLASSModel_TagNames.INTERFACECLASS_STRING, NodeCreator.GetCreator(classRefType)},
                    {CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING, NodeCreator.GetCreator(inheritType)},

                    // registration of no Name Types
                    {CAEX_CLASSModel_TagNames.CAEX_FILE, NodeCreator.GetCreator(noNameType)},
                    {CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING, NodeCreator.GetCreator(noNameType)},
                    {CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING, NodeCreator.GetCreator(noNameType)},
                    {CAEX_CLASSModel_TagNames.MAPPINGOBJECT_STRING, NodeCreator.GetCreator(noNameType)},
                    {
                        CAEX_CLASSModel_TagNames.MAPPINGOBJECT_ATTRIBUTENAME_STRING,
                        NodeCreator.GetCreator(mappingType)
                    },
                    {
                        CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACEID_STRING,
                        NodeCreator.GetCreator(mappingType)
                    },
                    {
                        CAEX_CLASSModel_TagNames.MAPPINGOBJECT_INTERFACENAME_STRING,
                        NodeCreator.GetCreator(mappingType)
                    }
                };

                return m_Singleton;
            }
        }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        ///     Changes the registered constructor for the view model of a CAEX-TagName. The
        ///     defined Type should be derived from the registered type. A List of ViewModels,
        ///     which are registered for specific CAEX-TagNames is listed at the Class Documentation.
        /// </summary>
        /// <param name="tagName">
        ///     Name of the CAEX-Element-Tag, which is represented in the specified ViewModel.
        /// </param>
        /// <param name="viewModel">
        ///     The view model for the CAEX-Element.
        /// </param>
        public static void ChangeRegisteredViewModelForNode(string tagName, Type viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (string.IsNullOrEmpty(tagName))
            {
                throw new ArgumentException("tagName not set");
            }

            var viewModelConstructor = viewModel.GetConstructor(new[]
                {typeof(AMLTreeViewModel), viewModel, typeof(XElement), typeof(bool)});

            viewModelConstructor ??= viewModel.GetConstructor(new[] { viewModel, typeof(XElement), typeof(bool) });
            if (viewModelConstructor == null || !m_Singleton.ContainsKey(tagName))
            {
                return;
            }

            var registeredCreator = m_Singleton[tagName];
            if (viewModel.IsSubclassOf(registeredCreator.DeclaringType))
            {
                m_Singleton[tagName] = NodeCreator.GetCreator(viewModelConstructor);
            }

            //Action<int> del = viewModelConstructor.BindDelegate(typeof(Action<int>)) as Action<int>;
        }

        #endregion Public Methods

        //public static class ConstructorHelper
        //{
        //    private delegate Delegate CreateDelegateHandler(Type type, RuntimeMethodHandle handle);
        //    private static CreateDelegateHandler _createDelegate;

        //    static ConstructorHelper()
        //    {
        //        MethodInfo methodInfo =
        //            typeof(Delegate).GetMethod("CreateDelegate", BindingFlags.Static | BindingFlags.NonPublic, null,
        //            new Type[] { typeof(Type), typeof(object), typeof(RuntimeMethodHandle) }, null);
        //        _createDelegate = Delegate.CreateDelegate(typeof(CreateDelegateHandler), methodInfo) as CreateDelegateHandler;
        //    }
        //    public static Delegate BindDelegate(this ConstructorInfo constructor,
        //        Type delegateType)
        //    {
        //        return _createDelegate(delegateType, constructor.MethodHandle);
        //    }
        //}
    }

    /// <summary>
    ///  Class AMLNodeRegistry defines a dictionary, containing CAEX-TagNames and
    ///  associated ConstructionInfo for the CAEX-Element, identifiable by the
    ///  CAEX-TagName. The Class implements the Singleton Pattern. The Static <see
    ///  cref="AMLNodeRegistry.Instance"/> contains default ConstructionInfo for each
    ///  CAEX-Element. The Default Dictionary Entries may be changed by an application, if
    ///  a CAEX-Element has a special ViewModel with a special constructor. Currently, the
    ///  following associations are created for the static Instance:
    ///  <list type="table" keepSeeTags="true">
    ///   <listheader>
    ///    <term>CAEX-Element</term>
    ///    <description>Associated ViewModel-Constructor.&gt;</description>
    ///   </listheader>
    ///   <item>
    ///    <term>InstanceHierarchy</term>
    ///    <description>
    ///     Basic
    ///     ViewModel: <see cref="AMLNodeViewModel(AMLNodeViewModel, XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>SystemUnitClassLibrary</term>
    ///    <description>
    ///     Basic
    ///     ViewModel: <see cref="AMLNodeViewModel(AMLNodeViewModel, XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>RoleClassLibrary</term>
    ///    <description>
    ///     Basic
    ///     ViewModel: <see cref="AMLNodeViewModel(AMLNodeViewModel, XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>InterfaceClassLibrary</term>
    ///    <description>
    ///     Basic
    ///     ViewModel: <see cref="AMLNodeViewModel(AMLNodeViewModel, XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>InternalLink</term>
    ///    <description>
    ///     Basic
    ///     ViewModel: <see cref="AMLNodeViewModel(AMLNodeViewModel, XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>InternalElement [has Class- and Role-Reference]</term>
    ///    <description>
    ///     Derived ViewModel: <see
    ///     cref="AMLNodeWithClassAndRoleReference(AMLNodeViewModel, XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>SystemUnitClass [has Class-Reference]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithClassReference(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>RoleClass [has Class-Reference]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithClassReference(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>InterfaceClass [has Class-Reference]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithClassReference(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>ExternalInterface [has Class-Reference]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithClassReference(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>CaexFile [has no Name-Attribute]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithoutName(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>RoleRequirement [has no Name-Attribute]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithoutName(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>SupportedRoleClass [has no Name-Attribute]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithoutName(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>MappingObject [has no Name-Attribute]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithoutName(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>AttributeNameMapping [has no Name-Attribute]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithoutName(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///   <item>
    ///    <term>InterfaceNameMapping [has no Name-Attribute]</term>
    ///    <description>
    ///     Derived ViewModel: <see cref="AMLNodeWithoutName(AMLNodeViewModel,
    ///     XElement, bool)"/>
    ///    </description>
    ///   </item>
    ///  </list>
    /// </summary>
}
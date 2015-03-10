using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AMLEngineExtensions;
using AMLToolkit.Model;
using CAEX_ClassModel;

namespace AMLToolkit.ViewModel
{
    public class AMLNodeWithClassAndRoleReference: AMLNodeWithClassReference
    {
        public AMLNodeWithClassAndRoleReference(AMLNodeViewModel parent, System.Xml.XmlElement CaexNode, bool lazyLoadChildren) :
            base (parent, CaexNode, lazyLoadChildren)
        {
            RefreshNodeInformation();
        }

        /// <summary>
        ///  <see cref="RoleReference"/>
        /// </summary>    
        private string _roleReference;

        /// <summary>
        ///  Gets and sets the RoleReference
        /// </summary>
        public string RoleReference
        {
            get
            {
                return _roleReference;
            }
            set
            {
                if (_roleReference != value)
                {
                    _roleReference = value; base.RaisePropertyChanged(() => RoleReference);
                }
            }
        }


        /// <summary>
        /// Refreshes the node information. This Method can be overridden in derived classes. The Method
        /// should be called, if the CAEX-Elements Data has changed and the Changes should be visible in any
        /// View, that has a binding to this ViewModel.
        /// </summary>
        public override void RefreshNodeInformation()
        {
            base.RefreshNodeInformation();

            if (CAEXNode != null  && CAEXNode.HasChildNodes)
            {
                var role = CAEXNode.ChildElements(CAEX_CLASSModel_TagNames.ROLEREQUIREMENTS_STRING).FirstOrDefault();
                if (role != null)
                {
                    var reference = role.RoleReference();
                    if (!string.IsNullOrEmpty(reference))
                    {
                        this.RoleReference = System.IO.Path.GetFileNameWithoutExtension(reference);
                    }
                }
            }
        }
        
    }
}

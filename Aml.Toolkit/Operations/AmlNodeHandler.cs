using Aml.Toolkit.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aml.Toolkit.Operations
{
    /// <summary>
    /// Handlers, called on tree node click events
    /// </summary>
    public static class AmlNodeHandler
    {
        /// <summary>
        /// The node double click event handler
        /// </summary>
        public static Func<AMLNodeViewModel, bool> OnNodeDoubleClick { get; set; }
    }
}

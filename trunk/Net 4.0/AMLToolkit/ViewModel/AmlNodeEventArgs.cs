using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AMLToolkit.ViewModel
{
    public class AmlNodeEventArgs: EventArgs
    {
        public AmlNodeEventArgs (AMLNodeViewModel source)
            : base()
        {
            Source = source;
        }
        public AMLNodeViewModel Source { get; private set; }
    }
}

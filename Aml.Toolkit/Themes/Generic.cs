using Aml.Toolkit.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Aml.Toolkit.Themes
{
    public partial class  Generic
    {
        /// <summary>
        /// AMLNode double click event handler. This handler calls <see cref="Operations.AmlNodeHandler.OnNodeDoubleClick"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem t && t.DataContext is AMLNodeViewModel nodeViewModel)
            {
                e.Handled = Operations.AmlNodeHandler.OnNodeDoubleClick?.Invoke(nodeViewModel) == true;
            }
        }

    }
}

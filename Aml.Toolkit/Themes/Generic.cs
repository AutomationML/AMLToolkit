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
        public void OnMouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
        }

        public void OnMouseDoubleClick(object sender, RoutedEventArgs e)
        {
            if (sender is TreeViewItem t && t.DataContext is AMLNodeViewModel nodeViewModel)
            {
                e.Handled = Operations.AmlNodeHandler.OnNodeDoubleClick?.Invoke(nodeViewModel) == true;
            }
        }

    }
}

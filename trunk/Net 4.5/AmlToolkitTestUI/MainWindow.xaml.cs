using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;
using CAEX_ClassModel;

namespace AmlToolkitTestUI
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AMLToolkit.ViewModel.AMLLayout.DefaultLayout.ShowClassReference = false;
            AMLToolkit.ViewModel.AMLLayout.DefaultLayout.ShowRoleReference  = false;

            var path = System.IO.Path.Combine(Environment.CurrentDirectory, @"TestFile/Test1.aml");

            var doc = CAEXDocument.LoadFromFile(path);
            
            
            this.IHTree.TreeViewModel = new AMLToolkit.ViewModel.AMLTreeViewModel((XmlElement)doc.CAEXFile.Node, AMLToolkit.ViewModel.AMLTreeViewTemplate.CompleteInstanceHierarchyTree);
            this.ICTree.TreeViewModel = new AMLToolkit.ViewModel.AMLTreeViewModel((XmlElement)doc.CAEXFile.Node, AMLToolkit.ViewModel.AMLTreeViewTemplate.InterfaceClassLibTree);
            this.SUCTree.TreeViewModel = new AMLToolkit.ViewModel.AMLTreeViewModel((XmlElement)doc.CAEXFile.Node, AMLToolkit.ViewModel.AMLTreeViewTemplate.CompleteSystemUnitClassLibTree);
            this.RCTree.TreeViewModel = new AMLToolkit.ViewModel.AMLTreeViewModel((XmlElement)doc.CAEXFile.Node, AMLToolkit.ViewModel.AMLTreeViewTemplate.ExtendedRoleClassLibTree);

            // test layout options
            System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(testFilter);
            dispatcherTimer.Interval = new TimeSpan(0, 0, 12);
            //dispatcherTimer.Start();

            // test layout options
            System.Windows.Threading.DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer2.Tick += new EventHandler(testReferenceDisplay);
            dispatcherTimer2.Interval = new TimeSpan(0, 0, 7);
            //dispatcherTimer2.Start();
            
        }

        bool set = false;
        private void testReferenceDisplay(object sender, EventArgs e)
        {
            AMLToolkit.ViewModel.AMLLayout.DefaultLayout.ShowClassReference = set;
            AMLToolkit.ViewModel.AMLLayout.DefaultLayout.ShowRoleReference = set;

            set = !set; 
        }

        bool remove = true;

        private void testFilter(object sender, EventArgs e)
        {
            if (remove)
            {
                AMLToolkit.ViewModel.AMLLayout.DefaultLayout.NamesOfVisibleElements.Remove(CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING);
                AMLToolkit.ViewModel.AMLLayout.DefaultLayout.NamesOfVisibleElements.Remove(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING);
            }
            else
            {
                AMLToolkit.ViewModel.AMLLayout.DefaultLayout.NamesOfVisibleElements.Add(CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING);
                AMLToolkit.ViewModel.AMLLayout.DefaultLayout.NamesOfVisibleElements.Add(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING);
            }

            remove = !remove;
        }
    }
}

using Aml.Engine.CAEX;
using Aml.Engine.Services;
using Aml.Engine.Xml.Extensions;
using Aml.Toolkit.ViewModel;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Aml.Toolkit.TestUI
{
    /// <summary>
    ///    Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region Private Fields

        private CAEXDocument _doc;
        private bool remove = true;
        private bool set = false;

        #endregion Private Fields

        #region Public Constructors

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            UndoRedoService = UndoRedoService.Register();

            AMLLayout.DefaultLayout.ShowClassReference = false;
            AMLLayout.DefaultLayout.ShowRoleReference = false;

            var path = System.IO.Path.Combine(Environment.CurrentDirectory, @"TestFile/MasterDatei.aml");

            _doc = CAEXDocument.LoadFromFile(path);

            this.IHTree.IsMultipleSelection = false;
            this.IHTree.TreeViewModel = new AMLTreeViewModel(_doc.CAEXFile.Node, AMLTreeViewTemplate.CompleteInstanceHierarchyTree);
            this.ICTree.TreeViewModel = new AMLTreeViewModel(_doc.CAEXFile.Node, AMLTreeViewTemplate.InterfaceClassLibTree);
            this.SUCTree.TreeViewModel = new AMLTreeViewModel(_doc.CAEXFile.Node, AMLTreeViewTemplate.CompleteSystemUnitClassLibTree);
            this.RCTree.TreeViewModel = new AMLTreeViewModel(_doc.CAEXFile.Node, AMLTreeViewTemplate.ExtendedRoleClassLibTree);

            var model = new AMLTreeViewModel(_doc.CAEXFile.Node, AMLTreeViewTemplate.CompleteInstanceHierarchyTree);
           
            model.ExpandAllCommand.Execute (true);
            

            // test layout options
            //System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(TestFilter);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 12);
            //dispatcherTimer.Start();

            // test layout options
            //System.Windows.Threading.DispatcherTimer dispatcherTimer2 = new System.Windows.Threading.DispatcherTimer();
            //dispatcherTimer2.Tick += new EventHandler(TestReferenceDisplay);
            //dispatcherTimer2.Interval = new TimeSpan(0, 0, 7);
            //dispatcherTimer2.Start();

            //this.IHTree.AllowDrop = true;
            //this.SUCTree.AllowDrop = true;

            CanDragDropPredicate CanDragDrop = delegate (AMLTreeViewModel tree, AMLNodeViewModel source, AMLNodeViewModel target)
            {
                return (source.CAEXNode.Name == CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING &&
                        target.CAEXNode.Name == CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING) ||
                       (source.CAEXNode.Name == CAEX_CLASSModel_TagNames.INTERNALELEMENT_STRING &&
                        target.CAEXNode.Name == CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING) ||
                       (source.CAEXNode.Name == CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING &&
                        target.CAEXNode.Name == CAEX_CLASSModel_TagNames.SYSTEMUNITCLASS_STRING);
            };

            DoDragDropAction DoDragDrop = delegate (AMLTreeViewModel tree, AMLNodeViewModel source, AMLNodeViewModel target)
            {
                UndoRedoService.BeginTransaction(_doc, "DragDropAMLObject");
                UniqueNameService.Register();
                var caexSource = source.CAEXNode.CreateCAEXWrapper() as CAEXBasicObject;
                var copiedObject = caexSource.Copy();
                var caexTarget = target.CAEXNode.CreateCAEXWrapper() as CAEXBasicObject;

                switch (copiedObject)
                {
                    case InternalElementType ie:
                        caexTarget.Insert(ie, false);
                        break;

                    case SystemUnitFamilyType su:
                        caexTarget.Insert(su, false);
                        break;
                }

                UniqueNameService.UnRegister();
                UndoRedoService.EndTransaction(_doc);

                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanUndo"));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRedo"));
            };

            //IHTree.TreeViewModel.CanDragDrop = CanDragDrop;
            //IHTree.TreeViewModel.DoDragDrop = DoDragDrop;

            //SUCTree.TreeViewModel.DoDragDrop = DoDragDrop;
            //SUCTree.TreeViewModel.CanDragDrop = CanDragDrop;

            //IHTree.SelectedItemChanged += IHTree_SelectedItemChanged;
        }

        #endregion Public Constructors

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion Public Events

        #region Public Properties

        public bool CanRedo
        {
            get
            {
                return UndoRedoService.CanRedo(_doc);
            }
        }

        public bool CanUndo
        {
            get
            {
                return UndoRedoService.CanUndo(_doc);
            }
        }

        public UndoRedoService UndoRedoService { get; }

        #endregion Public Properties

        #region Private Methods

        private void IHTree_Loaded(object sender, RoutedEventArgs e)
        {
            //IHTree.TreeViewModel.ExpandAllCommand.Execute(IHTree.TreeViewModel.Root);
        }

        private void IHTree_SelectedItemChanged(object sender, AmlNodeEventArgs e)
        {
            if (e.Source.AdditionalInformation == null)
                e.Source.AdditionalInformation = " xx ";
            else
                e.Source.AdditionalInformation = null;
        }

        private void Redo_Click(object sender, RoutedEventArgs e)
        {
            if (UndoRedoService.CanRedo(_doc))
                UndoRedoService.Redo(_doc);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanUndo"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRedo"));
        }

        private void TestFilter(object sender, EventArgs e)
        {
            //if (remove)
            //{
            //    SUCTree.TreeViewModel.CAEXTagNames.Remove(CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING);
            //    SUCTree.TreeViewModel.CAEXTagNames.Remove(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING);
            //}
            //else
            //{
            //    SUCTree.TreeViewModel.CAEXTagNames.Add(CAEX_CLASSModel_TagNames.SUPPORTEDROLECLASS_STRING);
            //    SUCTree.TreeViewModel.CAEXTagNames.Add(CAEX_CLASSModel_TagNames.EXTERNALINTERFACE_STRING);
            //}

            //SUCTree.TreeViewModel.NodeFilters.Refresh();

            remove = !remove;
        }

        private void TestReferenceDisplay(object sender, EventArgs e)
        {
            AMLLayout.DefaultLayout.ShowClassReference = set;
            AMLLayout.DefaultLayout.ShowRoleReference = set;

            set = !set;
        }

        private void Undo_Click(object sender, RoutedEventArgs e)
        {
            if (UndoRedoService.CanUndo(_doc))
                UndoRedoService.Undo(_doc);

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanUndo"));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("CanRedo"));
        }

        #endregion Private Methods
    }
}
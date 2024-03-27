using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Elements_Copier
{
    public partial class StartWindow : Window
    {
        private Document doc;
        private UIDocument uidoc;

        private readonly StartWindowViewModel _viewModel;
        public StartWindow(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            InitializeComponent();
            _viewModel = new StartWindowViewModel();
            DataContext = _viewModel;
            _viewModel.RequestClose += SelectionWindow;
        }

        private void SelectionWindow(object sender, EventArgs e)
        {
            _viewModel.RequestClose -= SelectionWindow;

            //Close();
            int typeOfOperation = _viewModel.GetTypeOfOperation();
            int optionsOfOperation = _viewModel.GetOptionsOfOperation();
            var SelectionElementsWindow = new SelectionElementsWindow(doc, uidoc, typeOfOperation, optionsOfOperation);
            SelectionElementsWindow.Topmost = true;
            SelectionElementsWindow.Show();
        }
    }
}

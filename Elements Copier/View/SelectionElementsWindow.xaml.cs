using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System; 

namespace Elements_Copier
{
    public partial class SelectionElementsWindow : Window
    {
        private readonly SelectionElementsViewModel _viewModel;

        public SelectionElementsWindow(Document doc, UIDocument uidoc, int typeOfOperation)
        {
            InitializeComponent();
            _viewModel = new SelectionElementsViewModel(doc, uidoc, typeOfOperation);
            DataContext = _viewModel;
            _viewModel.RequestClose += CloseWindow;
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            _viewModel.RequestClose -= CloseWindow;
            
            Close();
            var selectedElementsData = _viewModel.GetSelectedElementsData();
            var copiedElementsWindow = new CopiedElementsWindow(selectedElementsData);
            copiedElementsWindow.Topmost = true;
            copiedElementsWindow.Show();
        }
    }
}


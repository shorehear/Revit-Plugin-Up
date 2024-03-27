using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System; 

namespace Elements_Copier
{
    public partial class SelectionElementsWindow : Window
    {
        private Document doc;
        private UIDocument uidoc;
        private readonly SelectionElementsViewModel _viewModel;

        public SelectionElementsWindow(Document doc, UIDocument uidoc, int typeOfOperation)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            InitializeComponent();
            _viewModel = new SelectionElementsViewModel(doc, uidoc, typeOfOperation);
            DataContext = _viewModel;
            _viewModel.SelectingOver += StartSettings;
        }

        private void StartSettings(object sender, EventArgs e)
        {
            //_viewModel.SelectingOver -= StartSettings;
            Close();
            var copiedElementsWindow = new CopiedElementsWindow(_viewModel.GetSelectedElementsData(), doc, uidoc);
            copiedElementsWindow.Topmost = true;
            copiedElementsWindow.Show();
        }
    }
}


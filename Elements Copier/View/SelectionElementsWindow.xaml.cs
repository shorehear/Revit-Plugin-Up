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
        private int optionsOfOperation;

        public SelectionElementsWindow(Document doc, UIDocument uidoc, int typeOfOperation, int optionsOfOperation)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.optionsOfOperation = optionsOfOperation;
            InitializeComponent();
            _viewModel = new SelectionElementsViewModel(doc, uidoc, typeOfOperation);
            DataContext = _viewModel;
            _viewModel.SelectingOver += StartSettings;
        }

        private void StartSettings(object sender, EventArgs e)
        {
            //_viewModel.SelectingOver -= StartSettings;
            Close();
            var copiedElementsWindow = new CopiedElementsWindow(_viewModel.GetSelectedElementsData(), optionsOfOperation, doc, uidoc);
            copiedElementsWindow.Topmost = true;
            copiedElementsWindow.Show();
        }
    }
}


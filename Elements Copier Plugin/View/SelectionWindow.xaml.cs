using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Plugin
{
    public partial class SelectionWindow : Window
    {
        private readonly SelectionElementsViewModel _viewModel;

        public event EventHandler CloseSelectionWindow;

        public SelectionWindow(Document doc, UIDocument uidoc)
        {
            _viewModel = new SelectionElementsViewModel(doc, uidoc);
            DataContext = _viewModel;
            InitializeComponent();
            _viewModel.StartElementsCopier += ThisStartElementsCopier;
        }

        private void ThisStartElementsCopier(object sender, EventArgs e)
        {
            try
            {
                CloseSelectionWindow?.Invoke(this, EventArgs.Empty);
                Close();
            }
            catch(Exception ex)
            {
                TaskDialog.Show("Ошибка", "xaml30" + ex.Message);
            }
        }
    }
}

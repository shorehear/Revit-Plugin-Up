using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ElementsCopier
{
    public partial class SelectionWindow : Window
    {
        private readonly SelectionElementsViewModel _viewModel;

        private Document doc;
        private UIDocument uidoc;
        public SelectionWindow(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;

            _viewModel = new SelectionElementsViewModel(doc, uidoc);
            DataContext = _viewModel;
            InitializeComponent();
            _viewModel.StartElementsCopier += ThisStartElementsCopier;
        }

        private void ThisStartElementsCopier(object sender, EventArgs e)
        {
            try
            {
                ElementsCopier elementsCopier = new ElementsCopier(doc, uidoc);
                elementsCopier.CopyElements();

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", "SelectionWindow.xaml.cs35\n" + ex.Message);
            }
            Close();
        }
    }
}

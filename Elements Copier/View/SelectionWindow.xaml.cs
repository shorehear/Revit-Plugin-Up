using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Controls;

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
            InitializeComponent();
            DataContext = _viewModel;
            _viewModel.StartElementsCopier += ThisStartElementsCopier;
            listbox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ThisStartElementsCopier(object sender, EventArgs e)
        {
            try
            {
                ElementsCopier elementsCopier = new ElementsCopier(doc);
                elementsCopier.CopyElements();
                _viewModel.SelectedElements.Clear();

            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ListBox listBox && listBox.SelectedItem is Element selectedElement)
            {
                var viewModel = DataContext as SelectionElementsViewModel;
                viewModel?.ListBox_SelectionChanged(selectedElement);
            }
        }

    }
}
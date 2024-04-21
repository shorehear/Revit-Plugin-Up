using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Controls;

namespace ElementsCopier
{
    public partial class SelectionWindow : Window
    {
        public SelectionElementsViewModel viewModel;

        private Document doc;
        private UIDocument uidoc;
        private PluginLogger logger;
        public SelectionWindow(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            viewModel = new SelectionElementsViewModel(doc, uidoc);
            logger = new PluginLogger(viewModel);

            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetLogger(logger);

            viewModel.StartElementsCopier += ThisStartElementsCopier;
            listbox.SelectionChanged += ListBox_SelectionChanged;
        }

        private void ThisStartElementsCopier(object sender, EventArgs e)
        {
            try
            {
                ElementsCopier elementsCopier = new ElementsCopier(doc, logger);
                elementsCopier.CopyElements();
                logger.LogInformation("Copying is being performed");

                viewModel.ClearAllData();
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
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

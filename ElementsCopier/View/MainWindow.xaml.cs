using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Controls;
using Revit.Async;

namespace ElementsCopier
{
    public partial class SelectionWindow : Window
    {
        public SelectionElementsViewModel viewModel;

        private PluginLogger logger;
        public SelectionWindow()
        {
            viewModel = new SelectionElementsViewModel();
            logger = new PluginLogger(viewModel);

            InitializeComponent();
            DataContext = viewModel;
            viewModel.SetLogger(logger);

            listbox.SelectionChanged += ListBox_SelectionChanged;
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

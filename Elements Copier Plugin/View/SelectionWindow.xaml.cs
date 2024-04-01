using System.Windows;
using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Plugin
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

            _viewModel.StartSettingsWindow += StartSettings;
        }

        private void StartSettings(object sender, EventArgs e)
        {
            var settingsWindow = new SettingsWindow(doc, uidoc);
            settingsWindow.Topmost = true;
            settingsWindow.Show();

            settingsWindow.CloseAllWindows += CloseAllWindowsHandler;
        }

        private void CloseAllWindowsHandler(object sender, EventArgs e)
        {
            Close();
        }
    }
}

using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace Plugin
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _viewModel;
        public event EventHandler CloseAllWindows;

        private Document doc;
        private UIDocument uidoc;
        public SettingsWindow(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;

            _viewModel = new SettingsViewModel(doc, uidoc);
            DataContext = _viewModel;
            InitializeComponent();
            _viewModel.EndSettingsWindow += EndSettings;
        }

        private void EndSettings(object sender, EventArgs e)
        {
            ElementsCopier elementsCopier = new ElementsCopier(doc, uidoc);
            CloseAllWindows?.Invoke(this, EventArgs.Empty);
            Close();
        }
    }
}

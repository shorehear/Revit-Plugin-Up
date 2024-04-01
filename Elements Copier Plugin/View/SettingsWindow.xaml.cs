using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TextBox = System.Windows.Controls.TextBox;

namespace Plugin
{
    public partial class SettingsWindow : Window
    {
        private readonly SettingsViewModel _viewModel;
        public SettingsWindow(Document doc, UIDocument uidoc)
        {
            _viewModel = new SettingsViewModel(doc, uidoc);
            DataContext = _viewModel;
            InitializeComponent();
        }
    }
}

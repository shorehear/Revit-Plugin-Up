using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Elements_Copier
{
    public partial class SelectionElementsWindow : Window
    {
        public SelectionElementsWindow(Document doc, UIDocument uidoc)
        {
            InitializeComponent();
            DataContext = new SelectionElementsViewModel(doc, uidoc);
        }
    }

}


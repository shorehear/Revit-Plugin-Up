using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;


namespace Elements_Copier
{
    public class SelectedElementsData
    {
        public Document doc;
        public UIDocument uidoc;
        public ObservableCollection<ElementId> SelectedElements { get; set; }
        public Line SelectedLine { get; set; }
        public SelectedElementsData(Document doc, UIDocument uidoc)
        {
            SelectedElements = new ObservableCollection<ElementId>();
            this.doc = doc;
            this.uidoc = uidoc;
        }
    }

}

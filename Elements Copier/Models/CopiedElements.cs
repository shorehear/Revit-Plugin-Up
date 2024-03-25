using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;

namespace Elements_Copier
{
    public class CopiedElements
    {
        public Document doc;
        public UIDocument uidoc;
        public ObservableCollection<ElementId> SelectedElements { get; set; }
        public Line SelectedLine { get; set; }

        public int AmountOfCopies;
        public double DistanceBetweenElements;
        public XYZ CoordinatesToCopy;
        public CopiedElements(SelectedElementsData selectedData)
        {
            doc = selectedData.doc;
            uidoc = selectedData.uidoc;
            SelectedElements = selectedData.SelectedElements;
            SelectedLine = selectedData.SelectedLine;
        }
    }
}

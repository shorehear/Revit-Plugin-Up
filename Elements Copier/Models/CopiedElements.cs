using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.ObjectModel;

namespace Elements_Copier
{
    public class CopiedElementsData
    {
        public ObservableCollection<ElementId> SelectedElements { get; set; }
        public Line SelectedLine { get; set; }

        public int AmountOfCopies;
        public double DistanceBetweenElements;
        public XYZ CoordinatesToCopy;
        public CopiedElementsData(SelectedElementsData selectedData)
        {            
            SelectedElements = new ObservableCollection<ElementId>(selectedData.SelectedElements);
            SelectedLine = selectedData.SelectedLine;
        }
    }
}

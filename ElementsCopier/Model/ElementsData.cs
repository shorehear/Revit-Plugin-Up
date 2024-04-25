using Autodesk.Revit.DB;
using System.Collections.ObjectModel;

namespace ElementsCopier
{
    public static class ElementsData
    {
        public static ObservableCollection<ElementId> SelectedElements { get; set; }
        public static ModelLine SelectedLine { get; set; }
        public static Line selectedLine { get; set; }
        public static XYZ SelectedPoint { get; set; }
        public static bool WithSourceElements { get; set; }
        public static double DistanceBetweenElements { get; set; }

        public static void ConvertValues()
        {
            DistanceBetweenElements /= 304.8;
            selectedLine = SelectedLine.GeometryCurve as Line;
        }
        public static int CountCopies { get; set; }
        public static void Initialize()
        {
            SelectedElements = new ObservableCollection<ElementId>();
            SelectedLine = null;
            SelectedPoint = null;
            WithSourceElements = false;
            DistanceBetweenElements = 0.0;
            CountCopies = 0;
        }
    }
}
using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ElementsCopier
{
    public static class ElementsData
    {
        public static IList<ElementId> SelectedElements { get; set; }
        public static ModelLine SelectedLine { get; set; }
        public static XYZ SelectedPoint { get; set; }
        public static bool NeedRotate { get; set; }
        public static bool WithSourceElements { get; set; }
        public static double DistanceBetweenElements { get; set; }
        public static int CountCopies { get; set; }
        public static void Initialize()
        {
            SelectedElements = new List<ElementId>();
            SelectedLine = null;
            SelectedPoint = null;
            NeedRotate = false;
            WithSourceElements = false;
            DistanceBetweenElements = 0.0;
            CountCopies = 0;
        }
    }
}

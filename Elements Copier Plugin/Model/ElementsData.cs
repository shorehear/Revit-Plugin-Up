using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace Plugin
{
    public static class ElementsData
    {
        public static IList<ElementId> SelectedElements { get; set; }
        public static ModelLine SelectedLine { get; set; }
        public static XYZ SelectedPoint { get; set; }
        public static bool NeedRotate { get; set; }
        public static bool SelectedAndCopiedElements { get; set; }
        public static double DistanceBetweenElements { get; set; }
        public static int CountElements { get; set; }


        public static void Initialize()
        {
            SelectedElements = new List<ElementId>();
            SelectedLine = null;
            SelectedPoint = null;
            NeedRotate = false;
            SelectedAndCopiedElements = false;
            DistanceBetweenElements = 0.0;
            CountElements = 1;
        }

        public static void DeleteObj()
        {
            SelectedElements = null;
            SelectedLine = null;
            SelectedPoint = null;
            NeedRotate = false;
            SelectedAndCopiedElements = false;
            DistanceBetweenElements = 0.0;
            CountElements = 1;
        }


    }
}

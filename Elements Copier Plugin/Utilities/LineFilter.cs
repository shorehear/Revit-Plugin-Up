using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace Plugin
{
    class LineSelectionFilter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element elem)
        {
            return elem is Line ? true : false;
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}

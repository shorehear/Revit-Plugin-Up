using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI.Selection;

namespace ElementsCopier
{
    class LineSelectionFilter : ISelectionFilter
    {
        bool ISelectionFilter.AllowElement(Element element)
        {
            return element is ModelLine ? true : false;
        }

        bool ISelectionFilter.AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
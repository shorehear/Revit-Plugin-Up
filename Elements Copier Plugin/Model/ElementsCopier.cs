using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Plugin
{
    public class ElementsCopier
    {
        private Document doc;
        private UIDocument uidoc;

        public ElementsCopier(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
        }

        public void CopyElements()
        {
            
        }

        private double GetRotationAngle(Element selectedElement, Line selectedLine)
        {
            LocationCurve locationCurve = selectedElement.Location as LocationCurve;
            if (locationCurve != null)
            {
                Curve elementCurve = locationCurve.Curve;
                XYZ elementDirection = (elementCurve.GetEndPoint(1) - elementCurve.GetEndPoint(0)).Normalize();

                XYZ lineDirection = (selectedLine.GetEndPoint(1) - selectedLine.GetEndPoint(0)).Normalize();

                double angle = elementDirection.AngleTo(lineDirection);

                return angle;
            }

            return 0.0;
        }

    }
}
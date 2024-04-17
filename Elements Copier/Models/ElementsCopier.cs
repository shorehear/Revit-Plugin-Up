using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Net;

namespace ElementsCopier
{
    public class ElementsCopier
    {
        private Document doc;
        private Line selectedLine;

        public ElementsCopier(Document doc)
        {
            this.doc = doc;
            ElementsData.GetDistanceInMM();

            selectedLine = (ElementsData.SelectedLine).GeometryCurve as Line;
        }

        public void CopyElements()
        {
            try
            {
                using (Transaction transaction = new Transaction(doc, "Копирование элементов вдоль линии"))
                {

                    XYZ translationVector = (selectedLine.GetEndPoint(0) - ElementsData.SelectedPoint);

                    for (int copyIndex = 0; copyIndex < ElementsData.CountCopies; copyIndex++)
                    {
                        foreach (ElementId elementId in ElementsData.SelectedElements)
                        {
                            ICollection<ElementId> newElementsIds = ElementTransformUtils.CopyElements(doc, new List<ElementId> { elementId }, translationVector);
                        }

                        if (ElementsData.CountCopies > 1 && ElementsData.DistanceBetweenElements != 0)
                        {
                            translationVector = translationVector.Add(selectedLine.Direction.Multiply(ElementsData.DistanceBetweenElements));
                        }
                    }
                    if (ElementsData.WithSourceElements)
                    {
                        ElementTransformUtils.MoveElements(doc, ElementsData.SelectedElements, translationVector);
                    }
                    transaction.Commit();
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", "55ElementsCopier.cs" + ex.Message);
            }
        }
    }
}
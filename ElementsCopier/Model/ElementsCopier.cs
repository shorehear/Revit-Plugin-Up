using System.Collections.Generic;
using Autodesk.Revit.DB;
using System;
using System.Threading.Tasks;
using Revit.Async;
using System.Runtime.CompilerServices;

namespace ElementsCopier
{
    public class ElementsCopier
    {
        private Document doc;
        private Line selectedLine;
        private PluginLogger logger;

        public ElementsCopier(Document doc, PluginLogger logger)
        {
            this.doc = doc;
            this.logger = logger;
            ElementsData.GetDistanceInMM();

            selectedLine = (ElementsData.SelectedLine).GeometryCurve as Line;
        }

        public void CopyElements()
        {
            try
            {
                using (Transaction transaction = new Transaction(doc, "Копирование элементов вдоль линии"))
                {
                    RevitTask.RunAsync(
                        (uiapp) => {
                        transaction.Start();
                        });
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
                    logger.LogInformation("The copy is completed.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }
    }
}
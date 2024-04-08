using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace ElementsCopier
{
    public class ElementsCopier
    {
        private Document doc;
        private UIDocument uidoc;
        private Line selectedLine;

        public ElementsCopier(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;

            if (ElementsData.SelectedLine != null)
            {
                selectedLine = (ElementsData.SelectedLine).GeometryCurve as Line;
            }
        }

        public void CopyElements()
        {
            var operationType = OperationType.GetOperationType();

            switch (operationType.Item1)
            {
                case PositionOperations.IHaveCopyPoint:
                    switch (operationType.Item2)
                    {
                        case MoveOperations.MoveOnlyCopiedElements:
                            PointAndCopy();
                            break;
                        case MoveOperations.MoveCopiedAndSelecedElements:
                            //
                            break;
                    }
                    break;

                case PositionOperations.IHaveLine:
                    switch (operationType.Item2)
                    {
                        case MoveOperations.MoveOnlyCopiedElements:
                            LineAndCopy();
                            break;
                        case MoveOperations.MoveCopiedAndSelecedElements:
                            //
                            break;
                    }
                    break;

                case PositionOperations.Error:
                    TaskDialog.Show("Ошибка", "Некорректно заданная операция");
                    break;
            }
        }

        #region Копирование по точке
        private void PointAndCopy()
        {
            try
            {
                Transaction transaction = new Transaction(doc, "Копирование элементов по точке");

                if (ElementsData.SelectedElements.Count > 0 && ElementsData.SelectedPoint != null)
                {
                    for (int copyIndex = 0; copyIndex < ElementsData.CountElements; copyIndex++)
                    {
                        XYZ translation = ElementsData.SelectedPoint;

                        foreach (ElementId elementId in ElementsData.SelectedElements)
                        {
                            ICollection<ElementId> newElementsIds = ElementTransformUtils.CopyElements(doc, new List<ElementId> { elementId }, translation);

                            if (newElementsIds != null && newElementsIds.Count > 0)
                            {
                                translation = ElementsData.SelectedPoint;
                            }
                            else
                            {
                                TaskDialog.Show("Ошибка", "Откат транзакции");
                                transaction.RollBack();
                                return;
                            }
                        }
                    }
                    TaskDialog.Show("Успешно", "Элементы скопированы");
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", "Произошла ошибка: " + ex.Message);
            }
        }
        #endregion

        private void LineAndCopy()
        {
            try
            {
                Transaction transaction = new Transaction(doc, "Копирование элементов по линии");

                if (ElementsData.SelectedElements.Count > 0 && selectedLine != null)
                {
                    for (int copyIndex = 0; copyIndex < ElementsData.CountElements; copyIndex++)
                    {
                        XYZ translation = selectedLine.GetEndPoint(0);

                        foreach (ElementId elementId in ElementsData.SelectedElements)
                        {
                            ICollection<ElementId> newElementsIds = ElementTransformUtils.CopyElements(doc, new List<ElementId> { elementId }, translation);

                            if (newElementsIds != null && newElementsIds.Count > 0)
                            {
                                translation = selectedLine.GetEndPoint(0);
                            }
                            else
                            {
                                TaskDialog.Show("Ошибка", "Откат транзакции");
                                transaction.RollBack();
                                return;
                            }
                        }
                    }

                    TaskDialog.Show("Успешно", "Элементы скопированы");
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", "Произошла ошибка: " + ex.Message);
            }
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
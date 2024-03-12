using Autodesk.Revit.DB;
using System.Collections.Generic;

namespace ElementsCopier
{
    public class ElementsCopier
    {
        private List<Element> selectedElements;
        private Line selectedLine;

        private Document doc;

        private XYZ coordinatesPoint;
        private double distance;
        private int quantity;

        public ElementsCopier(List<Element> selectedElements, XYZ coordinatesPoint, double distance, int quantity)
        {
            selectedLine = null;
            this.selectedElements = selectedElements;
            this.coordinatesPoint = coordinatesPoint;
            this.distance = distance;
            this.quantity = quantity;
        }


        public ElementsCopier(List<Element> selectedElements, Line selectedLine, XYZ coordinatesPoint, double distance, int quantity)
        {
            this.selectedLine = selectedLine;
            this.selectedElements = selectedElements;
            this.coordinatesPoint = coordinatesPoint;
            this.distance = distance;
            this.quantity = quantity;
        }

        public void CopyElements()
        {
            if (selectedElements != null)
            {
                Transaction transaction = new Transaction(doc, "Копирование элементов");
                if (transaction.Start() == TransactionStatus.Started)
                {
                    XYZ translation = new XYZ(distance, 0, 0);

                    if (selectedLine != null)
                    {
                        double rotationAngle = GetRotationAngle(selectedElements.First(), selectedLine);

                        foreach (Element selectedElement in selectedElements)
                        {
                            ICollection<ElementId> newElementIds = ElementTransformUtils.CopyElement(doc, selectedElement.Id, translation);

                            if (newElementIds.Count > 0)
                            {
                                Element newElement = doc.GetElement(newElementIds.First());
                                ElementTransformUtils.RotateElement(doc, newElement.Id, Line.CreateBound(new XYZ(0, 0, 0), XYZ.BasisZ), rotationAngle);

                                translation = translation.Add(new XYZ(distance, 0, 0));
                            }
                        }
                    }
                    else
                    {
                        foreach (Element selectedElement in selectedElements)
                        {
                            ICollection<ElementId> newElementIds = ElementTransformUtils.CopyElement(doc, selectedElement.Id, translation);

                            if (newElementIds.Count > 0)
                            {
                                translation = translation.Add(new XYZ(distance, 0, 0));
                            }
                        }
                    }

                    transaction.Commit();
                }
            }
        }
    }
}


/*
        public void CopyElements()
        {
            if (selectedElement != null && selectedLine != null)
            {
                Transaction transaction = new Transaction(doc, "Копирование и вращение элементов");
                if (transaction.Start() == TransactionStatus.Started)
                {
                    XYZ translation = new XYZ(DistanceBetweenElements, 0, 0);

                    double rotationAngle = GetRotationAngle(selectedElement, selectedLine);

                    for (int i = 0; i < AmountOfElements; i++)
                    {
                        ICollection<ElementId> newElementIds = ElementTransformUtils.CopyElement(doc, selectedElement.Id, translation);

                        if (newElementIds.Count > 0)
                        {
                            Element newElement = doc.GetElement(newElementIds.First());
                            ElementTransformUtils.RotateElement(doc, newElement.Id, Line.CreateBound(new XYZ(0, 0, 0), XYZ.BasisZ), rotationAngle);

                            translation = translation.Add(new XYZ(DistanceBetweenElements, 0, 0));
                        }
                    }
                    transaction.Commit();
                }
            }
            else if (selectedElement != null && selectedLine == null)
            {
                Transaction transaction = new Transaction(doc, "Копирование элементов");
                if (transaction.Start() == TransactionStatus.Started)
                {
                    XYZ translation = new XYZ(DistanceBetweenElements, 0, 0);

                    for (int i = 0; i < AmountOfElements; i++)
                    {
                        ICollection<ElementId> newElementIds = ElementTransformUtils.CopyElement(doc, selectedElement.Id, translation);

                        if (newElementIds.Count > 0)
                        {
                            translation = translation.Add(new XYZ(DistanceBetweenElements, 0, 0));
                        }
                    }
                    transaction.Commit();
                }
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


        public void MoveCopiedElements(XYZ position)
        {
            if (selectedElement != null && selectedLine != null)
            {
                Transaction transaction = new Transaction(doc, "Копирование, перемещение и вращение элементов");
                if (transaction.Start() == TransactionStatus.Started)
                {
                    XYZ translation = new XYZ(DistanceBetweenElements, 0, 0);
                    double rotationAngle = GetRotationAngle(selectedElement, selectedLine);

                    for (int i = 0; i < AmountOfElements; i++)
                    {
                        ICollection<ElementId> newElementIds = ElementTransformUtils.CopyElement(doc, selectedElement.Id, translation);

                        if (newElementIds != null && newElementIds.Count > 0)
                        {
                            ElementId newElementId = newElementIds.FirstOrDefault();
                            Element newElement = doc.GetElement(newElementId);
                            ElementTransformUtils.RotateElement(doc, newElement.Id, Line.CreateBound(new XYZ(0, 0, 0), XYZ.BasisZ), rotationAngle);
                            ElementTransformUtils.MoveElement(doc, newElement.Id, position + translation);
                        }

                        translation = translation.Add(new XYZ(DistanceBetweenElements, 0, 0));
                    }

                    transaction.Commit();
                }
            }
            else if (selectedElement != null && selectedLine == null)
            {

                Transaction transaction = new Transaction(doc, "Копирование и перемещение элементов");
                if (transaction.Start() == TransactionStatus.Started)
                {
                    XYZ translation = new XYZ(0, 0, 0);

                    for (int i = 0; i < AmountOfElements; i++)
                    {
                        ICollection<ElementId> newElementIds = ElementTransformUtils.CopyElement(doc, selectedElement.Id, translation);

                        if (newElementIds != null && newElementIds.Count > 0)
                        {
                            ElementId newElementId = newElementIds.FirstOrDefault();
                            Element newElement = doc.GetElement(newElementId);
                            ElementTransformUtils.MoveElement(doc, newElement.Id, position + translation);
                        }

                        translation = translation.Add(new XYZ(DistanceBetweenElements, 0, 0));
                    }

                    transaction.Commit();
                }
            }
        }
    }
}*/
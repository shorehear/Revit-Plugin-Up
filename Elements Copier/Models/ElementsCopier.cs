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
            var operationType = CopySettings.GetCopySettingsType();

            switch (operationType)
            {
                case Operation.Default:
                    CopyToLine();
                    break;
                case Operation.NeedRotate:
                    break;
                case Operation.WithSourceElements:
                    break;
                case Operation.NeedRotateWithSourceElements:
                    break;
                default:
                    TaskDialog.Show("Ошибка", "Не удается определить тип исполняемой операции, будет выполнена операция по умолчанию.");
                    break;
            }
        }

        #region Operation.Default
        private void CopyToLine()
        {
            try
            {
                Transaction transaction = new Transaction(doc, "Копирование элементов по линии");
                XYZ translationVector = selectedLine.GetEndPoint(0) - ElementsData.SelectedPoint;

                for (int copyIndex = 0; copyIndex < ElementsData.CountCopies; copyIndex++)
                {
                    for (int i = 0; i < ElementsData.SelectedElements.Count; i++)
                    {
                        ElementId elementId = ElementsData.SelectedElements[i];
                        ICollection<ElementId> newElementsIds = ElementTransformUtils.CopyElements(doc, new List<ElementId> { elementId }, translationVector);
                    }
                }
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
            }
        }
        #endregion

    }
}
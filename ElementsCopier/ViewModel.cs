using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System;
using System.Collections.Generic;

namespace ElementsCopier
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Plugin : IExternalCommand
    {
        List<Element> selectedElements = new List<Element>();
        Line selectedLine = null;
        Document doc = null;

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                doc = uidoc.Document;

                SelectionWindow selectElementsWindow = new SelectionWindow(doc, uidoc, selectedElements);
                selectElementsWindow.Show();
                selectElementsWindow.SettingsClosedWithSettings += SelectionWindow_SettingsClosedWithSettings; //окно выбора отдаст значения на парсинг как только окно настроек закроется
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
            return Result.Succeeded;
        }

        private string missingDataMessage = "Не удалось получить все необходимые данные для копирования элементов.";
        private string invalidDataMessage = "Не удалось получить данные о расстоянии и/или количестве копий.";
        private string noElementsMessage = "Не выбраны элементы для копирования.";
        private string noLineMessage = "Не выбрана линия для копирования элементов.";
        private string noDocumentMessage = "Не удалось получить доступ к документу.";

        private void SelectionWindow_SettingsClosedWithSettings(object sender, Object[] settings)
        {
            if (settings.Length < 6)
            {
                TaskDialog.Show("Ошибка", missingDataMessage);
                return;
            }

            List<Element> selectedElements;
            Line selectedLine;
            XYZ coordinatesPoint;
            double distance;
            int quantity;
            Document doc;

            if (!RetrieveSettings(settings, out selectedElements, out selectedLine, out coordinatesPoint, out distance, out quantity, out doc))
                return;

            new ElementsCopier(selectedElements, selectedLine, coordinatesPoint, distance, quantity, doc);
        }

        private bool RetrieveSettings(object[] settings, out List<Element> selectedElements, out Line selectedLine, out XYZ coordinatesPoint, out double distance, out int quantity, out Document doc)
        {
            selectedElements = settings[0] as List<Element>;
            selectedLine = settings[1] as Line;
            coordinatesPoint = settings[2] as XYZ;
            distance = (double)settings[3]; 
            quantity = (int)settings[4];
            doc = settings[5] as Document;

            if (!double.TryParse(settings[3].ToString(), out distance) || !int.TryParse(settings[4].ToString(), out quantity))
            {
                TaskDialog.Show("Ошибка", invalidDataMessage);
                return false;
            }

            if (selectedElements == null || selectedElements.Count == 0)
            {
                TaskDialog.Show("Ошибка", noElementsMessage);
                return false;
            }

            if (selectedLine == null)
            {
                TaskDialog.Show("Ошибка", noLineMessage);
                return false;
            }

            if (doc == null)
            {
                TaskDialog.Show("Ошибка", noDocumentMessage);
                return false;
            }

            return true;
        }

    }
}
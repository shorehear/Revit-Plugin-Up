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


                //инициализация окна
                SelectionWindow selectElementsWindow = new SelectionWindow(doc, uidoc, selectedElements);
                selectElementsWindow.Show();
                //пользуясь mvvm вытаскиваем из окон получившуюся информацию
                selectElementsWindow.SettingsClosedWithSettings += SelectionWindow_SettingsClosedWithSettings;

                
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

        private void SelectionWindow_SettingsClosedWithSettings(object sender, Object[] settings)
        {
            if (settings.Length < 5)
            {
                TaskDialog.Show("Ошибка", "Не удалось получить все необходимые данные для копирования элементов.");
                return;
            }

            selectedElements = settings[0] as List<Element>;
            selectedLine = settings[1] as Line;
            XYZ coordinatesPoint = settings[2] as XYZ;
            double distance;
            int quantity;

            if (!double.TryParse(settings[3].ToString(), out distance) || !int.TryParse(settings[4].ToString(), out quantity))
            {
                TaskDialog.Show("Ошибка", "Не удалось получить данные о расстоянии и/или количестве копий.");
                return;
            }

            ElementsCopier elementsCopier = new ElementsCopier(selectedElements, selectedLine, coordinatesPoint, distance, quantity, doc);
        }
    }
}
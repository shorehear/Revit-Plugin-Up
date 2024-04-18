using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;

namespace ElementsCopier
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Launch : IExternalCommand
    {
        private Document doc;
        private UIDocument uidoc;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                uidoc = uiapp.ActiveUIDocument;
                doc = uidoc.Document;

                SelectionWindow selectionWindow = new SelectionWindow(doc, uidoc);
                selectionWindow.Topmost = true;
                selectionWindow.Show();
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", ex.Message);
                return Result.Failed;
            }
            return Result.Succeeded;
        }
    }
}
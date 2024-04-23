using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Revit.Async;

namespace ElementsCopier
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Launch : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                RevitTask.Initialize(commandData.Application);
                
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;


                SelectionWindow selectionWindow = new SelectionWindow(uidoc.Document, uiapp.ActiveUIDocument);
                
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
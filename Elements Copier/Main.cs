using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System;


namespace Elements_Copier
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Plugin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                UIApplication uiapp = commandData.Application;
                UIDocument uidoc = uiapp.ActiveUIDocument;
                Document doc = uidoc.Document;

                SelectionElementsWindow selectElementsWindow = new SelectionElementsWindow(doc, uidoc);
                //selectElementsWindow.Closed += SelectionWindow_Closed;
                //selectElementsWindow.ElementSelectionEvent += HandleElementSelection;
                selectElementsWindow.Show();
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
    }
}

using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;

namespace ElementsCopier
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

               
                SelectionWindow selectionWindow = new SelectionWindow(doc, commandData);
                selectionWindow.ElementSelectionEvent += (sender, args) => HandleElementSelection(selectionWindow);

                selectionWindow.Show();
                selectionWindow.HandleElementSelection();


            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                return Result.Cancelled;
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }

            return Result.Succeeded;
        }

        private void HandleElementSelection(SelectionWindow selectionWindow)
        {
            selectionWindow.HandleElementSelection();
        }
    }
}

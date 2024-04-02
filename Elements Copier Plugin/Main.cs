﻿using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI;
using System;

namespace Plugin
{
    [Transaction(TransactionMode.Manual)]
    [Regeneration(RegenerationOption.Manual)]
    public class Plugin : IExternalCommand
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

                selectionWindow.StartElementsCopier += ElementsCopierWork;


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
        private void ElementsCopierWork(object sender, EventArgs e)
        {
            try
            {
                ElementsCopier elementsCopier = new ElementsCopier(doc, uidoc);
                elementsCopier.CopyElements();
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", "Main.60\n" + ex.Message);
            }
        }
    }
}

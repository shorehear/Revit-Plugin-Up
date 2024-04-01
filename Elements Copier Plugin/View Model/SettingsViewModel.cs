using System;
using System.ComponentModel;
using Autodesk.Revit.UI.Selection;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;

namespace Plugin
{
    public class SettingsViewModel //: INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;

        public event EventHandler EndSettingsWindow;
        public ICommand EndSettingsCommand { get; }
        public ICommand SelectPointCommand { get; }
        public ICommand SelectLineCommand { get; }

        public SettingsViewModel(Document doc, UIDocument uidoc)
        {
            this.uidoc = uidoc;
            this.doc = doc;

            SelectPointCommand = new RelayCommand(SelectPoint);
            SelectLineCommand = new RelayCommand(SelectLine);
            EndSettingsCommand = new RelayCommand(EndSettings);
        }

        private void SelectPoint(object parameter)
        {
            try
            {
                if (uidoc != null)
                {
                    ElementsData.SelectedPoint = uidoc.Selection.PickPoint("Укажите точку");
                    TaskDialog.Show("Координаты", $"{ElementsData.SelectedPoint}");
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { TaskDialog.Show("Отмена", "Пользователь отменил операцию."); }
            catch (Exception ex) { TaskDialog.Show("Ошибка", $"{ex.Message}, [42 settings]"); }
        }
        private void SelectLine(object parameter)
        {

            if (uidoc != null)
            {
                var filter = new LineSelectionFilter();
                try
                {
                    var lineReference = uidoc.Selection.PickObject(ObjectType.Element, filter, "Укажите линию");

                    ElementsData.SelectedLine = uidoc.Document.GetElement(lineReference) as ModelLine;

                    TaskDialog.Show("Es", $"{ElementsData.SelectedLine.Id}");

                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException) 
                {
                    TaskDialog.Show("Отмена", "Пользователь отменил операцию\n[67 settings]");
                }
                catch (Exception ex) { TaskDialog.Show("Ошибка", $"{ex.Message}\n[69 settings]"); }
            }
        }

        private void EndSettings(object parameter)
        {
            EndSettingsWindow?.Invoke(this, new EventArgs());
            TaskDialog.Show("Завершено", $"Значения, заданные для копирования: {ElementsData.SelectedLine.Id}, " +
                $"\n{ElementsData.CountElements},\n{ElementsData.DistanceBetweenElements}");
        }

        //private string cointCopies;
        //public string CointCopies
        //{
        //    get { return cointCopies; }
        //    set
        //    {
        //        if (value != cointCopies)
        //        {
        //            cointCopies = value;
        //            OnPropertyChanged(nameof(CointCopies));
        //        }
        //    }

        //}

    }

}

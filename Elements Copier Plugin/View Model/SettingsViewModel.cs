using System;
using System.ComponentModel;
using Autodesk.Revit.UI.Selection;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;

namespace Plugin
{
    public class SettingsViewModel : INotifyPropertyChanged
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
                    UpdatePointData();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) { TaskDialog.Show("Отмена", "Пользователь отменил операцию."); }
            catch (Exception ex) { TaskDialog.Show("Ошибка", $"{ex.Message}, [42 settings]"); }
        }

        private string selectedPointData;
        public string SelectedPointData
        {
            get { return selectedPointData; }
            set
            {
                selectedPointData = value;
                OnPropertyChanged();
            }
        }
        private string selectedLineData;
        public string SelectedLineData
        {
            get { return selectedLineData; }
            set
            {
                selectedLineData = value;
                OnPropertyChanged();
            }
        }

        private void UpdatePointData()
        {
            string coordinates = $"({ElementsData.SelectedPoint.X:F2}; {ElementsData.SelectedPoint.Y:F2}; {ElementsData.SelectedPoint.X:F2})";
            SelectedPointData = coordinates;
            OnPropertyChanged();
        }
        private void UpdateLineData()
        {
            SelectedLineData = ElementsData.SelectedLine.Id.ToString();
            OnPropertyChanged();
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
                    UpdateLineData();
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

        private string cointCopies;
        public string CointCopiesText
        {
            get { return cointCopies; }
            set
            {
                cointCopies = value;
                OnPropertyChanged();
                ElementsData.CountElements = int.Parse(cointCopies);

            }
        }

        private string distanceBetweenCopies;
        public string DistanceBetweenCopiesText
        {
            get { return distanceBetweenCopies; }
            set
            {
                distanceBetweenCopies = value;
                OnPropertyChanged();
                ElementsData.DistanceBetweenElements = double.Parse(distanceBetweenCopies);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}

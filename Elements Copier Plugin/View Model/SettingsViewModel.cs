using System;
using System.ComponentModel;
using Autodesk.Revit.UI.Selection;
using System.Runtime.CompilerServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;

using System.Collections.Generic;

namespace Plugin
{
    public class SettingsViewModel : INotifyPropertyChanged
    {
        #region Инициализация свойств: элементы, линия, точка, дистанция, количество копий, необходимость во вращении, работа с выбранными элементами и с копируемыми
        public IList<ElementId> SelectedElements
        {
            get { return ElementsData.SelectedElements; }
            set { ElementsData.SelectedElements = value; OnPropertyChanged("Выбранные элементы"); }
        }

        public ModelLine SelectedLine
        {
            get { return ElementsData.SelectedLine; }
            set { ElementsData.SelectedLine = value; OnPropertyChanged("Выбранная линия"); }
        }

        public XYZ SelectedPoint
        {
            get { return ElementsData.SelectedPoint; }
            set { ElementsData.SelectedPoint = value; OnPropertyChanged("Выбранная точка"); }
        }

        public double DistanceBetweenElements
        {
            get { return ElementsData.DistanceBetweenElements; }
            set { ElementsData.DistanceBetweenElements = value; OnPropertyChanged("Дистанция между копиями"); }
        }

        public int CountElements
        {
            get { return ElementsData.CountElements; }
            set { ElementsData.CountElements = value; OnPropertyChanged("Количество копий"); }
        }

        public bool NeedRotate
        {
            get { return ElementsData.NeedRotate; }
            set { ElementsData.NeedRotate = value; OnPropertyChanged("Необходимо вращение"); }
        }

        public bool SelectedAndCopiedElements
        {
            get { return ElementsData.SelectedAndCopiedElements; }
            set { ElementsData.SelectedAndCopiedElements = value; OnPropertyChanged("Перемещение и выбранных, и копированных элементов"); }
        }
        #endregion

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

        #region Выбор точки для копирования
        private void SelectPoint(object parameter)
        {
            try
            {
                if (uidoc != null)
                {
                    SelectedPoint = uidoc.Selection.PickPoint("Укажите точку");
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
        private void UpdatePointData()
        {
            string coordinates = $"({SelectedPoint.X:F2}; {SelectedPoint.Y:F2}; {SelectedPoint.X:F2})";
            SelectedPointData = coordinates;
            OnPropertyChanged();
        }
        #endregion

        #region Выбор линии для копирования
        private void SelectLine(object parameter)
        {
            if (uidoc != null)
            {
                var filter = new LineSelectionFilter();
                try
                {
                    var lineReference = uidoc.Selection.PickObject(ObjectType.Element, filter, "Укажите линию");
                    SelectedLine = uidoc.Document.GetElement(lineReference) as ModelLine;
                    UpdateLineData();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    TaskDialog.Show("Отмена", "Пользователь отменил операцию\n[67 settings]");
                }
                catch (Exception ex) { TaskDialog.Show("Ошибка", $"{ex.Message}\n[69 settings]"); }
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
        private void UpdateLineData()
        {
            SelectedLineData = SelectedLine.Id.ToString();
            OnPropertyChanged();
        }
        #endregion

        private void EndSettings(object parameter)
        {
            if (Injerity())
            {
                EndSettingsWindow?.Invoke(this, new EventArgs());
            }
        }

        private bool Injerity()
        {
            if(SelectedLine == null && SelectedPoint == null)
            {
                TaskDialog.Show("Ошибка", "Пожалуйста, выберите либо линию копирования, либо точку копирования");
                return false;
            }
            if(SelectedLine != null && SelectedPoint != null)
            {
                TaskDialog.Show("Ошибка", "Пожалуйста, выберите что-то одно: либо линию направления, либо точку копирования");
                return false;
            }
            return true;
        }

        #region Настройки: количество копий, дистанция между ними
        private string countCopies;
        public string CountCopiesText
        {
            get { return countCopies; }
            set
            {
                countCopies = value;
                OnPropertyChanged();
                CountElements = int.Parse(countCopies);
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
                DistanceBetweenElements = double.Parse(distanceBetweenCopies);
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

}

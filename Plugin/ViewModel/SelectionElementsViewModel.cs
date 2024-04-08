using System;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Collections.ObjectModel;

namespace Plugin
{

    public class SelectionElementsViewModel : INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;

        public event EventHandler StartElementsCopier;
        private bool isSelecting;

        #region Инициализация геттеров, сеттеров для Elements Data
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

        public XYZ SelectedCopyPoint
        {
            get { return ElementsData.SelectedCopyPoint; }
            set { ElementsData.SelectedCopyPoint = value; OnPropertyChanged("Выбранная точка копирования"); }
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

        public ObservableCollection<string> ComboBoxItems { get; set; }
        #endregion

        #region Инициализация кнопок
        public ICommand AdditionalElementsCommand { get; }
        public ICommand SelectPointCommand { get; }
        public ICommand SelectLineCommand { get; }
        public ICommand StopSelectingCommand { get; }
        #endregion

        #region Запуск окна
        public SelectionElementsViewModel(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;

            ElementsData.Initialize();

            ComboBoxItems = new ObservableCollection<string>();
            ComboBoxItems.Add("Выбрать линию");
            ComboBoxItems.Add("Выбрать точку копирования");

            AdditionalElementsCommand = new RelayCommand(AdditionalElements);
            SelectPointCommand = new RelayCommand(SelectPoint);
            StopSelectingCommand = new RelayCommand(StopSelecting);
            Status = StatusType.GetStatusMessage("Default");

            Initialize();
        }

        private async void Initialize()
        {
            await Task.Delay(1);
            isSelecting = true;
            RequestElementSelection();
        }

        private void AdditionalElements(object parameters)
        {
            if (!isSelecting)
            {
                isSelecting = true;
                Status = StatusType.GetStatusMessage("Default");

                RequestElementSelection();

                Task.Delay(500).ContinueWith(_ => isSelecting = false);
            }
        }
        #endregion

        #region Выбор опции копирования
        private string _selectedComboBoxItem;
        public string SelectedComboBoxItem
        {
            get { return _selectedComboBoxItem; }
            set
            {
                if (_selectedComboBoxItem != value)
                {
                    if (value == "Выбрать линию")
                    {
                        SelectLine();
                    }
                    else if (value == "Выбрать точку копирования")
                    {
                        SelectCopyPoint();
                    }
                }
            }
        }

        private void SelectLine()
        {
            if (uidoc != null)
            {
                var filter = new LineSelectionFilter();
                try
                {
                    SelectedCopyPoint = null;
                    _selectedComboBoxItem = "";

                    Status = StatusType.GetStatusMessage("SelectLine");
                    var lineReference = uidoc.Selection.PickObject(ObjectType.Element, filter, "Укажите линию");
                    Element selectedElement = uidoc.Document.GetElement(lineReference);

                    SelectedLine = uidoc.Document.GetElement(lineReference) as ModelLine;
                    _selectedComboBoxItem = SelectedLine.Id.ToString();
                    OnPropertyChanged(nameof(SelectedComboBoxItem));
                    Status = StatusType.GetStatusMessage("SelectedLine");
                }
                catch (Exception ex) { TaskDialog.Show("Ошибка", $"{ex.Message}\n[166 select]"); }
            }
        }
        private void SelectCopyPoint()
        {
            if (uidoc != null)
            {
                try
                {
                    SelectedLine = null;
                    _selectedComboBoxItem = "";
                    Status = StatusType.GetStatusMessage("SelectCopyPoint");
                    SelectedCopyPoint = uidoc.Selection.PickPoint("Укажите точку");
                    _selectedComboBoxItem = $"X: {SelectedCopyPoint.X:F1}; Y: {SelectedCopyPoint.Y:F1}; Z: {SelectedCopyPoint.Z:F1}";
                    Status = StatusType.GetStatusMessage("SelectedCopyPoint");
                    OnPropertyChanged(nameof(SelectedComboBoxItem));
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", $"{ ex.Message}\n[186 select]");
                }
            }
        }
        #endregion

        #region Выбор точки области
        private void SelectPoint(object parameter)
        {
            try
            {
                if (uidoc != null)
                {
                    Status = StatusType.GetStatusMessage("SelectPoint");
                    SelectedPoint = uidoc.Selection.PickPoint("Укажите точку");
                    SelectedPointData = $"X: {SelectedPoint.X:F1}; Y: {SelectedPoint.Y:F1}; Z: {SelectedPoint.Z:F1}";
                    Status = StatusType.GetStatusMessage("SelectedPoint");
                    OnPropertyChanged();
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

        #endregion

        #region Настройки копирования элементов
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

        #region Настройки выбора элементов
        private void RequestElementSelection()
        {
            try
            {
                IList<Element> selectedElements = uidoc.Selection.PickElementsByRectangle("Выберите область");

                foreach (Element selectedElement in selectedElements)
                {
                    ElementId elementId = selectedElement.Id;
                    Category category = GetElementCategory(selectedElement);

                    if (category != null)
                    {
                        if (!ElementsData.SelectedElements.Contains(elementId))
                        {
                            ElementsData.SelectedElements.Add(elementId);
                            UpdateSelectedElementsText();
                        }
                    }
                }
                Status = "Выбраны элементы. \nЧтобы добавить еще, нажмите 'Добавить'.";
                isSelecting = false;
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {

                TaskDialog.Show("Отмена", "Пользователь отменил выбор объектов.");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", $"{ex.Message}, [77 selection]");
            }

        }

        private string selectedElementsText;
        public string SelectedElementsText
        {
            get { return selectedElementsText; }
            set
            {
                selectedElementsText = value;
                OnPropertyChanged();
            }
        }
        private void UpdateSelectedElementsText()
        {
            StringBuilder elementsListBuilder = new StringBuilder();
            foreach (var elementId in ElementsData.SelectedElements)
            {
                Element element = doc.GetElement(elementId);
                if (element != null)
                {
                    elementsListBuilder.AppendLine($"{element.Name} {element.Id}");
                }
            }
            SelectedElementsText = elementsListBuilder.ToString();
        }
        private Category GetElementCategory(Element element) { return element?.Category; }

        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }
        #endregion

        #region Завершение выбора элементов
        private void StopSelecting(object parameter)
        {
            if (ElementsData.SelectedLine == null || ElementsData.SelectedPoint == null)
            {
                if (ElementsData.SelectedLine == null && ElementsData.SelectedPoint == null)
                {
                    Status = StatusType.GetStatusMessage("MissingLineAndPoint");
                }
                else if (ElementsData.SelectedLine == null && ElementsData.SelectedPoint != null)
                {
                    Status = StatusType.GetStatusMessage("MissingLine");
                }
                else if (ElementsData.SelectedLine != null && ElementsData.SelectedPoint == null)
                {
                    Status = StatusType.GetStatusMessage("MissingPoint");
                }
            }
            else
            {
                Status = StatusType.GetStatusMessage("ObjectsSelected");
                StartElementsCopier?.Invoke(this, new EventArgs());
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

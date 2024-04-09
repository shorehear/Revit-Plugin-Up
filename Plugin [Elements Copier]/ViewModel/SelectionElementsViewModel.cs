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

namespace ElementsCopier
{

    public class SelectionElementsViewModel : INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;

        public event EventHandler StartElementsCopier;

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
        public double DistanceBetweenElements
        {
            get { return ElementsData.DistanceBetweenElements; }
            set { ElementsData.DistanceBetweenElements = (value/304.8); OnPropertyChanged("Дистанция между копиями"); }
        }
        #endregion

        #region Инициализация кнопок
        public ICommand AdditionalElementsCommand { get; }
        public ICommand SelectPointCommand { get; }
        public ICommand SelectLineCommand { get; set; }
        public ICommand SelectCopyPointCommand { get; set; }
        public ICommand StopSelectingCommand { get; }
        #endregion

        #region Запуск окна
        public SelectionElementsViewModel(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;

            ElementsData.Initialize();


            AdditionalElementsCommand = new RelayCommand(AdditionalElements);
            SelectPointCommand = new RelayCommand(SelectPoint);

            SelectLineCommand = new RelayCommand(SelectLine);
            SelectCopyPointCommand = new RelayCommand(SelectCopyPoint);

            StopSelectingCommand = new RelayCommand(StopSelecting);
            Status = StatusType.GetStatusMessage("Default");

            Initialize();
        }

        private async void Initialize()
        {
            await Task.Delay(1);
            RequestElementSelection();
        }

        private void AdditionalElements(object parameters)
        {
            Status = StatusType.GetStatusMessage("Default");
            RequestElementSelection();
        }
        #endregion

        #region Выбор точки области копируемых элементов
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
            catch (Autodesk.Revit.Exceptions.OperationCanceledException) 
            {
                Status = StatusType.GetStatusMessage("UserCanseledOperation");
            }
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

        #region Выбор области для копирования
        private string _selectedItem;
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem != value)
                {
                    _selectedItem = value;
                    OnPropertyChanged(nameof(SelectedItem));
                }
            }
        }

        private void SelectLine(object parameter)
        {
            if (uidoc != null)
            {
                var filter = new LineSelectionFilter();
                try
                {
                    SelectedCopyPoint = null;
                    SelectedItem = "";

                    Status = StatusType.GetStatusMessage("SelectLine");
                    var lineReference = uidoc.Selection.PickObject(ObjectType.Element, filter, "Укажите линию");
                    Element selectedElement = uidoc.Document.GetElement(lineReference);

                    SelectedLine = uidoc.Document.GetElement(lineReference) as ModelLine;
                    SelectedItem = SelectedLine.Id.ToString();
                    Status = StatusType.GetStatusMessage("SelectedLine");
                    OnPropertyChanged(nameof(SelectedItem));
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    Status = " ";
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", $"{ex.Message}\n[115.ViewModel]");
                }
            }
        }

        private void SelectCopyPoint(object parameter)
        {
            if (uidoc != null)
            {
                try
                {
                    SelectedLine = null;
                    SelectedItem = " ";
                    Status = StatusType.GetStatusMessage("SelectCopyPoint");
                    SelectedCopyPoint = uidoc.Selection.PickPoint("Укажите точку");
                    SelectedItem = $"X: {SelectedCopyPoint.X:F1}; Y: {SelectedCopyPoint.Y:F1}; Z: {SelectedCopyPoint.Z:F1}";
                    Status = StatusType.GetStatusMessage("SelectedCopyPoint");
                    OnPropertyChanged(nameof(SelectedItem));
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", $"{ ex.Message}\n[137.ViewModel]");
                }
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
                SetCountCopies(countCopies);
            }
        }

        private void SetCountCopies(string countCopies)
        {
            CountElements = int.Parse(countCopies);
        }

        private string distanceBetweenCopies;
        public string DistanceBetweenCopiesText
        {
            get { return distanceBetweenCopies; }
            set
            {
                if (ElementsData.SelectedLine != null)
                {
                    distanceBetweenCopies = value;
                    if (distanceBetweenCopies.Contains("."))
                    {
                        Status = "Пожалуйста, введите \nнецелое число через запятую.";
                    }
                    else
                    {
                        SetDistanceBetweenCopies(distanceBetweenCopies);
                        Status = StatusType.GetStatusMessage("");
                    }
                }
                else
                {
                    Status = "Выбор дистанции между копиями \nэлементов доступен только \nпри копировании вдоль линии."; 
                }
            }
        }

        private void SetDistanceBetweenCopies(string distanceBetweenCopies)
        {
            DistanceBetweenElements = double.Parse(distanceBetweenCopies);
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
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                Status = StatusType.GetStatusMessage("UserCanseledSelection");
            }
            catch (Exception ex)
            {
                TaskDialog.Show("Ошибка", $"Кажется, вы нажали кнопку 'Добавить', когда выбор элементов был уже активен. {ex.Message}");
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
            if (ElementsData.SelectedPoint == null)
            {
                Status = StatusType.GetStatusMessage("MissingPoint");
            }
            else if (ElementsData.SelectedLine == null && ElementsData.SelectedCopyPoint == null)
            {
                Status = StatusType.GetStatusMessage("MissingLineAndCopyPoint");
            }
            else
            {
                try
                {
                    if (ElementsData.CountElements == 0)
                    {
                        Status = StatusType.GetStatusMessage("ZeroCountElements");
                    }
                    else if (ElementsData.CountElements != 0 && ElementsData.DistanceBetweenElements == 0)
                    {
                        Status = StatusType.GetStatusMessage("ZeroDistanceBetweenElements");
                        Status = StatusType.GetStatusMessage("ObjectsSelected");
                        StartElementsCopier?.Invoke(this, new EventArgs());
                    }
                    else
                    {
                        Status = StatusType.GetStatusMessage("ObjectsSelected");
                        StartElementsCopier?.Invoke(this, new EventArgs());
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", $"{ex.Message}, [340.ViewModel]");
                }
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

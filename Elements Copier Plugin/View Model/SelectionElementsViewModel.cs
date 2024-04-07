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

namespace Plugin
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
        
        private bool isSelecting;
        private const string DEFAULTSTATUS = "Ожидание выбора области объектов...";

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

            AdditionalElementsCommand = new RelayCommand(AdditionalElements);
            SelectPointCommand = new RelayCommand(SelectPoint);
            SelectLineCommand = new RelayCommand(SelectLine);
            StopSelectingCommand = new RelayCommand(StopSelecting);
            Status = DEFAULTSTATUS;

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
                Status = DEFAULTSTATUS;

                RequestElementSelection();

                Task.Delay(500).ContinueWith(_ => isSelecting = false);
            }
        }
        #endregion

        #region Выбор точки
        private void SelectPoint(object parameter)
        {
            try
            {
                if (uidoc != null)
                {
                    Status = "Выберите точку в области для копирования.";
                    SelectedPoint = uidoc.Selection.PickPoint("Укажите точку");
                    UpdatePointData();
                    Status = "Выбрана точка. \nЧтобы еще добавить элементов, нажмите 'Добавить'.";

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
            SelectedPointData = SelectedPoint != null ? $"X:{SelectedPoint.X:F1}; \nY:{SelectedPoint.Y:F1}; \nZ:{SelectedPoint.Z:F1}" : "0";
            OnPropertyChanged();
        }
        #endregion

        #region Выбор линии
        private void SelectLine(object parameter)
        {
            if (uidoc != null)
            {
                var filter = new LineSelectionFilter();
                try
                {
                    Status = "Выберите линию направления.";
                    var lineReference = uidoc.Selection.PickObject(ObjectType.Element, filter, "Укажите линию");
                    Element selectedElement = uidoc.Document.GetElement(lineReference);

                    SelectedLine = uidoc.Document.GetElement(lineReference) as ModelLine;
                    UpdateLineData();
                    Status = "Выбрана линия направления. \nЧтобы еще добавить элементов, нажмите 'Добавить'.";

                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    TaskDialog.Show("Отмена", "Пользователь отменил операцию\n[159 select]");
                }
                catch (Exception ex) { TaskDialog.Show("Ошибка", $"{ex.Message}\n[161 select]"); }
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
            SelectedLineData = SelectedLine != null ? SelectedLine.Id.ToString() : "Null";
            OnPropertyChanged();
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
                    TaskDialog.Show("Выберите линию и точку", "Не выбрана линия размещения объектов, не выбрана точка в области, " +
                        "\nотносительно которой совершится копирование. \nПожалуйста, определите недостающие объекты.");
                    Status = "Для начала копирования ожидается \nвыбор точки и линии.";
                }
                else if(ElementsData.SelectedLine == null && ElementsData.SelectedPoint != null)
                {
                    TaskDialog.Show("Выберите линию", "Не выбрана линия размещения объектов. \nПожалуйста, определите недостающий объект.");
                    Status = "Для начала копирования ожидается выбор линии.";
                }
                else if(ElementsData.SelectedLine != null && ElementsData.SelectedPoint == null)
                {
                    TaskDialog.Show("Выберите точку", "Не выбрана точка в области, относительно которой совершится копирование. \nПожалуйста, определите недостающие объекты");
                    Status = "Для начала копирования ожидается выбор точки.";
                }
            }
            else
            {
                Status = "Элементы выбраны.";
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

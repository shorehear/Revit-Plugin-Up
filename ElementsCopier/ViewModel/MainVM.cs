using System;
using System.Windows;
using System.Windows.Input;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Revit.Async;

namespace ElementsCopier
{
    public class SelectionElementsViewModel : INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;
        public PluginLogger logger;
        

        #region icommand
        public ICommand DeleteAllSelectedElementsCommand { get; }
        public ICommand AdditionalElementsCommand { get; }
        public ICommand SelectPointCommand { get; }
        public ICommand SelectLineCommand { get; set; }
        public ICommand StopSelectingCommand { get; }
        public ICommand CopyClipBoardCommand { get; }
        #endregion

        #region properties
        private ObservableCollection<Element> selectedElements;
        public ObservableCollection<Element> SelectedElements
        {
            get { return selectedElements; }
            set { selectedElements = value; OnPropertyChanged(nameof(SelectedElements)); }
        }

        public ModelLine SelectedLine
        {
            get { return ElementsData.SelectedLine; }
            set { ElementsData.SelectedLine = value; OnPropertyChanged(nameof(SelectedLine)); }
        }

        public XYZ SelectedPoint
        {
            get { return ElementsData.SelectedPoint; }
            set { ElementsData.SelectedPoint = value; OnPropertyChanged(nameof(SelectedPoint)); }
        }

        public int CountElements
        {
            get { return ElementsData.CountCopies; }
            set { ElementsData.CountCopies = value; OnPropertyChanged(nameof(CountElements)); }
        }

        public double DistanceBetweenElements
        {
            get { return ElementsData.DistanceBetweenElements; }
            set { ElementsData.DistanceBetweenElements = (value); OnPropertyChanged(nameof(DistanceBetweenElements)); }
        }

        public bool WithSourceElements
        {
            get { return ElementsData.WithSourceElements; }
            set { ElementsData.WithSourceElements = value; OnPropertyChanged(nameof(WithSourceElements)); }
        }
        #endregion

        #region setters
        private void AddSelectedElement(ElementId elementId)
        {
            Element element = doc.GetElement(elementId);
            if (element != null)
            {
                SelectedElements.Add(element);
                ElementsData.SelectedElements.Add(elementId);
                OnPropertyChanged(nameof(SelectedElements));
            }
        }

        private string selectedLine;
        public string SelectedLineLabel
        {
            get { return selectedLine; }
            set
            {
                selectedLine = value;
                OnPropertyChanged();
            }
        }

        private string selectedPoint;
        public string SelectedPointLabel
        {
            get { return selectedPoint; }
            set
            {
                selectedPoint = value;
                OnPropertyChanged();
            }
        }

        private string countCopies = "0";
        public string CountCopiesText
        {
            get { return countCopies; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    countCopies = "0";
                }
                else
                {
                    countCopies = value;
                    CountElements = int.Parse(countCopies);
                }
                OnPropertyChanged(nameof(CountCopiesText));
            }
        }

        private string distanceBetweenCopies = "0.0";
        public string DistanceBetweenCopiesText
        {
            get { return distanceBetweenCopies; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    distanceBetweenCopies = "0";
                }
                else
                {
                    distanceBetweenCopies = value;
                    if (distanceBetweenCopies.Contains("."))
                    {
                        Status = StatusType.GetStatusMessage("DistanceContains");
                    }
                    else
                    {
                        DistanceBetweenElements = double.Parse(distanceBetweenCopies);
                    }
                }
                OnPropertyChanged(nameof(DistanceBetweenCopiesText));
            }
        }

        private bool withSourceElements;
        public bool WithSourceElementsCheckBox
        {
            get { return withSourceElements; }
            set
            {
                withSourceElements = value;
                {
                    WithSourceElements = withSourceElements;
                }
                OnPropertyChanged(nameof(WithSourceElementsCheckBox));
            }
        }

        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }

        private string logText;
        public string LogText
        {
            get { return logText; }
            set
            {
                logText = value;
                OnPropertyChanged(nameof(LogText));
            }
        }

        private void CopyClipBoard(object parameter)
        {
            Clipboard.SetText(LogText.ToString());
        }
        #endregion

        #region initialization
        public SelectionElementsViewModel()
        {
            RevitTask.RunAsync((uiapp) => 
            { 
                doc = uiapp.ActiveUIDocument.Document; 
                uidoc = uiapp.ActiveUIDocument;
            });

            ElementsData.Initialize();
            SelectedElements = new ObservableCollection<Element>();

            AdditionalElementsCommand = new RelayCommand(AdditionalElements);
            DeleteAllSelectedElementsCommand = new RelayCommand(DeleteAllElements);
            CopyClipBoardCommand = new RelayCommand(CopyClipBoard);
            SelectPointCommand = new RelayCommand(SelectPoint);
            SelectLineCommand = new RelayCommand(SelectLine);
            StopSelectingCommand = new RelayCommand(StopSelecting);

            Status = StatusType.GetStatusMessage("WaitingStartSelection");
        }

        public void SetLogger(PluginLogger logger)
        {
            this.logger = logger;
            logger.LogInformation("Initialization.");
        }
        #endregion

        private void AdditionalElements(object parameters)
        {
            Status = StatusType.GetStatusMessage("WaitingForSelection");
            RequestElementSelection();
        }

        private void SelectPoint(object parameter)
        {
            try
            {
                if (uidoc != null)
                {
                    Status = StatusType.GetStatusMessage("SetPoint");

                    SelectedPoint = uidoc.Selection.PickPoint("Укажите точку");
                    SelectedPointLabel = $"X: {SelectedPoint.X:F1}; Y: {SelectedPoint.Y:F1}; Z: {SelectedPoint.Z:F1}";

                    Status = StatusType.GetStatusMessage("GetPoint");
                    OnPropertyChanged();
                }
            }
            catch (Autodesk.Revit.Exceptions.OperationCanceledException)
            {
                logger.LogWarning($"Operation Canceled Exception");
                Status = StatusType.GetStatusMessage("CanselOperation");
            }
        }

        private void SelectLine(object parameter)
        {
            if (uidoc != null)
            {
                var filter = new LineSelectionFilter();
                try
                {
                    Status = StatusType.GetStatusMessage("SetLine");
                    var lineReference = uidoc.Selection.PickObject(ObjectType.Element, filter, "Выберите линию");
                    Element selectedElement = uidoc.Document.GetElement(lineReference);

                    SelectedLine = uidoc.Document.GetElement(lineReference) as ModelLine;
                    SelectedLineLabel = SelectedLine.Id.ToString();

                    Status = StatusType.GetStatusMessage("GetLine");
                    OnPropertyChanged();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    logger.LogWarning($"Operation Canceled Exception");
                    Status = StatusType.GetStatusMessage("CanselOperation");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"{ex.Message}");
                }
            }
        }

        private void RequestElementSelection()
        {
            try
            {
                RevitTask.RunAsync(uiapp =>
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
                                AddSelectedElement(elementId);

                                Element element = doc.GetElement(elementId);

                                if (element is AssemblyInstance)
                                {
                                    AssemblyInstance assemblyInstance = element as AssemblyInstance;

                                    ICollection<ElementId> memberIds = assemblyInstance.GetMemberIds();
                                    foreach (ElementId memberId in memberIds)
                                    {
                                        RemoveElementFromSelectedElements(memberId);
                                    }
                                }
                            }
                        }
                    }

                    Status = StatusType.GetStatusMessage("GetElements");
                });
            }
            catch (Exception ex)
            {
                logger.LogError($"Произошла ошибка: {ex.Message}");
            }
        }

        private void RemoveElementFromSelectedElements(ElementId elementIdToRemove)
        {
            try
            {
                List<int> indexesToRemove = new List<int>();

                for (int i = 0; i < SelectedElements.Count; i++)
                {
                    if (SelectedElements[i].Id == elementIdToRemove)
                    {
                        indexesToRemove.Add(i);
                    }
                }

                indexesToRemove.Reverse();
                foreach (int index in indexesToRemove)
                {
                    SelectedElements.RemoveAt(index);
                }

                OnPropertyChanged(nameof(SelectedElements));
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }

        private void DeleteElement(Element parameter)
        {
            if (parameter != null)
            {
                SelectedElements.Remove(parameter);
                ElementsData.SelectedElements.Remove(parameter.Id);
                OnPropertyChanged(nameof(SelectedElements));
            }
        }

        private void DeleteAllElements(object parameter)
        {
            ClearAllData();
        }

        private void StopSelecting(object parameter)
        {
            if (ElementsData.SelectedElements == null)
            {
                Status = StatusType.GetStatusMessage("NoElementsSelected");
            }
            else if (ElementsData.SelectedPoint == null)
            {
                Status = StatusType.GetStatusMessage("NoPointSelected");
            }
            else if (ElementsData.SelectedLine == null)
            {
                Status = StatusType.GetStatusMessage("NoLineSelected");
            }
            else if (ElementsData.CountCopies == 0)
            {
                Status = StatusType.GetStatusMessage("NoCountCopies");
            }
            else
            {
                try
                {
                    CopyElements();
                }
                catch (Exception ex)
                {
                    logger.LogError($"{ex.Message}");
                }
            }
        }

        private void CopyElements()
        {
            ElementsData.ConvertValues();
            try
            {
                Transaction transaction = new Transaction(doc, "Copy Elements");
                RevitTask.RunAsync(uiapp =>
                {

                    FailureHandlingOptions failureHandlingOptions = transaction.GetFailureHandlingOptions();
                    FailureHandler failureHandler = new FailureHandler(logger);
                    failureHandlingOptions.SetFailuresPreprocessor(failureHandler);
                    failureHandlingOptions.SetClearAfterRollback(true);
                    transaction.SetFailureHandlingOptions(failureHandlingOptions);

                    if (transaction.Start() == TransactionStatus.Started)
                    {
                        XYZ translationVector = ElementsData.selectedLine.GetEndPoint(0) - ElementsData.SelectedPoint;

                        for (int copyIndex = 0; copyIndex < ElementsData.CountCopies; copyIndex++)
                        {   
                            foreach (ElementId elementId in ElementsData.SelectedElements)
                            {
                                ICollection<ElementId> newElementsIds = ElementTransformUtils.CopyElements(doc, new List<ElementId> { elementId }, translationVector);
                            }

                            if (ElementsData.CountCopies > 1 && ElementsData.DistanceBetweenElements != 0)
                            {
                                translationVector = translationVector.Add(ElementsData.selectedLine.Direction.Multiply(ElementsData.DistanceBetweenElements));
                            }
                        }

                        if (ElementsData.WithSourceElements)
                        {
                            ElementTransformUtils.MoveElements(doc, ElementsData.SelectedElements, translationVector);
                        }

                    }
                    if (transaction.Commit() == TransactionStatus.Committed)
                    {
                        logger.LogInformation("The copy is completed.");
                        ClearAllData();
                    }
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
            }
        }


        private Category GetElementCategory(Element element) { return element?.Category; }

        public void ClearAllData()
        {
            ElementsData.Initialize();

            SelectedElements.Clear();
            CountCopiesText = "0";
            DistanceBetweenCopiesText = "0";
            SelectedLineLabel = string.Empty;
            SelectedPointLabel = string.Empty;
            WithSourceElementsCheckBox = false;

            Status = StatusType.GetStatusMessage("WaitingForSelection");
            logger.LogInformation("The collection is ready to select new items.");
        }

        public void ListBox_SelectionChanged(object sender)
        {
            var parameter = sender as Element;
            DeleteElement(parameter);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
using System;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Elements_Copier
{
    public class SelectionElementsViewModel : INotifyPropertyChanged
    {
        public event EventHandler SelectingOver;

        private Document doc;
        private UIDocument uidoc;
        private SelectedElementsData selectedElementsData;

        private bool continueSelecting;
        private int typeOfOperation;

        public ICommand EndSelectionCommand { get; }

        public SelectionElementsViewModel(Document doc, UIDocument uidoc, int typeOfOperation)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.typeOfOperation = typeOfOperation; //1 выбор по клику, 2 выбор по области
            selectedElementsData = new SelectedElementsData(doc, uidoc);

            EndSelectionCommand = new RelayCommand(EndSelecting);
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await Task.Delay(1);
            continueSelecting = typeOfOperation != 0;
            RequestElementSelection(typeOfOperation);
        }

        private Category GetElementCategory(Element element) { return element?.Category; }
        private void HandleSelectedLine(Element selectedElement)
        {
            if (selectedElementsData.SelectedLine != null)
            {
                TaskDialog.Show("Ошибка", "Уже выбрана линия модели!");
                return;
            }
            else
            {
                selectedElementsData.SelectedLine = ((CurveElement)selectedElement).GeometryCurve as Line;
                ElementAdded?.Invoke(this, EventArgs.Empty);
                UpdateSelectedElementsText();
            }
        }
        private bool CheckProblemBeforeLaunch()
        {
            if (selectedElementsData.SelectedLine == null)
            {
                TaskDialog.Show("Ошибка", "Не была выбрана линия направления");
                return false;
            }
            else if (selectedElementsData.SelectedElements == null)
            {
                TaskDialog.Show("Ошибка", "Не были выбраны элементы");
                return false;
            }

            return true;
        }

        private void EndSelecting(object parameter)
        {
            if (CheckProblemBeforeLaunch())
            {
                continueSelecting = false;
                SelectingOver?.Invoke(this, EventArgs.Empty);
            }
            else
            {
                return;
            }
        }
        private void RequestElementSelection(int typeOfOperation)
        {
            switch (typeOfOperation)
            {
                case 1:
                    try
                    {
                        SingleSelectingElements(continueSelecting);
                    }
                    catch (OperationCanceledException)
                    {
                        continueSelecting = false;
                    }
                    catch (Exception ex)
                    {
                        continueSelecting = false;
                        TaskDialog.Show("Ошибка", $"{ex.Message}");
                    }
                    break;

                case 2:
                    try
                    {
                        GroupSelectingElements(continueSelecting);
                    }
                    catch (OperationCanceledException)
                    {
                        continueSelecting = false;
                    }
                    catch (Exception ex)
                    {
                        continueSelecting = false;
                        TaskDialog.Show("Ошибка", $"{ex.Message}");
                    }
                    break;

                default:
                    TaskDialog.Show("Ошибка", "Некорректно инициализирован тип операции");
                    break;
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

        public event EventHandler ElementAdded;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void UpdateSelectedElementsText() // вывод выбранных элементов в окно
        {
            StringBuilder elementsListBuilder = new StringBuilder();
            foreach (var elementId in selectedElementsData.SelectedElements)
            {
                Element element = doc.GetElement(elementId);
                if (element != null)
                {
                    elementsListBuilder.AppendLine($"{element.Name} {element.Id}");
                }
            }
            if (selectedElementsData.SelectedLine != null)
            {
                elementsListBuilder.AppendLine("\nЛиния направления выбрана. ");
            }
            SelectedElementsText = elementsListBuilder.ToString();
        }
        public SelectedElementsData GetSelectedElementsData() //для передачи данных об элементах в окно копирования
        {
            return selectedElementsData;
        }

        private void SingleSelectingElements(bool continueSelecting)
        {
            if (!continueSelecting) { return; }
            while (continueSelecting)
            {
                try
                {
                    using (Reference pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element))
                    {
                        if (pickedRef != null && continueSelecting)
                        {
                            Element selectedElement = doc.GetElement(pickedRef.ElementId);
                            ElementId selectedElementId = pickedRef.ElementId;
                            Category category = GetElementCategory(selectedElement);

                            if (category != null)
                            {
                                if (category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                                {
                                    HandleSelectedLine(selectedElement);
                                }
                                else if (!selectedElementsData.SelectedElements.Contains(selectedElementId))
                                {
                                    selectedElementsData.SelectedElements.Add(selectedElementId);
                                    UpdateSelectedElementsText();
                                }
                                else
                                {
                                    TaskDialog.Show("Ошибка", "Этот элемент уже выбран.");
                                }
                            }
                        }
                    }
                }
                catch (OperationCanceledException) { continueSelecting = false; return; }
            }
        }
        private void GroupSelectingElements(bool continueSelecting)
        {
            if (!continueSelecting) { return; }
            while (continueSelecting)
            {
                try
                {
                    ICollection<Reference> pickedRefs = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

                    if (continueSelecting && pickedRefs != null && pickedRefs.Count > 0)
                    {
                        foreach (Reference pickedRef in pickedRefs)
                        {
                            ElementId elementId = pickedRef.ElementId;
                            Element selectedElement = uidoc.Document.GetElement(elementId);
                            Category category = GetElementCategory(selectedElement);

                            if (category != null)
                            {
                                if (category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                                {
                                    HandleSelectedLine(selectedElement);
                                    UpdateSelectedElementsText();
                                }
                                else if (!selectedElementsData.SelectedElements.Contains(elementId))
                                {
                                    selectedElementsData.SelectedElements.Add(elementId);
                                    UpdateSelectedElementsText();
                                }
                                else
                                {
                                    TaskDialog.Show("Ошибка", "Этот элемент уже выбран.");
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", $"246 строка\n+{ex.Message}");
                    continueSelecting = false;
                }
            }
        }


    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Predicate<object> _canExecute;

        public event EventHandler CanExecuteChanged;
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }
        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute(parameter);
        }
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}

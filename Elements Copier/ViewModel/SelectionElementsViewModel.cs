using System;
using System.Text;
using System.Windows;
using System.Threading;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;


using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Elements_Copier
{
    public class SelectionElementsViewModel : INotifyPropertyChanged
    {
        public event EventHandler RequestClose;

        private Document doc;
        private UIDocument uidoc;
        private int typeOfOperation;

        private SelectedElementsData selectedElementsData;
        private bool continueSelecting;

        public ICommand StartSelectionCommand { get; }
        public ICommand EndSelectionCommand { get; }

        public SelectionElementsViewModel(Document doc, UIDocument uidoc, int typeOfOperation)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.typeOfOperation = typeOfOperation; //1-2 выбор элементов по клику без линии/с линией, 3-4 выбор элементов по области без линии/с линией


            selectedElementsData = new SelectedElementsData(doc, uidoc);

            EndSelectionCommand = new RelayCommand(EndSelecting);

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await Task.Delay(1);
            if (typeOfOperation != 0)
            {
                continueSelecting = true;
                RequestElementSelection(typeOfOperation);
            }
            else
            {
                TaskDialog.Show("Ошибка", "Тип операции");
            }
        }

        private Category GetElementCategory(Element element)
        {
            return element?.Category;
        }
        private void HandleSelectedLine(Element selectedElement)
        {
            if (typeOfOperation == 2 || typeOfOperation == 3)
            {
                if (selectedElementsData.SelectedLine != null)
                {
                    TaskDialog.Show("Ошибка", "Уже выбрана линия модели!");
                }
                else
                {
                    selectedElementsData.SelectedLine = ((CurveElement)selectedElement).GeometryCurve as Line;
                    ElementAdded?.Invoke(this, EventArgs.Empty);
                    UpdateSelectedElementsText();
                }
            }
            else
            {
                return;
            }
        }

        private void EndSelecting(object parameter)
        {
            continueSelecting = false;

            //обработать закрытие окна
            if ((typeOfOperation == 2 || typeOfOperation == 4) && selectedElementsData.SelectedLine == null)
            {
                TaskDialog.Show("Ошибка", "Не выбрана линия направления");
                return;
            }
            if ((typeOfOperation == 1 || typeOfOperation == 3) && selectedElementsData.SelectedLine != null)
            {
                TaskDialog.Show("Ошибка", "Была проинициализирована линия");
                return;
            }
            if (selectedElementsData.SelectedElements == null)
            {
                TaskDialog.Show("Ошибка", "Не были выбраны элементы");
                return;
            }
            TaskDialog.Show("Успешно", "Выбор элементов завершен!");
        }

        private void RequestElementSelection(int typeOfOperation)
        {
            switch (typeOfOperation)
            {
                case 1: // алгоритм работы, если операция "выбор элементов по клику, без линии"
                    try
                    {
                        while (true)
                        {
                            if (continueSelecting)
                            {
                                Reference pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);

                                if (pickedRef != null && continueSelecting)
                                {
                                    Element selectedElement = doc.GetElement(pickedRef.ElementId);
                                    ElementId selectedElementId = pickedRef.ElementId;
                                    Category category = GetElementCategory(selectedElement);

                                    if (category != null)
                                    {
                                        if (category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                                        {
                                            TaskDialog.Show("Ошибка", "В текущем режиме не определен выбор направляющей линии");
                                            continue;
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
                            else
                            {
                                break;
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        continueSelecting = false;
                        RequestClose?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case 2: //алгоритм работы, если операция "выбор элементов по клику, с линией"
                    try
                    {
                        while (true)
                        {
                            if (!continueSelecting) { break; }
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
                                            UpdateSelectedElementsText();
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
                                    else
                                    {
                                        TaskDialog.Show("Ошибка", "Ошибка выбора элементов. Выбранный элемент равен null");
                                    }
                                }
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        continueSelecting = false;
                        RequestClose?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case 3: // алгоритм работы, если операция "выбор элементов по области, без линии"
                    try
                    {
                        while (continueSelecting)
                        {
                            if (!continueSelecting) { break; }
                            ICollection<Reference> pickedRefs = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

                            if (pickedRefs != null && pickedRefs.Count > 0 && continueSelecting)
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
                                            TaskDialog.Show("Ошибка", "В текущем режиме не определен выбор направляющей линии");
                                            return;
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
                            else
                            {
                                continueSelecting = false;
                                break;
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        continueSelecting = false;
                        RequestClose?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                case 4:
                    try
                    {
                        while (continueSelecting)
                        {
                            if (!continueSelecting) { break; }
                            ICollection<Reference> pickedRefs = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element);

                            if (pickedRefs != null && pickedRefs.Count > 0 && continueSelecting)
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
                                            return;
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
                                    UpdateSelectedElementsText();
                                }
                            }
                            else
                            {
                                continueSelecting = false;
                                break;
                            }
                        }
                    }
                    catch (OperationCanceledException)
                    {
                        continueSelecting = false;
                        RequestClose?.Invoke(this, EventArgs.Empty);
                    }
                    break;
                default:
                    TaskDialog.Show("Ошибка", "Некорректно инициализирован тип операции");
                    break;
            }
        }

        
        private string _selectedElementsText;
        public string SelectedElementsText
        {
            get { return _selectedElementsText; }
            set
            {
                _selectedElementsText = value;
                OnPropertyChanged();
            }
        }

        public event EventHandler ElementAdded;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void UpdateSelectedElementsText()
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
                elementsListBuilder.AppendLine("\nЛиния направления выбрана");
            }
            SelectedElementsText = elementsListBuilder.ToString();
        }
        public SelectedElementsData GetSelectedElementsData()
        {
            return selectedElementsData;
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

using System.Collections.ObjectModel;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Input;
using System;
using System.Threading.Tasks;

//уникальность элементов проверяется не по ид так что корректно не работает, поправь

namespace Elements_Copier
{
    public class SelectionElementsViewModel //: INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;


        private SelectedElementsData _selectedElementsData;
        private bool _continueSelecting = true;

        public SelectionElementsViewModel(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            _selectedElementsData = new SelectedElementsData(doc, uidoc);
            EndSelectingCommand = new RelayCommand(EndSelecting);
            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            await Task.Delay(100);
            RequestElementSelection();
        }

        public ObservableCollection<ElementId> SelectedElementIds
        {
            get { return _selectedElementsData.SelectedElements; }
        }

        public Line SelectedLine
        {
            get { return _selectedElementsData.SelectedLine; }
            set
            {
                _selectedElementsData.SelectedLine = value;
            }
        }

        public ICommand EndSelectingCommand { get; }
        private void RequestElementSelection()
        {
            if (!_continueSelecting)
            {
                return;
            }
            else if (_continueSelecting)
            {
                try
                {
                    Reference pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                    if (pickedRef != null)
                    {
                        Element selectedElement = doc.GetElement(pickedRef.ElementId);
                        ElementId selectedElementId = pickedRef.ElementId;

                        if (doc.GetElement(selectedElementId).Category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                        {
                            if (_selectedElementsData.SelectedLine != null)
                            {
                                TaskDialog.Show("Ошибка", "Уже выбрана линия модели!");
                            }
                            else
                            {
                                _selectedElementsData.SelectedLine = ((CurveElement)selectedElement).GeometryCurve as Line;
                                //UpdateSelectedElementsTextBox();
                            }
                        }
                        else
                        {
                            if (!_selectedElementsData.SelectedElements.Contains(selectedElementId))
                            {
                                _selectedElementsData.SelectedElements.Add(selectedElementId);

                                //ElementSelectionEvent?.Invoke(this, selectedElement);
                                //UpdateSelectedElementsTextBox();
                            }
                            else
                            {
                                TaskDialog.Show("Ошибка", "Этот элемент уже выбран.");
                            }
                        }
                    }
                    else
                    {
                        TaskDialog.Show("Ошибка выбора элементов", "pickedRef == null");
                    }
                    RequestElementSelection();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    _continueSelecting = false;
                    Close();
                }
            }
        }

        public event EventHandler RequestClose;

        private void Close()
        {
            RequestClose?.Invoke(this, EventArgs.Empty);
        }

        private void EndSelecting(object parameter)
        {
            _continueSelecting = false;
            RequestElementSelection();

            TaskDialog.Show("Успешно", "Выбор элементов завершен!");

            string elementsList = "Элементы:\n";
            foreach (var elementId in _selectedElementsData.SelectedElements)
            {
                Element element = doc.GetElement(elementId);
                if (element != null)
                {
                    elementsList += element.Name + "  " + element.Id + "\n";
                }
            }
            TaskDialog.Show("Выбранные элементы", elementsList);
            if (_selectedElementsData.SelectedLine != null)
            {
                TaskDialog.Show("Статус линии", "Линия успешно выбрана!");
            } 
            else
            {
                TaskDialog.Show("Статус линии", "Линия не была выбрана.");
            }

            Close();
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


using System;
using System.Text;
using System.Threading;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace Elements_Copier
{
    public class SelectionElementsViewModel : INotifyPropertyChanged
    {
        public event EventHandler RequestClose;
        private CancellationTokenSource _cancellationTokenSource;

        private Document doc;
        private UIDocument uidoc;

        private SelectedElementsData _selectedElementsData;
        private bool _continueSelecting = true;
        private int _typeOfOperation;
        public SelectionElementsViewModel(Document doc, UIDocument uidoc, int typeOfOperation)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            _typeOfOperation = typeOfOperation;
            _selectedElementsData = new SelectedElementsData(doc, uidoc);
            EndSelectingCommand = new RelayCommand(EndSelecting);
            _cancellationTokenSource = new CancellationTokenSource();
            InitializeAsync();
        }
        private async void InitializeAsync()
        {
            await Task.Delay(10);
            RequestElementSelection(_cancellationTokenSource.Token); 
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
        private Category GetElementCategory(Element element)
        {
            return element?.Category;
        }
        private void HandleSelectedLine(Element selectedElement)
        {
            if (_selectedElementsData.SelectedLine != null)
            {
                TaskDialog.Show("Ошибка", "Уже выбрана линия модели!");
            }
            else
            {
                _selectedElementsData.SelectedLine = ((CurveElement)selectedElement).GeometryCurve as Line;
                ElementAdded?.Invoke(this, EventArgs.Empty);
                UpdateSelectedElementsText();
            }
        }
        private void RequestElementSelection(CancellationToken cancellationToken)
        {
            try
            {
                while (_continueSelecting && !cancellationToken.IsCancellationRequested)
                {
                    using (Reference pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element))
                    {
                        if (!_continueSelecting || cancellationToken.IsCancellationRequested)
                        {
                            break;
                        }
                        if (pickedRef != null)
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
                                else if (!_selectedElementsData.SelectedElements.Contains(selectedElementId))
                                {
                                    _selectedElementsData.SelectedElements.Add(selectedElementId);
                                    UpdateSelectedElementsText();
                                }
                                else
                                {
                                    TaskDialog.Show("Ошибка", "Этот элемент уже выбран.");
                                }
                            }
                        }
                        else
                        {
                            TaskDialog.Show("Ошибка выбора элементов", "Выбранный элемент равен null.");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                _continueSelecting = false;
                RequestClose?.Invoke(this, EventArgs.Empty);
            }
        }

        private void EndSelecting(object parameter)
        {
            _cancellationTokenSource.Cancel();

            TaskDialog.Show("Успешно", "Выбор элементов завершен!");
            _continueSelecting = false;
            RequestClose?.Invoke(this, EventArgs.Empty);
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
            foreach (var elementId in _selectedElementsData.SelectedElements)
            {
                Element element = doc.GetElement(elementId);
                if (element != null)
                {
                    elementsListBuilder.AppendLine($"{element.Name} {element.Id}");
                }
            }
            if (_selectedElementsData.SelectedLine != null)
            {
                elementsListBuilder.AppendLine("\nЛиния направления выбрана");
            }
            SelectedElementsText = elementsListBuilder.ToString();
        }
        public SelectedElementsData GetSelectedElementsData()
        {
            return _selectedElementsData;
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

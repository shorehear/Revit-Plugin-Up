using System;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace Plugin
{

    public class SelectionElementsViewModel : INotifyPropertyChanged
    {
        public event EventHandler StartSettingsWindow;

        private Document doc;
        private UIDocument uidoc;

        private bool continueSelecting;
        private bool isSelecting;

        public ICommand StopSelectingCommand { get; }
        public ICommand AdditionalElementsCommand { get; }

        #region Инициализация окна выбора элементов
        public SelectionElementsViewModel(Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;

            ElementsData.Initialize();

            AdditionalElementsCommand = new RelayCommand(AdditionalElements);
            StopSelectingCommand = new RelayCommand(StopSelecting);
            Status = DEFAULTSTATUS;

            Initialize();
        }

        private async void Initialize()
        {
            await Task.Delay(1);
            continueSelecting = true;
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

        #region Добавление элементов в стек
        private void RequestElementSelection() 
        {
            if (continueSelecting)
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
                    if (continueSelecting)
                    {
                        TaskDialog.Show("Отмена", "Пользователь отменил выбор объектов.");
                        continueSelecting = false;
                    }
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка", $"{ex.Message}, [77 selection]");
                    continueSelecting = false;
                }
            }
        }

        private void StopSelecting(object parameter)
        {
            Status = "Элементы выбраны.";
            continueSelecting = false;
            StartSettingsWindow?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Обработка окна выбора элементов
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
        #endregion

        #region Статус текущей операции
        private string status;
        public string Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }
        private const string DEFAULTSTATUS = "Ожидание выбора области объектов...";
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

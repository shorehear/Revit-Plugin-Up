using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using System;
using System.Text;

namespace ElementsCopier
{
    public partial class SelectionWindow : Window, INotifyPropertyChanged
    {
        private Document doc;
        private UIDocument uidoc;
        private List<Element> selectedElements;


        private Label statusLabel;
        private Label infoLabel;
        private System.Windows.Controls.TextBox selectedElementsTextBox;
        private Button endSelecting;


        bool continueSelecting;
        private string status;
        public string Status
        {
            get { return status; }
            set
            {
                if (status != value)
                {
                    status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public SelectionWindow(Document doc, UIDocument uidoc, List<Element> selectedElements)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.selectedElements = selectedElements;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Title = "Менеджер копирования";
            Width = 500;
            Height = 400;

            infoLabel = new Label()
            {
                Content = "Выберите объекты, которые необходимо дублировать.\nДля вращения выберите направляющую линию модели.",
                Margin = new Thickness(10, 0, 0, 0)
            };

            statusLabel = new Label()
            {
                Content = "Выбранные элементы отразятся здесь:",
                Margin = new Thickness(10, 0, 0, 0)
            };

            endSelecting = new Button()
            {
                Content = "Необходимые элементы выбраны.",
                Margin = new Thickness(10, 10, 0, 0),

                HorizontalAlignment = HorizontalAlignment.Right
            };
            endSelecting.Click+= endSelecting_Click;

            selectedElementsTextBox = new System.Windows.Controls.TextBox()
            {
                IsReadOnly = true,
                Margin = new Thickness(10, 10, 0, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 100,
                Width = 400
            };

           
            StackPanel panel = new StackPanel();
            panel.Children.Add(infoLabel);
            panel.Children.Add(statusLabel);
            panel.Children.Add(selectedElementsTextBox);
            panel.Children.Add(endSelecting);

            Content = panel;
            UpdateSelectedElementsTextBox();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSelectedElementsTextBox()
        {

            selectedElementsTextBox.Text = "Нет выбранных элементов";

            if(selectedElements != null && selectedElements.Any())
            {
                StringBuilder elementsText = new StringBuilder("Выбранные элементы:\n");
                foreach (var element in selectedElements)
                {
                    elementsText.Append(element.Name + " (" + element.ToString() + ")\n");
                }

                selectedElementsTextBox.Text = elementsText.ToString();
            }
        }


        public delegate void ElementSelectionEventHandler(object sender, Element selectedElement);

        public event ElementSelectionEventHandler ElementSelectionEvent;

        protected virtual void OnElementSelectionEvent(Element selectedElement)
        {
            ElementSelectionEvent?.Invoke(this, selectedElement);
        }

        public void HandleElementSelection()
        {
            if (!continueSelecting)
            {
                try
                {
                    continueSelecting = true; //  флаг выбора элементов в true
                    RequestElementSelection(); //  рекурсивный выбор элементов
                }
                catch (Exception ex)
                {
                    TaskDialog.Show("Ошибка при получении выбранных элементов", ex.Message);
                }
            }
        }


        private void RequestElementSelection()
        {
            if (continueSelecting)
            {
                try
                {
                    Reference pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
                    Element selectedElement = doc.GetElement(pickedRef.ElementId);

                    if (!selectedElements.Contains(selectedElement))
                    {
                        //вызов события выбора элемента и передача выбранного элемента
                        ElementSelectionEvent?.Invoke(this, selectedElement);
                        selectedElements.Add(selectedElement);

                        UpdateSelectedElementsTextBox();
                    }
                    else
                    {
                        MessageBox.Show("Этот элемент уже выбран!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    RequestElementSelection();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    continueSelecting = false;
                    Close();
                }
            }
            else
            {
                Close();
            }
        }


        private void endSelecting_Click(object sender, RoutedEventArgs e)
        {
            continueSelecting = false; // флаг продолжения выбора элементов в false
            RequestElementSelection(); // обработка завершения выбора элементов
        }
    }
}
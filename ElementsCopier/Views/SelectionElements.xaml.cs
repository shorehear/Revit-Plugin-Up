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
        Line selectedLine = null;


        private Label statusLabel;
        private Label infoLabel;
        private System.Windows.Controls.TextBox selectedElementsTextBox;
        private Button endSelecting;

        private bool continueSelecting = true;
        private bool settingsWindowBeenOpen = false;

        private string status;

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<Object[]> SettingsClosedWithSettings;

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

            Loaded += SelectionWindow_Loaded;
        }

        private void InitializeComponent()
        {
            Title = "Менеджер выбора элементов";
            Width = 600;
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
                Width = 200,
                Height = 20,
                Margin = new Thickness(0, 10, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            endSelecting.Click += endSelecting_Click;

            selectedElementsTextBox = new System.Windows.Controls.TextBox()
            {
                IsReadOnly = true,
                Margin = new Thickness(10, 10, 0, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 200,
                Width = 500
            };

            StackPanel panel = new StackPanel();
            panel.Children.Add(infoLabel);
            panel.Children.Add(statusLabel);
            panel.Children.Add(selectedElementsTextBox);
            panel.Children.Add(endSelecting);

            Content = panel;
            UpdateSelectedElementsTextBox();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSelectedElementsTextBox()
        {
            selectedElementsTextBox.Text = "Нет выбранных элементов";

            if (selectedElements != null && selectedElements.Any())
            {
                StringBuilder elementsText = new StringBuilder("Выбранные элементы:\n");
                foreach (var element in selectedElements)
                {
                    elementsText.Append(element.Name + " (" + element.ToString() + ")\n");
                }
                if (selectedLine != null)
                {
                    elementsText.Append("\n Выбрана линия направления");
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

        private void HandleElementSelection()
        {
            RequestElementSelection();
        }

        private void RequestElementSelection()
        {
            if (!continueSelecting || settingsWindowBeenOpen)
            {
                return;
            }

            Reference pickedRef = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element);
            if (pickedRef != null)
            {
                Element selectedElement = doc.GetElement(pickedRef.ElementId);

                if (selectedElement.Category.Id.IntegerValue == (int)BuiltInCategory.OST_Lines)
                {
                    if (selectedLine != null)
                    {
                        MessageBox.Show("Уже выбрана линия модели!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        selectedLine = ((CurveElement)selectedElement).GeometryCurve as Line;
                        UpdateSelectedElementsTextBox();
                    }
                }
                else
                {
                    if (!selectedElements.Any(elem => elem.Id == selectedElement.Id))
                    {
                        selectedElements.Add(selectedElement);
                        ElementSelectionEvent?.Invoke(this, selectedElement);
                        UpdateSelectedElementsTextBox();
                    }
                    else
                    {
                        MessageBox.Show("Этот элемент уже выбран!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }

                HandleElementSelection();
            }
            else
            {
                return;
            }
        }



        private void SelectionWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                InitializeComponent();
                HandleElementSelection();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Произошла ошибка при загрузке окна: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void endSelecting_Click(object sender, RoutedEventArgs e)
        {
            continueSelecting = false;
            RequestElementSelection();
            
            Hide();
            
            settingsWindowBeenOpen = true;
            SettingsWindow settingsWindow = new SettingsWindow(selectedElements, selectedLine, doc);
            settingsWindow.SettingsClosed += SettingsWindow_SettingsClosedWithSettings;
            settingsWindow.Show();
        }

        private void SettingsWindow_SettingsClosedWithSettings(object sender, Object[] settings)
        {
            settingsWindowBeenOpen = true;

            SettingsClosedWithSettings?.Invoke(this, settings);
            Close();
        }
    }
}
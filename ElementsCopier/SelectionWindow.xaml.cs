using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Collections.Generic;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Linq;
using Autodesk.Revit.UI.Selection;
using System;

namespace ElementsCopier
{
    public partial class SelectionWindow : Window, INotifyPropertyChanged
    {
        private Document doc;
        private ExternalCommandData commandData;
        private ICollection<ElementId> selectedIdElements;


        private Label statusLabel;
        private Label infoLabel;
        private System.Windows.Controls.TextBox selectedElementsTextBox;

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                }
            }
        }

        public SelectionWindow(Document doc, ExternalCommandData commandData)
        {
            this.doc = doc;
            this.commandData = commandData;

            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Title = "Менеджер копирования";
            Width = 400;
            Height = 400;

            infoLabel = new Label()
            {
                Content = "Выберите объекты, которые необходимо дублировать.\nДля вращения выберите направляющую линию модели.",
                Margin = new Thickness(10, 0, 0, 0)
            };

            statusLabel = new Label()
            {
                Content = "Статус: Ожидание выбора элементов...",
                Margin = new Thickness(10, 0, 0, 0)
            };

            selectedElementsTextBox = new System.Windows.Controls.TextBox()
            {
                IsReadOnly = true,
                Margin = new Thickness(10, 10, 0, 0),
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                Height = 100,
                Width = 300
            };

            StackPanel panel = new StackPanel();
            panel.Children.Add(infoLabel);
            panel.Children.Add(statusLabel);
            panel.Children.Add(selectedElementsTextBox);

            Content = panel;

            System.Windows.Data.Binding binding = new System.Windows.Data.Binding();
            binding.Path = new PropertyPath("-_-");
            binding.Source = this;
            statusLabel.SetBinding(Label.ContentProperty, binding);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void UpdateSelectedElementsTextBox()
        {
            if (selectedIdElements != null && selectedIdElements.Any())
            {
                string elementIdsText = "Выбранные элементы:\n";
                foreach (var elementId in selectedIdElements)
                {
                    elementIdsText += elementId.IntegerValue.ToString() + "\n";
                }

                selectedElementsTextBox.Text = elementIdsText;
            }
            else
            {
                selectedElementsTextBox.Text = "Нет выбранных элементов";
            }
        }

        public event EventHandler ElementSelectionEvent;

        private void RaiseElementSelectionEvent()
        {
            ElementSelectionEvent?.Invoke(this, EventArgs.Empty);
        }

        public void HandleElementSelection()
        {
            try
            {
                using (UIDocument uiDoc = new UIDocument(doc))
                {
                    Selection selection = uiDoc.Selection;

                    ICollection<ElementId> selectedIds = selection.GetElementIds();

                    selectedIdElements = selectedIds;

                    UpdateSelectedElementsTextBox();

                    Status = "Выбрано элементов: " + selectedIds.Count;

                    RaiseElementSelectionEvent();
                }
            }
            catch (Autodesk.Revit.Exceptions.ArgumentException ex)
            {
                Status = "Ошибка при получении выбранных элементов: " + ex.Message;
            }
            catch (Exception ex)
            {
                Status = "Ошибка при получении выбранных элементов: " + ex.Message;
            }
        }
    }
}
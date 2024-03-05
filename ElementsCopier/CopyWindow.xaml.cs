using System;
using System.Windows;
using Autodesk.Revit.DB;
using System.Collections.Generic;
using System.Windows.Controls;
using Autodesk.Revit.UI;
namespace ElementsCopier
{
    public partial class SelectElements : Window//, IDisposable
    {
        private Document doc;
        private List<Element> selectedElements = new List<Element>();
        private Element lineElement = null;
        private ICollection<ElementId> elementsIdList;
        ExternalCommandData commandData;
        private Label statusLabel;


        private System.Windows.Threading.DispatcherTimer selectionTimer;

        public SelectElements(ExternalCommandData commandData, Document doc)
        {
            this.doc = doc;
            this.commandData = commandData;
            InitializeComponent();
            Loaded += SelectElements_Loaded;

            selectionTimer = new System.Windows.Threading.DispatcherTimer();
            selectionTimer.Tick += SelectionTimer_Tick;
            selectionTimer.Interval = TimeSpan.FromMilliseconds(500); 
            selectionTimer.Start();
        }

        private void SelectionTimer_Tick(object sender, EventArgs e)
        {
            CheckSelectedElements();
        }

        private void CheckSelectedElements()
        {
            ICollection<ElementId> currentSelection = commandData.Application.ActiveUIDocument.Selection.GetElementIds();

            foreach (ElementId elementId in currentSelection)
            {
                Element element = doc.GetElement(elementId);
                if (!(element is CurveElement))
                {
                    selectedElements.Add(element);
                    UpdateStatusLabel();
                }
                else if (element is CurveElement && lineElement == null)
                {
                    lineElement = element;
                }
                else if (element is CurveElement && lineElement != null)
                {
                    MessageBox.Show("Ошибка. Нельзя выбрать более одной направляющей линии");
                    return;
                }
            }
        }

        private void UpdateStatusLabel()
        {
            statusLabel.Content = $"Статус: Выбрано элементов - {selectedElements.Count}";
        }


        private class IdlingEvent : IExternalEventHandler
        {
            private SelectElements window;

            public IdlingEvent(SelectElements window)
            {
                this.window = window;
            }

            public void Execute(UIApplication app) => window.CheckSelectedElements();

            public string GetName()
            {
                return "IdlingEvent";
            }
        }

        private void SelectElements_Loaded(object sender, RoutedEventArgs e)
        {
            CheckSelectedElements();
        }

        public void InitializeComponent()
        {
            Title = "CopyManager";
            Width = 400;
            Height = 400;

            var infoLabel = new Label()
            {
                Content = "Выберите объекты, которые необходимо дублировать.\nДля вращения выберите направляющую линию модели.",
                Margin = new Thickness(10, 0, 0, 0)
            };

            elementsIdList = commandData.Application.ActiveUIDocument.Selection.GetElementIds();

            statusLabel = new Label()
            {
                Content = "Статус: Ожидание выбора элементов...",
                Margin = new Thickness(10, 0, 0, 0)
            };

            StackPanel panel = new StackPanel();
            panel.Children.Add(infoLabel);
            panel.Children.Add(statusLabel);

            Content = panel;
        }
    }
}

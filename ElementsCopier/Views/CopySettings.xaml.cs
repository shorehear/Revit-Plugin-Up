using System;
using System.Windows;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using System.Windows.Controls;

namespace ElementsCopier
{
    public partial class SettingsWindow : Window
    {
        private Label selectedElementsLabel;
        private Label aboutCoordinatesLabel;
        private Label aboutDistanceLabel;
        private Label aboutQuantityLabel;

        private TextBox globalCoordinatesTextBox;
        private TextBox globalDistanceTextBox;
        private TextBox globalQuantityTextBox;

        private Button okButton;

        private Document doc;
        private List<Element> selectedElements;
        private Line selectedLine;

        public event EventHandler<Object[]> SettingsClosed;

        public SettingsWindow(List<Element> selectedElements, Line selectedLine, Document doc)
        {
            this.doc = doc;
            this.selectedElements = selectedElements;
            this.selectedLine = selectedLine;
            InitializeComponent(selectedElements);
        }

        private void InitializeComponent(List<Element> selectedElements)
        {
            Title = "Менеджер копирования";
            Width = 750;
            Height = 400;

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            StackPanel stackPanel = new StackPanel();

            selectedElementsLabel = new Label()
            {
                Content = "Выбранные элементы:",
                Margin = new Thickness(10, 10, 0, 0)
            };
            stackPanel.Children.Add(selectedElementsLabel);

            for (int i = 0; i < selectedElements.Count; i++)
            {
                Element element = selectedElements[i];

                Label elementLabel = new Label();
                elementLabel.Content = $"{element.Name} ({element.Id.IntegerValue})";
                elementLabel.Margin = new Thickness(10, 0, 0, 0);

                stackPanel.Children.Add(elementLabel);
            }

            aboutCoordinatesLabel = new Label()
            {
                Content = "Укажите координаты (X,Y,Z)",
                Margin = new Thickness(10, 10, 0, 0)
            };
            globalCoordinatesTextBox = new TextBox();
            globalCoordinatesTextBox.Margin = new Thickness(10, 0, 400, 0);

            aboutDistanceLabel = new Label()
            {
                Content = "Укажите дистанцию между копиями:",
                Margin = new Thickness(10, 10, 0, 0)
            };
            globalDistanceTextBox = new TextBox();
            globalDistanceTextBox.Margin = new Thickness(10, 0, 400, 0);

            aboutQuantityLabel = new Label()
            {
                Content = "Укажите количество копий:",
                Margin = new Thickness(10, 10, 0, 0)
            };
            globalQuantityTextBox = new TextBox();
            globalQuantityTextBox.Margin = new Thickness(10, 0, 400, 0);

            okButton = new Button()
            {
                Content = "Произвести копирование",
                Width = 175,
                Margin = new Thickness(0, 0, 10, 0),
                HorizontalAlignment = HorizontalAlignment.Right
            };
            okButton.Click += OkButton_Click;

            stackPanel.Children.Add(aboutCoordinatesLabel);
            stackPanel.Children.Add(globalCoordinatesTextBox);
            stackPanel.Children.Add(aboutDistanceLabel);
            stackPanel.Children.Add(globalDistanceTextBox);
            stackPanel.Children.Add(aboutQuantityLabel);
            stackPanel.Children.Add(globalQuantityTextBox);


            stackPanel.Children.Add(okButton);

            scrollViewer.Content = stackPanel;
            Content = scrollViewer;
        }


        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                XYZ coordinatesPoint;
                if (!string.IsNullOrWhiteSpace(globalCoordinatesTextBox.Text))
                {
                    string[] coordinates = globalCoordinatesTextBox.Text.Split(',');
                    double x, y, z;
                    if (coordinates.Length >= 3 &&
                        double.TryParse(coordinates[0], out x) &&
                        double.TryParse(coordinates[1], out y) &&
                        double.TryParse(coordinates[2], out z))
                    {
                        coordinatesPoint = new XYZ(x, y, z);
                    }
                    else
                    {
                        throw new FormatException("Неверный формат координат.");
                    }
                }
                else
                {
                    coordinatesPoint = XYZ.Zero;
                }

                double distance = string.IsNullOrWhiteSpace(globalDistanceTextBox.Text) ? 0.0 : double.Parse(globalDistanceTextBox.Text);

                int quantity = string.IsNullOrWhiteSpace(globalQuantityTextBox.Text) ? 1 : int.Parse(globalQuantityTextBox.Text);

                Object[] settings = new Object[] { selectedElements, selectedLine, coordinatesPoint, distance, quantity, doc };
                SettingsClosed?.Invoke(this, settings);

                Close(); 
            }
            catch (FormatException ex)
            {
                MessageBox.Show($"Ошибка формата ввода: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка:{ex.Message}");
            }
        }
    }
}

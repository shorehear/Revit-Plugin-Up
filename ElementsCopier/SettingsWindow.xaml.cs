using System.Windows;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using System.Windows.Controls;
using Grid = System.Windows.Controls.Grid;

namespace ElementsCopier
{
    public partial class SettingsWindow : Window
    {
        private Label aboutSettings;
        public SettingsWindow(List<Element> selectedElements)
        {
            InitializeComponent(selectedElements);
        }

        private void InitializeComponent(List<Element> selectedElements)
        {
            Title = "Менеджер копирования";
            Width = 750;
            Height = 400;

            Grid grid = new Grid();

            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            aboutSettings = new Label()
            {
                Content = "Выбранные элементы:    Координаты копирования:    Дистанция между копиями:    Количество копий:",
                Margin = new Thickness(0, 0, 0, 0)
            };

            grid.Children.Add(aboutSettings);
            Grid.SetColumnSpan(aboutSettings, 7); 

            for (int i = 0; i < selectedElements.Count; i++)
            {
                Element element = selectedElements[i];

                grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                Label elementLabel = new Label();
                elementLabel.Content = $"{element.Name} ({element.Id.IntegerValue})";
                Grid.SetColumn(elementLabel, 0);
                Grid.SetRow(elementLabel, i + 1);

                TextBox copiesTextBox = new TextBox();
                copiesTextBox.Width = 50;
                copiesTextBox.Margin = new Thickness(10, 0, 0, 0);
                copiesTextBox.Tag = element;
                Grid.SetColumn(copiesTextBox, 1);
                Grid.SetRow(copiesTextBox, i + 1);

                //координаты копирования
                TextBox coordinatesTextBox = new TextBox();
                coordinatesTextBox.Width = 100;
                coordinatesTextBox.Margin = new Thickness(10, 0, 0, 0);
                Grid.SetColumn(coordinatesTextBox, 2); 
                Grid.SetRow(coordinatesTextBox, i + 1);

                //дистанция между копиями
                TextBox distanceTextBox = new TextBox();
                distanceTextBox.Width = 100;
                distanceTextBox.Margin = new Thickness(10, 0, 0, 0);
                Grid.SetColumn(distanceTextBox, 3); 
                Grid.SetRow(distanceTextBox, i + 1);

                //количество копий
                TextBox quantityTextBox = new TextBox();
                quantityTextBox.Width = 50;
                quantityTextBox.Margin = new Thickness(10, 0, 0, 0);
                Grid.SetColumn(quantityTextBox, 4);
                Grid.SetRow(quantityTextBox, i + 1);

                Button decreaseButton = new Button();
                decreaseButton.Content = "-";
                decreaseButton.Width = 20;
                decreaseButton.Click += (sender, e) =>
                {
                    int currentValue;
                    if (int.TryParse(copiesTextBox.Text, out currentValue))
                    {
                        copiesTextBox.Text = (currentValue - 1).ToString();
                    }
                };
                decreaseButton.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(decreaseButton, 5); 
                Grid.SetRow(decreaseButton, i + 1);

                Button increaseButton = new Button();
                increaseButton.Content = "+";
                increaseButton.Width = 20;
                increaseButton.Click += (sender, e) =>
                {
                    int currentValue;
                    if (int.TryParse(copiesTextBox.Text, out currentValue))
                    {
                        copiesTextBox.Text = (currentValue + 1).ToString();
                    }
                };
                increaseButton.VerticalAlignment = VerticalAlignment.Center;
                Grid.SetColumn(increaseButton, 6); 
                Grid.SetRow(increaseButton, i + 1);

                grid.Children.Add(elementLabel);
                grid.Children.Add(copiesTextBox);
                grid.Children.Add(coordinatesTextBox);
                grid.Children.Add(distanceTextBox);
                grid.Children.Add(quantityTextBox);
                grid.Children.Add(decreaseButton);
                grid.Children.Add(increaseButton);
            }

            ScrollViewer scrollViewer = new ScrollViewer();
            scrollViewer.Content = grid;

            Content = scrollViewer;
        }
    }
}

using System.Windows;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using TextBox = System.Windows.Controls.TextBox;
using System;


namespace Elements_Copier
{
    public partial class CopiedElementsWindow : Window
    {
        private readonly CopiedElementsViewModel _viewModel;

        public CopiedElementsWindow(SelectedElementsData selectedElementsData, int optionsOfOperation, Document doc, UIDocument uidoc)
        {
            InitializeComponent();

            CopiedElementsData copiedElementsData = new CopiedElementsData(selectedElementsData);
            _viewModel = new CopiedElementsViewModel(copiedElementsData, doc, uidoc);
            DataContext = _viewModel;

            //_viewModel.EndSetSettings += StartElementsCopier;
        }

        //private void StartElementsCopier(object sender, EventArgs e)
        //{

        //}
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (textBox.Text == "(X, Y, Z)" || textBox.Text == "0")
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (string.IsNullOrWhiteSpace(textBox.Text))
            {
                if (textBox == N)
                {
                    textBox.Text = "0";
                }
                else
                {
                    textBox.Text = "(X, Y, Z)";
                }
            }
        }


    }

}

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
        private Document doc;
        private UIDocument uidoc;
        private int optionsOfOperation;
        private CopiedElementsData copiedElementsData;
        public CopiedElementsWindow(SelectedElementsData selectedElementsData, int optionsOfOperation, Document doc, UIDocument uidoc)
        {
            this.doc = doc;
            this.uidoc = uidoc;
            this.optionsOfOperation = optionsOfOperation;
            InitializeComponent();

            copiedElementsData = new CopiedElementsData(selectedElementsData);
            _viewModel = new CopiedElementsViewModel(copiedElementsData, optionsOfOperation, doc, uidoc);
            DataContext = _viewModel;

            _viewModel.EndSettings += StartElementsCopier;
        }

        private void StartElementsCopier(object sender, EventArgs e)
        {
            TaskDialog.Show("Data", $"{copiedElementsData.SelectedLine.GetEndPoint(0)}, {optionsOfOperation}");
            Close();

            //ElementsCopier(copiedElementsData, optionsOfOperation, coordinatesOfCopies, numberOfCopies, distanceBetweenCopies);
        }
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

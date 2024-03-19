using System.Windows;
using System.Windows.Controls;

namespace Elements_Copier
{
    public partial class CopiedElementsWindow : Window
    {
        public CopiedElementsWindow()
        {
            InitializeComponent();
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

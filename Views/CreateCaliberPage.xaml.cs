using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace Budweg.Views
{
    public partial class CreateCaliberPage : Page
    {
        private string? _selectedImagePath;

        public CreateCaliberPage()
        {
            InitializeComponent();
            TypeBox.SelectedIndex = 0; // "Vælg type"
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Close the parent window that hosts this page
            var win = Window.GetWindow(this);
            if (win != null)
                win.Close();
        }

        private void PickImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                _selectedImagePath = dlg.FileName;
                SelectedImageText.Text = System.IO.Path.GetFileName(_selectedImagePath);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var stelnummer = StelnummerBox.Text;
            var type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            var producent = ProducentBox.Text;
            var kommentar = KommentarBox.Text;

            // TODO: replace with your real persistence layer
            MessageBox.Show($"Gem:\n{stelnummer}\n{type}\n{producent}\n{kommentar}\nImage: {_selectedImagePath}");
        }
    }
}

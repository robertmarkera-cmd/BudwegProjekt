using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using Budweg.Models;
using Budweg.View_Models;

namespace Budweg.Views
{
    public partial class CreateCaliberPage : Page
    {
        private string? _selectedImagePath;

        public CreateCaliberPage()
        {
            InitializeComponent();
            TypeBox.SelectedIndex = 0;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this)?.Close();
        }

        private void PickImage_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "Image files|*.png;*.jpg;*.jpeg;*.bmp;*.gif"
            };

            if (dlg.ShowDialog() == true)
            {
                _selectedImagePath = dlg.FileName;
                SelectedImageText.Text = Path.GetFileName(_selectedImagePath);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // ItemNumber
            if (!int.TryParse(StelnummerBox.Text.Trim(), out int frameId))
            {
                MessageBox.Show("Stelnummer skal være et tal!");
                return;
            }

            // BatchID (IKKE NULL!)
            if (!int.TryParse(BatchBox.Text.Trim(), out int batchId))
            {
                MessageBox.Show("BatchID skal være et tal, og må ikke være tom!");
                return;
            }

            var type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

            var cal = new Caliper
            {
                FrameID = frameId,
                BatchID = batchId,
                Type = type,
                Manufacturer = ProducentBox.Text.Trim(),
                Comment = KommentarBox.Text.Trim()
            };

            if (_selectedImagePath != null && File.Exists(_selectedImagePath))
                cal.Picture = new System.Drawing.Bitmap(_selectedImagePath);

            var repo = new CaliberRepository();

            try
            {
                repo.AddCaliber(cal);
                MessageBox.Show($"Kaliber oprettet! Nyt ID: {cal.FrameID}");
                Window.GetWindow(this)?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Fejl: " + ex.Message);
            }
        }
    }
}
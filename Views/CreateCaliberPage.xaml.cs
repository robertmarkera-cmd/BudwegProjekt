using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;
using System.IO;
using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

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
            // Read values from UI
            var stelnummerText = StelnummerBox.Text?.Trim();
            var type = (TypeBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "";
            var producent = ProducentBox.Text?.Trim();
            var kommentar = KommentarBox.Text?.Trim();
            var batchText = string.Empty;
            try { batchText = BatchBox?.Text?.Trim() ?? string.Empty; } catch { batchText = string.Empty; }

            // Basic validation
            if (string.IsNullOrEmpty(type) || type == "Vælg type")
            {
                MessageBox.Show("Vælg en gyldig type.", "Validering", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(stelnummerText, out var itemNumber))
            {
                MessageBox.Show("Stelnummer skal være et heltal (itemnummer).", "Validering", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            int? batchId = null;
            if (int.TryParse(batchText, out var parsedBatch))
                batchId = parsedBatch;

            // Read image bytes if provided
            byte[]? imageBytes = null;
            if (!string.IsNullOrEmpty(_selectedImagePath) && File.Exists(_selectedImagePath))
            {
                try
                {
                    imageBytes = File.ReadAllBytes(_selectedImagePath);
                }
                catch
                {
                    MessageBox.Show("Kunne ikke indlæse billedet. Tjek filen.", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // Load connection string from jsconfig1.json (same pattern as repositories)
            IConfigurationRoot config;
            try
            {
                config = new ConfigurationBuilder()
                    .AddJsonFile("jsconfig1.json", optional: false, reloadOnChange: false)
                    .Build();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Kunne ikke læse konfiguration: {ex.Message}", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var connectionString = config.GetConnectionString("MyDBConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Connection string 'MyDBConnection' ikke fundet i jsconfig1.json.", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using var con = new SqlConnection(connectionString);
                using var cmd = new SqlCommand("dbo.CreateCaliber", con) { CommandType = CommandType.StoredProcedure };

                cmd.Parameters.Add("@Type", SqlDbType.NVarChar, 80).Value = (object)type ?? DBNull.Value;
                cmd.Parameters.Add("@Comment", SqlDbType.NVarChar, 255).Value = string.IsNullOrEmpty(kommentar) ? (object)DBNull.Value : kommentar;
                cmd.Parameters.Add("@Picture", SqlDbType.VarBinary, -1).Value = imageBytes != null ? (object)imageBytes : DBNull.Value;
                cmd.Parameters.Add("@ItemNumber", SqlDbType.Int).Value = itemNumber;
                cmd.Parameters.Add("@Brand", SqlDbType.NVarChar, 80).Value = string.IsNullOrEmpty(producent) ? (object)DBNull.Value : producent;
                if (batchId.HasValue)
                    cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = batchId.Value;
                else
                    cmd.Parameters.Add("@BatchID", SqlDbType.Int).Value = DBNull.Value;

                con.Open();
                var result = cmd.ExecuteScalar();
                int newCaliberId = result != null && int.TryParse(result.ToString(), out var tmp) ? tmp : 0;

                MessageBox.Show($"Kaliber gemt. Nyt ID: {newCaliberId}", "Succes", MessageBoxButton.OK, MessageBoxImage.Information);

                var win = Window.GetWindow(this);
                win?.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Fejl ved gem: {ex.Message}", "Fejl", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }
    }
}

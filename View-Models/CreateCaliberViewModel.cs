using Budweg.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Budweg.Commands;

namespace Budweg.View_Models
{
    public class CreateCaliberViewModel : INotifyPropertyChanged
    {
        private string _stelnummer = string.Empty;
        private string _type = string.Empty;
        private string _producent = string.Empty;
        private string _kommentar = string.Empty;
        private string _selectedImagePath = string.Empty;

        private readonly CaliberRepository _repo = new CaliberRepository();

        public string Stelnummer { get => _stelnummer; set { _stelnummer = value; OnPropertyChanged(nameof(Stelnummer)); } }
        public string Type { get => _type; set { _type = value; OnPropertyChanged(nameof(Type)); } }
        public string Producent { get => _producent; set { _producent = value; OnPropertyChanged(nameof(Producent)); } }
        public string Kommentar { get => _kommentar; set { _kommentar = value; OnPropertyChanged(nameof(Kommentar)); } }
        public string SelectedImagePath { get => _selectedImagePath; set { _selectedImagePath = value; OnPropertyChanged(nameof(SelectedImagePath)); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateCaliberViewModel()
        {
            SaveCommand = new RelayCommand(param =>
            {
                try
                {
                    var cal = new Caliper
                    {
                        Type = this.Type,
                        Manufacturer = this.Producent,
                        Comment = this.Kommentar,
                        Picture = this.SelectedImagePath,
                        FrameID = int.TryParse(this.Stelnummer, out var id) ? id : 0
                    };

                    _repo.AddCaliber(cal);

                    if (param is Window w)
                        w.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fejl ved gem: " + ex.Message);
                }
            });

            CancelCommand = new RelayCommand(param =>
            {
                if (param is Window w)
                    w.Close();
            });
            // Note: Image picking removed - kept only create/save functionality
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

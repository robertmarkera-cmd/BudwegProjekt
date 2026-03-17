using Budweg.Models;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Budweg.Commands;
using Budweg.Repositories;

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
        private readonly BatchRepository _batchRepo = new BatchRepository();

        private System.Collections.ObjectModel.ObservableCollection<Batch> _batches = new System.Collections.ObjectModel.ObservableCollection<Batch>();
        private Batch? _selectedBatch;

        public string Stelnummer { get => _stelnummer; set { _stelnummer = value; OnPropertyChanged(nameof(Stelnummer)); } }
        public string Type { get => _type; set { _type = value; OnPropertyChanged(nameof(Type)); } }
        public string Producent { get => _producent; set { _producent = value; OnPropertyChanged(nameof(Producent)); } }
        public string Kommentar { get => _kommentar; set { _kommentar = value; OnPropertyChanged(nameof(Kommentar)); } }
        public string SelectedImagePath { get => _selectedImagePath; set { _selectedImagePath = value; OnPropertyChanged(nameof(SelectedImagePath)); } }

        public System.Collections.ObjectModel.ObservableCollection<Batch> Batches { get => _batches; set { _batches = value; OnPropertyChanged(nameof(Batches)); } }
        public Batch? SelectedBatch { get => _selectedBatch; set { _selectedBatch = value; OnPropertyChanged(nameof(SelectedBatch)); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public CreateCaliberViewModel()
        {
            // load existing batches for selection
            try
            {
                var list = _batchRepo.GetAll();
                foreach (var b in list) Batches.Add(b);
            }
            catch { /* ignore load errors; UI will show no batches */ }
            SaveCommand = new SaveCommand(param =>
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

                    // require a selected batch
                    if (SelectedBatch == null)
                    {
                        MessageBox.Show("Vælg en Batch før du gemmer.");
                        return;
                    }

                    // set BatchID via FrameID property (existing model)
                    cal.FrameID = SelectedBatch.BatchID;

                    _repo.AddCaliber(cal);

                    if (param is Window w)
                        w.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Fejl ved gem: " + ex.Message);
                }
            });

            CancelCommand = new CancelCommand(param =>
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

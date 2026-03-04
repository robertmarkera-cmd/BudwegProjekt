
using System.Windows;
using System.Windows.Controls;

namespace Budweg.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            // open CreateCaliberPage in a new Window
            var page = new CreateCaliberPage();
            var win = new Window
            {
                Title = "Opret Kaliber",
                Content = page,
                Owner = this,
                Width = 380,
                Height = 700,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                ResizeMode = ResizeMode.NoResize
            };

            win.ShowDialog();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
            => Application.Current.Shutdown();
    }
}

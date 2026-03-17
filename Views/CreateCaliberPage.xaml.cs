using System.Windows;
using System.Windows.Controls;
using Budweg.View_Models;

namespace Budweg.Views
{
    public partial class CreateCaliberPage : Page
    {
        public CreateCaliberPage()
        {
            InitializeComponent();
            // only set DataContext from code-behind
            this.DataContext = new CreateCaliberViewModel();
        }
    }
}

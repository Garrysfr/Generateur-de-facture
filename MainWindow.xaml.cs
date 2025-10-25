using GenerateurFactures.ViewModels;
using System.Windows;

namespace GenerateurFactures
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}
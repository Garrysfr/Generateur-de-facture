using GenerateurFactures.Models;
using GenerateurFactures.ViewModels;
using System.Windows;

namespace GenerateurFactures
{
    public partial class ApercuFactureWindow : Window
    {
        public ApercuFactureWindow(Facture facture)
        {
            InitializeComponent();
            DataContext = new ApercuFactureViewModel(facture);
        }

        private void Fermer_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
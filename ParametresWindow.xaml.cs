using GenerateurFactures.Models;
using GenerateurFactures.Services;
using System.ComponentModel;
using System.Windows;

namespace GenerateurFactures
{
    public partial class ParametresWindow : Window, INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private Parametres _parametres;

        public Parametres Parametres
        {
            get => _parametres;
            set
            {
                if (_parametres != value)
                {
                    _parametres = value;
                    OnPropertyChanged(nameof(Parametres));
                }
            }
        }

        public ParametresWindow(SettingsService settingsService)
        {
            InitializeComponent();
            _settingsService = settingsService;

            Parametres = _settingsService.ChargerParametres() ?? new Parametres();
            DataContext = this;
        }

        private void Sauvegarder_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _settingsService.SauvegarderParametres(Parametres);
                DialogResult = true;
                Close();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur lors de la sauvegarde : {ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Annuler_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
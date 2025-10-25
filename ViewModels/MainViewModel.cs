using GenerateurFactures.Models;
using GenerateurFactures.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace GenerateurFactures.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly SettingsService _settingsService;
        private readonly PdfService _pdfService;
        private Facture _factureActuelle;

        public Facture FactureActuelle
        {
            get => _factureActuelle;
            set
            {
                if (_factureActuelle != value)
                {
                    _factureActuelle = value;
                    OnPropertyChanged(nameof(FactureActuelle));
                }
            }
        }

        public ICommand AjouterProduitCommand { get; }
        public ICommand SupprimerProduitCommand { get; }
        public ICommand NouvelleFactureCommand { get; }
        public ICommand GenererPdfCommand { get; }
        public ICommand OuvrirParametresCommand { get; }

        public MainViewModel()
        {
            _settingsService = new SettingsService();
            _pdfService = new PdfService();

            FactureActuelle = new Facture();
            ChargerParametresParDefaut();

            AjouterProduitCommand = new RelayCommand(AjouterProduit);
            SupprimerProduitCommand = new RelayCommand<Produit>(SupprimerProduit);
            NouvelleFactureCommand = new RelayCommand(NouvelleFacture);
            GenererPdfCommand = new RelayCommand(GenererPdf);
            OuvrirParametresCommand = new RelayCommand(OuvrirParametres);
        }

        private void ChargerParametresParDefaut()
        {
            var parametres = _settingsService.ChargerParametres();
            if (parametres != null)
            {
                FactureActuelle.Entreprise = parametres.Entreprise ?? new Entreprise();
                FactureActuelle.TauxTva = parametres.TauxTvaParDefaut;
            }
        }

        private void AjouterProduit()
        {
            FactureActuelle.Produits.Add(new Produit());
        }

        private void SupprimerProduit(Produit? produit)
        {
            if (produit != null && FactureActuelle.Produits.Contains(produit))
            {
                FactureActuelle.Produits.Remove(produit);
            }
        }

        private void NouvelleFacture()
        {
            var parametres = _settingsService.ChargerParametres();
            FactureActuelle = new Facture();
            if (parametres != null)
            {
                FactureActuelle.Entreprise = parametres.Entreprise ?? new Entreprise();
                FactureActuelle.TauxTva = parametres.TauxTvaParDefaut;
            }
        }

        private void GenererPdf()
        {
            try
            {
                if (FactureActuelle.Produits.Count == 0)
                {
                    MessageBox.Show("Veuillez ajouter au moins un produit avant de générer la facture.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                if (string.IsNullOrWhiteSpace(FactureActuelle.Client.Nom))
                {
                    MessageBox.Show("Veuillez renseigner le nom du client.", "Erreur", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Ouvrir la fenêtre d'aperçu
                var apercuWindow = new ApercuFactureWindow(FactureActuelle);
                apercuWindow.Owner = Application.Current.MainWindow;
                apercuWindow.ShowDialog();
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'ouverture de l'aperçu :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OuvrirParametres()
        {
            var parametresWindow = new ParametresWindow(_settingsService);
            if (parametresWindow.ShowDialog() == true)
            {
                ChargerParametresParDefaut();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly System.Action _execute;
        private readonly System.Func<bool>? _canExecute;

        public RelayCommand(System.Action execute, System.Func<bool>? canExecute = null)
        {
            _execute = execute ?? throw new System.ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event System.EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke() ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute();
        }
    }

    public class RelayCommand<T> : ICommand
    {
        private readonly System.Action<T?> _execute;
        private readonly System.Func<T?, bool>? _canExecute;

        public RelayCommand(System.Action<T?> execute, System.Func<T?, bool>? canExecute = null)
        {
            _execute = execute ?? throw new System.ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        public event System.EventHandler? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke((T?)parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute((T?)parameter);
        }
    }
}
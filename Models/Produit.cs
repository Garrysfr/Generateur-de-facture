using System.ComponentModel;

namespace GenerateurFactures.Models
{
    public class Produit : INotifyPropertyChanged
    {
        private string _nom = string.Empty;
        private decimal _prix;
        private int _quantite = 1;

        public string Nom
        {
            get => _nom;
            set
            {
                if (_nom != value)
                {
                    _nom = value;
                    OnPropertyChanged(nameof(Nom));
                }
            }
        }

        public decimal Prix
        {
            get => _prix;
            set
            {
                if (_prix != value)
                {
                    _prix = value;
                    OnPropertyChanged(nameof(Prix));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public int Quantite
        {
            get => _quantite;
            set
            {
                if (_quantite != value)
                {
                    _quantite = value;
                    OnPropertyChanged(nameof(Quantite));
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public decimal Total => Prix * Quantite;

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
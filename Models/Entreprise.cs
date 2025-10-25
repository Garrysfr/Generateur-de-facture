using System.ComponentModel;

namespace GenerateurFactures.Models
{
    public class Entreprise : INotifyPropertyChanged
    {
        private string _nom = string.Empty;
        private string _adresse = string.Empty;
        private string _codePostal = string.Empty;
        private string _ville = string.Empty;
        private string _siret = string.Empty;
        private string _email = string.Empty;
        private string _telephone = string.Empty;

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

        public string Adresse
        {
            get => _adresse;
            set
            {
                if (_adresse != value)
                {
                    _adresse = value;
                    OnPropertyChanged(nameof(Adresse));
                }
            }
        }

        public string CodePostal
        {
            get => _codePostal;
            set
            {
                if (_codePostal != value)
                {
                    _codePostal = value;
                    OnPropertyChanged(nameof(CodePostal));
                }
            }
        }

        public string Ville
        {
            get => _ville;
            set
            {
                if (_ville != value)
                {
                    _ville = value;
                    OnPropertyChanged(nameof(Ville));
                }
            }
        }

        public string Siret
        {
            get => _siret;
            set
            {
                if (_siret != value)
                {
                    _siret = value;
                    OnPropertyChanged(nameof(Siret));
                }
            }
        }

        public string Email
        {
            get => _email;
            set
            {
                if (_email != value)
                {
                    _email = value;
                    OnPropertyChanged(nameof(Email));
                }
            }
        }

        public string Telephone
        {
            get => _telephone;
            set
            {
                if (_telephone != value)
                {
                    _telephone = value;
                    OnPropertyChanged(nameof(Telephone));
                }
            }
        }

        public string AdresseComplete => $"{Adresse}\n{CodePostal} {Ville}";

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
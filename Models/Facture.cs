using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GenerateurFactures.Models
{
    public class Facture : INotifyPropertyChanged
    {
        private string _numero = string.Empty;
        private DateTime _date = DateTime.Now;
        private Client _client = new Client();
        private Entreprise _entreprise = new Entreprise();
        private decimal _reduction;
        private decimal _tauxTva = 20.0m;
        private ObservableCollection<Produit> _produits = new ObservableCollection<Produit>();

        public string Numero
        {
            get => _numero;
            set
            {
                if (_numero != value)
                {
                    _numero = value;
                    OnPropertyChanged(nameof(Numero));
                }
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public Client Client
        {
            get => _client;
            set
            {
                if (_client != value)
                {
                    _client = value;
                    OnPropertyChanged(nameof(Client));
                }
            }
        }

        public Entreprise Entreprise
        {
            get => _entreprise;
            set
            {
                if (_entreprise != value)
                {
                    _entreprise = value;
                    OnPropertyChanged(nameof(Entreprise));
                }
            }
        }

        public decimal Reduction
        {
            get => _reduction;
            set
            {
                if (_reduction != value)
                {
                    _reduction = value;
                    OnPropertyChanged(nameof(Reduction));
                    OnPropertyChanged(nameof(SousTotal));
                    OnPropertyChanged(nameof(MontantTva));
                    OnPropertyChanged(nameof(TotalTTC));
                }
            }
        }

        public decimal TauxTva
        {
            get => _tauxTva;
            set
            {
                if (_tauxTva != value)
                {
                    _tauxTva = value;
                    OnPropertyChanged(nameof(TauxTva));
                    OnPropertyChanged(nameof(MontantTva));
                    OnPropertyChanged(nameof(TotalTTC));
                }
            }
        }

        public ObservableCollection<Produit> Produits
        {
            get => _produits;
            set
            {
                if (_produits != value)
                {
                    if (_produits != null)
                    {
                        foreach (var produit in _produits)
                        {
                            produit.PropertyChanged -= OnProduitPropertyChanged;
                        }
                        _produits.CollectionChanged -= OnProduitsCollectionChanged;
                    }

                    _produits = value;

                    if (_produits != null)
                    {
                        foreach (var produit in _produits)
                        {
                            produit.PropertyChanged += OnProduitPropertyChanged;
                        }
                        _produits.CollectionChanged += OnProduitsCollectionChanged;
                    }

                    OnPropertyChanged(nameof(Produits));
                    RecalculerTotaux();
                }
            }
        }

        public decimal TotalHT => Produits?.Sum(p => p.Total) ?? 0;
        public decimal SousTotal => TotalHT - Reduction;
        public decimal MontantTva => SousTotal * (TauxTva / 100);
        public decimal TotalTTC => SousTotal + MontantTva;

        public Facture()
        {
            _produits.CollectionChanged += OnProduitsCollectionChanged;
            GenerateNumero();
        }

        private void GenerateNumero()
        {
            Numero = $"F{DateTime.Now:yyyyMMdd}-{DateTime.Now:HHmmss}";
        }

        private void OnProduitsCollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (Produit produit in e.NewItems)
                {
                    produit.PropertyChanged += OnProduitPropertyChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (Produit produit in e.OldItems)
                {
                    produit.PropertyChanged -= OnProduitPropertyChanged;
                }
            }

            RecalculerTotaux();
        }

        private void OnProduitPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Produit.Total))
            {
                RecalculerTotaux();
            }
        }

        private void RecalculerTotaux()
        {
            OnPropertyChanged(nameof(TotalHT));
            OnPropertyChanged(nameof(SousTotal));
            OnPropertyChanged(nameof(MontantTva));
            OnPropertyChanged(nameof(TotalTTC));
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
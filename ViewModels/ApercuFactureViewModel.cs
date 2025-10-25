using GenerateurFactures.Models;
using GenerateurFactures.Services;
using System.ComponentModel;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

namespace GenerateurFactures.ViewModels
{
    public class ApercuFactureViewModel : INotifyPropertyChanged
    {
        private readonly PdfService _pdfService;
        private Facture _facture;

        public Facture Facture
        {
            get => _facture;
            set
            {
                if (_facture != value)
                {
                    _facture = value;
                    OnPropertyChanged(nameof(Facture));
                    OnPropertyChanged(nameof(ShowReduction));
                }
            }
        }

        public bool ShowReduction => Facture?.Reduction > 0;

        public ICommand ImprimerCommand { get; }
        public ICommand TelechargerCommand { get; }

        public ApercuFactureViewModel(Facture facture)
        {
            _pdfService = new PdfService();
            Facture = facture;

            ImprimerCommand = new RelayCommand(Imprimer);
            TelechargerCommand = new RelayCommand(Telecharger);
        }

        private void Imprimer()
        {
            try
            {
                var printDialog = new PrintDialog();
                if (printDialog.ShowDialog() == true)
                {
                    // Créer un document d'impression
                    var flowDocument = CreerDocumentImpression();

                    // Configurer la pagination
                    var paginator = ((IDocumentPaginatorSource)flowDocument).DocumentPaginator;

                    // Imprimer
                    printDialog.PrintDocument(paginator, $"Facture {Facture.Numero}");

                    MessageBox.Show("Impression lancée avec succès.", "Impression", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur lors de l'impression :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Telecharger()
        {
            try
            {
                var dialog = new Microsoft.Win32.SaveFileDialog
                {
                    Filter = "Fichiers PDF|*.pdf",
                    FileName = $"Facture_{Facture.Numero}.pdf"
                };

                if (dialog.ShowDialog() == true)
                {
                    _pdfService.GenererFacturePdf(Facture, dialog.FileName);

                    var result = MessageBox.Show($"Facture sauvegardée avec succès :\n{dialog.FileName}\n\nVoulez-vous ouvrir le fichier ?",
                                               "Succès", MessageBoxButton.YesNo, MessageBoxImage.Information);

                    if (result == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = dialog.FileName,
                            UseShellExecute = true
                        });
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show($"Erreur lors de la génération du PDF :\n{ex.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private FlowDocument CreerDocumentImpression()
        {
            var flowDocument = new FlowDocument();
            flowDocument.PagePadding = new Thickness(50);

            // En-tête
            var enteteTable = new Table();
            enteteTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            enteteTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            var enteteRowGroup = new TableRowGroup();
            var enteteRow = new TableRow();

            // Informations entreprise
            var entrepriseCell = new TableCell();
            entrepriseCell.Blocks.Add(new Paragraph(new Run(Facture.Entreprise.Nom) { FontWeight = FontWeights.Bold, FontSize = 16 }));
            entrepriseCell.Blocks.Add(new Paragraph(new Run(Facture.Entreprise.Adresse)));
            entrepriseCell.Blocks.Add(new Paragraph(new Run($"{Facture.Entreprise.CodePostal} {Facture.Entreprise.Ville}")));
            if (!string.IsNullOrWhiteSpace(Facture.Entreprise.Siret))
                entrepriseCell.Blocks.Add(new Paragraph(new Run($"SIRET: {Facture.Entreprise.Siret}") { FontSize = 10 }));

            // Informations facture
            var factureCell = new TableCell();
            factureCell.TextAlignment = TextAlignment.Right;
            factureCell.Blocks.Add(new Paragraph(new Run("FACTURE") { FontWeight = FontWeights.Bold, FontSize = 20 }));
            factureCell.Blocks.Add(new Paragraph(new Run($"N° {Facture.Numero}") { FontWeight = FontWeights.Bold }));
            factureCell.Blocks.Add(new Paragraph(new Run($"Date: {Facture.Date:dd/MM/yyyy}")));

            enteteRow.Cells.Add(entrepriseCell);
            enteteRow.Cells.Add(factureCell);
            enteteRowGroup.Rows.Add(enteteRow);
            enteteTable.RowGroups.Add(enteteRowGroup);

            flowDocument.Blocks.Add(enteteTable);
            flowDocument.Blocks.Add(new Paragraph(new Run(" ")));

            // Informations client
            flowDocument.Blocks.Add(new Paragraph(new Run("Facturé à :") { FontWeight = FontWeights.Bold }));
            flowDocument.Blocks.Add(new Paragraph(new Run(Facture.Client.Nom) { FontWeight = FontWeights.Medium }));
            flowDocument.Blocks.Add(new Paragraph(new Run(Facture.Client.Adresse)));
            flowDocument.Blocks.Add(new Paragraph(new Run($"{Facture.Client.CodePostal} {Facture.Client.Ville}")));
            if (!string.IsNullOrWhiteSpace(Facture.Client.Email))
                flowDocument.Blocks.Add(new Paragraph(new Run($"Email: {Facture.Client.Email}")));

            flowDocument.Blocks.Add(new Paragraph(new Run(" ")));

            // Tableau des produits
            var produitsTable = new Table();
            produitsTable.Columns.Add(new TableColumn { Width = new GridLength(3, GridUnitType.Star) });
            produitsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            produitsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });
            produitsTable.Columns.Add(new TableColumn { Width = new GridLength(1, GridUnitType.Star) });

            // En-têtes
            var headerRowGroup = new TableRowGroup();
            var headerRow = new TableRow();
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Produit") { FontWeight = FontWeights.Bold })));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Prix unitaire") { FontWeight = FontWeights.Bold }) { TextAlignment = TextAlignment.Right }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Quantité") { FontWeight = FontWeights.Bold }) { TextAlignment = TextAlignment.Center }));
            headerRow.Cells.Add(new TableCell(new Paragraph(new Run("Total") { FontWeight = FontWeights.Bold }) { TextAlignment = TextAlignment.Right }));
            headerRowGroup.Rows.Add(headerRow);
            produitsTable.RowGroups.Add(headerRowGroup);

            // Lignes de produits
            var produitsRowGroup = new TableRowGroup();
            foreach (var produit in Facture.Produits)
            {
                var row = new TableRow();
                row.Cells.Add(new TableCell(new Paragraph(new Run(produit.Nom))));
                row.Cells.Add(new TableCell(new Paragraph(new Run($"{produit.Prix:C}")) { TextAlignment = TextAlignment.Right }));
                row.Cells.Add(new TableCell(new Paragraph(new Run(produit.Quantite.ToString())) { TextAlignment = TextAlignment.Center }));
                row.Cells.Add(new TableCell(new Paragraph(new Run($"{produit.Total:C}")) { TextAlignment = TextAlignment.Right }));
                produitsRowGroup.Rows.Add(row);
            }
            produitsTable.RowGroups.Add(produitsRowGroup);

            flowDocument.Blocks.Add(produitsTable);
            flowDocument.Blocks.Add(new Paragraph(new Run(" ")));

            // Totaux
            var list = new List();
            list.ListItems.Add(new ListItem(new Paragraph(new Run($"Total HT : {Facture.TotalHT:C}"))));
            if (Facture.Reduction > 0)
            {
                list.ListItems.Add(new ListItem(new Paragraph(new Run($"Réduction : -{Facture.Reduction:C}"))));
                list.ListItems.Add(new ListItem(new Paragraph(new Run($"Sous-total : {Facture.SousTotal:C}"))));
            }
            list.ListItems.Add(new ListItem(new Paragraph(new Run($"TVA ({Facture.TauxTva:N1}%) : {Facture.MontantTva:C}"))));
            list.ListItems.Add(new ListItem(new Paragraph(new Run($"Total TTC : {Facture.TotalTTC:C}") { FontWeight = FontWeights.Bold })));

            // Aligner les totaux à droite
            foreach (ListItem item in list.ListItems)
            {
                foreach (Paragraph para in item.Blocks)
                {
                    para.TextAlignment = TextAlignment.Right;
                }
            }

            flowDocument.Blocks.Add(list);

            return flowDocument;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
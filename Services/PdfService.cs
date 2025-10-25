using GenerateurFactures.Models;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.IO;
using System.Linq;

namespace GenerateurFactures.Services
{
    public class PdfService
    {
        public void GenererFacturePdf(Facture facture, string cheminFichier)
        {
            var document = new Document(PageSize.A4, 50, 50, 50, 50);

            try
            {
                var writer = PdfWriter.GetInstance(document, new FileStream(cheminFichier, FileMode.Create));
                document.Open();

                // Fonts
                var fontTitre = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 18, BaseColor.BLACK);
                var fontSousTitre = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 14, BaseColor.BLACK);
                var fontNormal = FontFactory.GetFont(FontFactory.HELVETICA, 12, BaseColor.BLACK);
                var fontPetit = FontFactory.GetFont(FontFactory.HELVETICA, 10, BaseColor.GRAY);

                // En-tête
                var tableEntete = new PdfPTable(2) { WidthPercentage = 100 };
                tableEntete.SetWidths(new float[] { 1, 1 });

                // Informations entreprise
                var cellEntreprise = new PdfPCell();
                cellEntreprise.Border = Rectangle.NO_BORDER;
                cellEntreprise.AddElement(new Paragraph(facture.Entreprise.Nom, fontSousTitre));
                cellEntreprise.AddElement(new Paragraph(facture.Entreprise.Adresse, fontNormal));
                cellEntreprise.AddElement(new Paragraph($"{facture.Entreprise.CodePostal} {facture.Entreprise.Ville}", fontNormal));
                if (!string.IsNullOrWhiteSpace(facture.Entreprise.Siret))
                    cellEntreprise.AddElement(new Paragraph($"SIRET: {facture.Entreprise.Siret}", fontPetit));
                if (!string.IsNullOrWhiteSpace(facture.Entreprise.Email))
                    cellEntreprise.AddElement(new Paragraph($"Email: {facture.Entreprise.Email}", fontPetit));
                if (!string.IsNullOrWhiteSpace(facture.Entreprise.Telephone))
                    cellEntreprise.AddElement(new Paragraph($"Tél: {facture.Entreprise.Telephone}", fontPetit));

                // Informations facture
                var cellFacture = new PdfPCell();
                cellFacture.Border = Rectangle.NO_BORDER;
                cellFacture.HorizontalAlignment = Element.ALIGN_RIGHT;
                cellFacture.AddElement(new Paragraph("FACTURE", fontTitre));
                cellFacture.AddElement(new Paragraph($"N° {facture.Numero}", fontSousTitre));
                cellFacture.AddElement(new Paragraph($"Date: {facture.Date:dd/MM/yyyy}", fontNormal));

                tableEntete.AddCell(cellEntreprise);
                tableEntete.AddCell(cellFacture);
                document.Add(tableEntete);

                document.Add(new Paragraph(" "));

                // Informations client
                var paragrapheClient = new Paragraph("Facturé à:", fontSousTitre);
                document.Add(paragrapheClient);

                var paragrapheClientInfo = new Paragraph();
                paragrapheClientInfo.Add(new Chunk(facture.Client.Nom, fontNormal));
                paragrapheClientInfo.Add(Chunk.NEWLINE);
                paragrapheClientInfo.Add(new Chunk(facture.Client.Adresse, fontNormal));
                paragrapheClientInfo.Add(Chunk.NEWLINE);
                paragrapheClientInfo.Add(new Chunk($"{facture.Client.CodePostal} {facture.Client.Ville}", fontNormal));
                if (!string.IsNullOrWhiteSpace(facture.Client.Email))
                {
                    paragrapheClientInfo.Add(Chunk.NEWLINE);
                    paragrapheClientInfo.Add(new Chunk($"Email: {facture.Client.Email}", fontNormal));
                }
                document.Add(paragrapheClientInfo);

                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));

                // Tableau des produits
                var tableProduits = new PdfPTable(4) { WidthPercentage = 100 };
                tableProduits.SetWidths(new float[] { 3, 1, 1, 1 });

                // En-têtes
                var cellHeaderProduit = new PdfPCell(new Phrase("Produit", fontSousTitre))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_LEFT
                };
                var cellHeaderPrix = new PdfPCell(new Phrase("Prix unitaire", fontSousTitre))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };
                var cellHeaderQuantite = new PdfPCell(new Phrase("Quantité", fontSousTitre))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_CENTER
                };
                var cellHeaderTotal = new PdfPCell(new Phrase("Total", fontSousTitre))
                {
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                };

                tableProduits.AddCell(cellHeaderProduit);
                tableProduits.AddCell(cellHeaderPrix);
                tableProduits.AddCell(cellHeaderQuantite);
                tableProduits.AddCell(cellHeaderTotal);

                // Lignes de produits
                foreach (var produit in facture.Produits)
                {
                    tableProduits.AddCell(new PdfPCell(new Phrase(produit.Nom, fontNormal)) { Padding = 8 });
                    tableProduits.AddCell(new PdfPCell(new Phrase($"{produit.Prix:C}", fontNormal))
                    {
                        Padding = 8,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    });
                    tableProduits.AddCell(new PdfPCell(new Phrase(produit.Quantite.ToString(), fontNormal))
                    {
                        Padding = 8,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    });
                    tableProduits.AddCell(new PdfPCell(new Phrase($"{produit.Total:C}", fontNormal))
                    {
                        Padding = 8,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    });
                }

                document.Add(tableProduits);

                document.Add(new Paragraph(" "));

                // Tableau des totaux
                var tableTotaux = new PdfPTable(2) { WidthPercentage = 50, HorizontalAlignment = Element.ALIGN_RIGHT };
                tableTotaux.SetWidths(new float[] { 1, 1 });

                // Total HT
                tableTotaux.AddCell(new PdfPCell(new Phrase("Total HT:", fontNormal))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                tableTotaux.AddCell(new PdfPCell(new Phrase($"{facture.TotalHT:C}", fontNormal))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                });

                // Réduction si applicable
                if (facture.Reduction > 0)
                {
                    tableTotaux.AddCell(new PdfPCell(new Phrase("Réduction:", fontNormal))
                    {
                        Border = Rectangle.NO_BORDER,
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    });
                    tableTotaux.AddCell(new PdfPCell(new Phrase($"-{facture.Reduction:C}", fontNormal))
                    {
                        Border = Rectangle.NO_BORDER,
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    });

                    tableTotaux.AddCell(new PdfPCell(new Phrase("Sous-total:", fontNormal))
                    {
                        Border = Rectangle.NO_BORDER,
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_LEFT
                    });
                    tableTotaux.AddCell(new PdfPCell(new Phrase($"{facture.SousTotal:C}", fontNormal))
                    {
                        Border = Rectangle.NO_BORDER,
                        Padding = 5,
                        HorizontalAlignment = Element.ALIGN_RIGHT
                    });
                }

                // TVA
                tableTotaux.AddCell(new PdfPCell(new Phrase($"TVA ({facture.TauxTva:N1}%):", fontNormal))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                tableTotaux.AddCell(new PdfPCell(new Phrase($"{facture.MontantTva:C}", fontNormal))
                {
                    Border = Rectangle.NO_BORDER,
                    Padding = 5,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                });

                // Total TTC
                tableTotaux.AddCell(new PdfPCell(new Phrase("Total TTC:", fontSousTitre))
                {
                    BorderWidth = 1,
                    BorderColor = BaseColor.BLACK,
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_LEFT
                });
                tableTotaux.AddCell(new PdfPCell(new Phrase($"{facture.TotalTTC:C}", fontSousTitre))
                {
                    BorderWidth = 1,
                    BorderColor = BaseColor.BLACK,
                    BackgroundColor = BaseColor.LIGHT_GRAY,
                    Padding = 8,
                    HorizontalAlignment = Element.ALIGN_RIGHT
                });

                document.Add(tableTotaux);

                // Pied de page
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph(" "));
                document.Add(new Paragraph("Merci pour votre confiance!", fontPetit));
            }
            finally
            {
                document.Close();
            }
        }
    }
}
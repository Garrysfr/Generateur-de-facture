namespace GenerateurFactures.Models
{
    public class Parametres
    {
        public Entreprise? Entreprise { get; set; } = new Entreprise();
        public decimal TauxTvaParDefaut { get; set; } = 20.0m;
    }
}
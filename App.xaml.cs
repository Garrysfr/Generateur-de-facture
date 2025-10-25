using System.Globalization;
using System.Threading;
using System.Windows;

namespace GenerateurFactures
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Configurer la culture française pour l'affichage en euros
            var culture = new CultureInfo("fr-FR");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            base.OnStartup(e);
        }
    }
}
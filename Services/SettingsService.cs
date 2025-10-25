using GenerateurFactures.Models;
using Newtonsoft.Json;
using System;
using System.IO;

namespace GenerateurFactures.Services
{
    public class SettingsService
    {
        private readonly string _settingsPath;

        public SettingsService()
        {
            var appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataPath, "GenerateurFactures");
            Directory.CreateDirectory(appFolder);
            _settingsPath = Path.Combine(appFolder, "parametres.json");
        }

        public Parametres? ChargerParametres()
        {
            try
            {
                if (File.Exists(_settingsPath))
                {
                    var json = File.ReadAllText(_settingsPath);
                    return JsonConvert.DeserializeObject<Parametres>(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erreur lors du chargement des paramètres : {ex.Message}");
            }

            return new Parametres();
        }

        public void SauvegarderParametres(Parametres parametres)
        {
            try
            {
                var json = JsonConvert.SerializeObject(parametres, Formatting.Indented);
                File.WriteAllText(_settingsPath, json);
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la sauvegarde des paramètres : {ex.Message}");
            }
        }
    }
}
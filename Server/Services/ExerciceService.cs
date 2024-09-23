using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using STIMULUS_V2.Server.Data;
using STIMULUS_V2.Shared.Interface.ChildInterface;
using STIMULUS_V2.Shared.Models.DTOs;
using STIMULUS_V2.Shared.Models.Entities;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace STIMULUS_V2.Server.Services
{
    public class ExerciceService : IExerciceService
    {
        private readonly STIMULUSContext sTIMULUSContext;
        public string sortie;
        public ExerciceService(STIMULUSContext sTIMULUSContext)
        {
            this.sTIMULUSContext = sTIMULUSContext;
        }

        // Ajout d'un nouvel exercice dans la bd
        public async Task<APIResponse<Exercice>> Create(Exercice item)
        {
            try
            {
                sTIMULUSContext.Exercice.Add(item);
                await sTIMULUSContext.SaveChangesAsync();

                if (sTIMULUSContext.Exercice.Contains(item))
                {
                    return new APIResponse<Exercice>(item, 200, "Succès");
                }
                else
                {
                    return new APIResponse<Exercice>(null, 500, "Erreur interne du serveur");
                }
            }
            catch (Exception ex)
            {
                return new APIResponse<Exercice>(null, 500, $"Erreur lors de la création du modèle : {typeof(Exercice).Name}. Message : {ex.Message}");
            }
        }

        // Implémentation des méthodes manquantes de IModelService<Exercice, int>

        public async Task<APIResponse<Exercice>> Get(int id)
        {
            var exercice = await sTIMULUSContext.Exercice.FindAsync(id);
            return exercice != null
                ? new APIResponse<Exercice>(exercice, 200, "Succès")
                : new APIResponse<Exercice>(null, 404, "Exercice non trouvé");
        }

        public async Task<APIResponse<IEnumerable<Exercice>>> GetAll()
        {
            var exercices = await sTIMULUSContext.Exercice.ToListAsync();
            return new APIResponse<IEnumerable<Exercice>>(exercices, 200, "Succès");
        }

        public async Task<APIResponse<IEnumerable<Exercice>>> GetAllById(int id)
        {
            var exercices = await sTIMULUSContext.Exercice.Where(e => e.ExerciceId == id).ToListAsync();
            return new APIResponse<IEnumerable<Exercice>>(exercices, 200, "Succès");
        }

        public async Task<APIResponse<Exercice>> Update(int id, Exercice updatedItem)
        {
            var exercice = await sTIMULUSContext.Exercice.FindAsync(id);
            if (exercice != null)
            {
                sTIMULUSContext.Entry(exercice).CurrentValues.SetValues(updatedItem);
                await sTIMULUSContext.SaveChangesAsync();
                return new APIResponse<Exercice>(exercice, 200, "Mise à jour réussie");
            }
            return new APIResponse<Exercice>(null, 404, "Exercice non trouvé");
        }

        public async Task<APIResponse<bool>> Delete(int id)
        {
            var exercice = await sTIMULUSContext.Exercice.FindAsync(id);
            if (exercice != null)
            {
                sTIMULUSContext.Exercice.Remove(exercice);
                await sTIMULUSContext.SaveChangesAsync();
                return new APIResponse<bool>(true, 200, "Suppression réussie");
            }
            return new APIResponse<bool>(false, 404, "Exercice non trouvé");
        }
        // Exécution du code C# en local et récupère la sortie
        //public async Task<APIResponse<string>> ExecuteCode(string codeUtilisateur)
        //{
        //    try
        //    {
        //        int cptReadLine = 0;
        //        string[] donneesUtilisateur = null;

        //        // Vérification de la nécessité d'appels à Console.ReadLine()
        //        var regex = new Regex(@"Console\.ReadLine\s*\(\s*\)\s*;");
        //        var matches = regex.Matches(codeUtilisateur);
        //        cptReadLine = matches.Count;

        //        if (cptReadLine > 0)
        //        {
        //            donneesUtilisateur = new string[cptReadLine];
        //            for (int i = 0; i < cptReadLine; i++)
        //            {
        //                // Ici, on simule la récupération d'entrées utilisateur
        //                donneesUtilisateur[i] = $"Entrée simulation {i + 1}";
        //            }
        //        }

        //        // Création des fichiers temporaires pour le projet et compilation
        //        string cheminFichier = Path.Combine(Path.GetTempPath(), "MonProjet");
        //        string nomFichier = Path.Combine(cheminFichier, "Program.cs");
        //        string projetFichier = Path.Combine(cheminFichier, "MonProjet.csproj");
        //        string outputDir = Path.Combine(cheminFichier, "bin", "Debug", "net8.0");
        //        string exePath = Path.Combine(outputDir, "MonProjet.exe");

        //        Directory.CreateDirectory(cheminFichier);
        //        await File.WriteAllTextAsync(nomFichier, GenerateProgramFile(codeUtilisateur));

        //        string projetContenu = @"
        //<Project Sdk=""Microsoft.NET.Sdk"">
        //<PropertyGroup>
        //    <OutputType>Exe</OutputType>
        //    <TargetFramework>net8.0</TargetFramework>
        //    <ImplicitUsings>enable</ImplicitUsings>
        //    <Nullable>enable</Nullable>
        //</PropertyGroup>
        //</Project>";

        //        await File.WriteAllTextAsync(projetFichier, projetContenu);

        //        // Compilation locale
        //        var startInfo = new ProcessStartInfo
        //        {
        //            FileName = "dotnet",
        //            Arguments = "build",
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true,
        //            WorkingDirectory = cheminFichier,
        //        };

        //        using var processus = new Process { StartInfo = startInfo };
        //        processus.Start();

        //        string sortieCompilation = await processus.StandardOutput.ReadToEndAsync();
        //        string errorCompilation = await processus.StandardError.ReadToEndAsync();
        //        processus.WaitForExit();

        //        if (processus.ExitCode != 0)
        //        {
        //            // Nettoyage en cas d'erreur de compilation
        //            CleanUp(cheminFichier);
        //            return new APIResponse<string>(sortieCompilation + errorCompilation, 500, "Erreur lors de la compilation");
        //        }

        //        // Exécution du programme après compilation
        //        var processInfo = new ProcessStartInfo
        //        {
        //            FileName = exePath,
        //            RedirectStandardOutput = true,
        //            RedirectStandardError = true,
        //            RedirectStandardInput = true,
        //            UseShellExecute = false,
        //            CreateNoWindow = true
        //        };

        //        using var process = new Process { StartInfo = processInfo };
        //        process.Start();

        //        if (cptReadLine > 0)
        //        {
        //            using StreamWriter sw = process.StandardInput;
        //            foreach (var donnee in donneesUtilisateur)
        //            {
        //                if (sw.BaseStream.CanWrite)
        //                {
        //                    await sw.WriteLineAsync(donnee);
        //                }
        //            }
        //        }

        //        string sortieExecution = await process.StandardOutput.ReadToEndAsync();
        //        string errorExecution = await process.StandardError.ReadToEndAsync();
        //        process.WaitForExit();

        //        // Nettoyage après exécution
        //        CleanUp(cheminFichier);

        //        return new APIResponse<string>(string.IsNullOrEmpty(sortieExecution) ? errorExecution : sortieExecution, 200, "Succès");
        //    }
        //    catch (Exception ex)
        //    {
        //        return new APIResponse<string>(null, 500, $"Erreur lors de l'exécution du code. Message : {ex.Message}");
        //    }
        //}

        private void CleanUp(string cheminFichier)
        {
            try
            {
                if (Directory.Exists(cheminFichier))
                {
                    Directory.Delete(cheminFichier, true);
                }
            }
            catch (Exception ex)
            {
                // Log error or handle cleanup errors
                Console.WriteLine($"Erreur lors du nettoyage des fichiers temporaires : {ex.Message}");
            }
        }

        private string GenerateProgramFile(string codeUtilisateur)
        {
            string modele = @"
            using System;
            namespace MyApplication
            {
                class Program
                {
                    static void Main(string[] args)
                    {
                        {0}
                    }
                }
            }";

            string code = modele.Replace("{0}", codeUtilisateur.Replace("\r\n", "\n").Replace("\n", "\n    "));

            return code;
        }

        // Autres méthodes du service (Create, Delete, etc.)...
    }
}

using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;
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
using System.Text;

namespace STIMULUS_V2.Server.Services
{
    public class ExerciceService : IExerciceService
    {
        private readonly STIMULUSContext sTIMULUSContext;
        public string sortie;

        private string[] donneesUtilisateur;
        private int cptReadLine = 0;
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

        private void VerifierBesoinDonnees(string codeUtilisateur)
        {
            var regex = new Regex(@"Console\.ReadLine\s*\(\s*\)\s*;");
            var matches = regex.Matches(codeUtilisateur);
            cptReadLine = matches.Count;
        }

        /// <summary>
        /// Envoie le code à dotnet et compile le code
        /// </summary>
        /// <returns></returns>
        public async Task<APIResponse<string>> ExecuteCode(string codeUtilisateur)
        {
            VerifierBesoinDonnees(codeUtilisateur);

            if (cptReadLine > 0)
            {
                donneesUtilisateur = new string[cptReadLine];
                for (int i = 0; i < cptReadLine; i++)
                {
                    donneesUtilisateur[i] = await PromptUser($"Entrez la valeur pour Console.ReadLine() #{i + 1}:");
                }
            }

            string cheminFichier = Path.Combine(Path.GetTempPath(), "MonProjet");
            string nomFichier = Path.Combine(cheminFichier, "Program.cs");
            string projetFichier = Path.Combine(cheminFichier, "MonProjet.csproj");
            string outputDir = Path.Combine(cheminFichier, "bin", "Debug", "net8.0");
            string exePath = Path.Combine(outputDir, "MonProjet.exe");

            // Crée le dossier de la solution
            Directory.CreateDirectory(cheminFichier);
            await File.WriteAllTextAsync(nomFichier, GenerateProgramFile(codeUtilisateur));

            string projetContenu = @"
            <Project Sdk=""Microsoft.NET.Sdk"">
              <PropertyGroup>
                <OutputType>Exe</OutputType>
                <TargetFramework>net8.0</TargetFramework>
                <ImplicitUsings>enable</ImplicitUsings>
                <Nullable>enable</Nullable>
              </PropertyGroup>
            </Project>";

            await File.WriteAllTextAsync(projetFichier, projetContenu);

            // Vérifie et supprime l'exécutable existant
            if (File.Exists(exePath))
            {
                try
                {
                    File.Delete(exePath);
                }
                catch (IOException ex)
                {
                    return new APIResponse<string>
                    {
                        Data = null,
                        Message = $"Erreur : Impossible de supprimer l'exécutable existant. {ex.Message}"
                    };
                }
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "build",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WorkingDirectory = cheminFichier,
            };

            using var processus = new Process { StartInfo = startInfo };
            processus.Start();

            string sortieCompilation = await processus.StandardOutput.ReadToEndAsync();
            string errorCompilation = await processus.StandardError.ReadToEndAsync();
            processus.WaitForExit();

            if (processus.ExitCode == 0)
            {
                return await ExecuteGeneratedCode(exePath);
            }
            else
            {
                return new APIResponse<string>
                {
                    Data = null,
                    Message = string.IsNullOrEmpty(sortieCompilation) ? errorCompilation : sortieCompilation
                };
            }
        }

        private async Task<APIResponse<string>> ExecuteGeneratedCode(string exePath)
        {
            var processInfo = new ProcessStartInfo
            {
                FileName = exePath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                RedirectStandardInput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = processInfo };
            process.Start();

            if (cptReadLine > 0)
            {
                using StreamWriter sw = process.StandardInput;
                foreach (var input in donneesUtilisateur)
                {
                    await sw.WriteLineAsync(input);
                }
            }

            string sortieExecution = await process.StandardOutput.ReadToEndAsync();
            string errorExecution = await process.StandardError.ReadToEndAsync();
            process.WaitForExit();

            return new APIResponse<string>
            {
                Data = string.IsNullOrEmpty(sortieExecution) ? errorExecution : sortieExecution,
                Message = null
            };
        }

        private string GenerateProgramFile(string codeUtilisateur)
        {
            string modele = @"{0}";

            return modele.Replace("{0}", codeUtilisateur.Replace("\r\n", "\n").Replace("\n", "\n    "));
        }

        private async Task<string> PromptUser(string message)
        {
            // Placeholder pour l'appel JavaScript. Remplacer par une logique de prompt appropriée.
            return await Task.FromResult("Valeur de l'utilisateur");
        }

    }
}
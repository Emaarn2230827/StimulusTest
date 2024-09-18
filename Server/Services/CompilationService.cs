using System.Diagnostics;

using STIMULUS_V2.Shared.Interface.ChildInterface;


namespace STIMULUS_V2.Server.Services
{ 
    public class CompilationService 
    {
        public static async Task<string> CompileCSharpAsync(string code)
        {
            string tempPath = Path.GetTempPath();
            string fileName = Path.Combine(tempPath, "Program.cs");

            // Créer un fichier temporaire pour la compilation
            await File.WriteAllTextAsync(fileName, code);

            // Commande pour lancer la compilation
            ProcessStartInfo psi = new ProcessStartInfo("dotnet", "build")
            {
                WorkingDirectory = tempPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = Process.Start(psi);
            await process.WaitForExitAsync();

            // Lire le résultat de la compilation
            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            return process.ExitCode == 0 ? output : error;
        }
    }

}

using STIMULUS_V2.Shared.Interface.ChildInterface;
using STIMULUS_V2.Shared.Models.DTOs;
using STIMULUS_V2.Shared.Models.Entities;
using System.Net.Http.Json;
using Serilog;

namespace STIMULUS_V2.Client.Services
{
    public class ExerciceService : IExerciceService
    {
        private readonly HttpClient _httpClient;

        public ExerciceService(HttpClient httpClient)
        {
            _httpClient = httpClient;


        }

        public async Task<APIResponse<Exercice>> Create(Exercice item)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("api/Exercice/Create", item);
                var log = Log.ForContext<ExerciceService>();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<Exercice>>();
                    log.Information($"Create(Exercice item = {item}) ApiResponse: {apiResponse}");
                    return apiResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    log.Error($"Create(Exercice item = {item}) failed. Status Code: {response.StatusCode}, Error: {errorContent}");
                    return new APIResponse<Exercice>(default, (int)response.StatusCode, $"Erreur lors de la création : {errorContent}");
                }
            }
            catch (Exception ex)
            {
                var log = Log.ForContext<ExerciceService>();
                log.Error($"Create(Exercice item = {item}) Exception: {ex.Message}");
                return new APIResponse<Exercice>(default, 500, $"Erreur lors de la création : {ex.Message}");
            }
        }

        public async Task<APIResponse<string>> ExecuteCode(string json, int cptReadLine, string[] dataReadLine)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync<string>($"api/Exercice/Execute", json);
                var log = Log.ForContext<ExerciceService>();

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<APIResponse<string>>();
                    log.Information($"ExecuteCode(string json = {json}) ApiResponse: {apiResponse}");
                    return apiResponse;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    log.Error($"ExecuteCode(string json = {json}) failed. Status Code: {response.StatusCode}, Error: {errorContent}");
                    return new APIResponse<string>(null, (int)response.StatusCode, $"Erreur lors de l'exécution : {errorContent}");
                }
            }
            catch (Exception ex)
            {
                var log = Log.ForContext<ExerciceService>();
                log.Error($"ExecuteCode(string json = {json}) Exception: {ex.Message}");
                return new APIResponse<string>(null, 500, $"Erreur lors de l'exécution : {ex.Message}");
            }
        }


        public async Task<APIResponse<bool>> Delete(int id)
        {
            var result = await _httpClient.DeleteAsync($"api/Exercice/Delete/{id}");
            var log = Log.ForContext<ExerciceService>();
            var apiResponse = await result.Content.ReadFromJsonAsync<APIResponse<bool>>();
            log.Information($"Delete(int id = {id}) ApiResponse: {apiResponse}");
            return apiResponse;
        }

        public async Task<APIResponse<Exercice>> Get(int id)
        {
            var result = await _httpClient.GetFromJsonAsync<APIResponse<Exercice>>($"api/Exercice/Fetch/{id}");
            var log = Log.ForContext<ExerciceService>();
            log.Information($"Get(int id = {id}) ApiResponse: {result}");
            return result;
        }

        public async Task<APIResponse<IEnumerable<Exercice>>> GetAll()
        {
            var result = await _httpClient.GetFromJsonAsync<APIResponse<IEnumerable<Exercice>>>("api/Exercice/Fetch/All");
            var log = Log.ForContext<ExerciceService>();
            log.Information($"GetAll() ApiResponse: {result}");
            return result;
        }

        public Task<APIResponse<IEnumerable<Exercice>>> GetAllById(int id)
        {
            var log = Log.ForContext<ExerciceService>();
            var apiResponse = new NotImplementedException();
            log.Information($"GetAllById(int id = {id}) ApiResponse: {apiResponse}");
            throw apiResponse;
        }

        public async Task<APIResponse<Exercice>> Update(int id, Exercice item)
        {
            var result = await _httpClient.PutAsJsonAsync($"api/Exercice/Update/{id}", item);
            var log = Log.ForContext<ExerciceService>();
            var apiResponse = await result.Content.ReadFromJsonAsync<APIResponse<Exercice>>();
            log.Information($"Update(int id = {id}, Exercice item = {item}) ApiResponse: {apiResponse}");
            return apiResponse;
        }
    }
}

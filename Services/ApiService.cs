using System.Net.Http.Json;

namespace AppParidade.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://servicodados.ibge.gov.br/api/v2/censos/")
            };
        }

        public async Task<List<string>> GetPopularNamesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<List<NomeResponse>>("nomes");
                if (response != null)
                    return response.Select(r => r.nome).Take(10).ToList();

                return new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro na API: {ex.Message}");
                return new List<string>();
            }
        }
    }

    public class NomeResponse
    {
        public string nome { get; set; }
    }
}
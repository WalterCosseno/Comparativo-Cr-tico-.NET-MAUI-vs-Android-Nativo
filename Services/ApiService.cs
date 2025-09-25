using System.Text.Json;

namespace AppParidade.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://servicodados.ibge.gov.br/api/v2/censos/nomes";

        public ApiService()
        {
            _httpClient = new HttpClient();
        }

        public async Task<List<string>> GetPopularNamesAsync()
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/ranking");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var nomesData = JsonSerializer.Deserialize<List<NomeRanking>>(content);

                    return nomesData?.SelectMany(n => n.res)
                                     .OrderByDescending(r => r.frequencia)
                                     .Select(r => r.nome)
                                     .Take(20)
                                     .ToList() ?? new List<string>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar nomes: {ex.Message}");
            }

            return new List<string>();
        }

        public async Task<List<NomeFrequencia>> GetNameDetailsAsync(string nome)
        {
            try
            {
                var response = await _httpClient.GetAsync($"{BaseUrl}/{Uri.EscapeDataString(nome)}");
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var nomeData = JsonSerializer.Deserialize<List<NomeData>>(content);

                    return nomeData?.FirstOrDefault()?.res ?? new List<NomeFrequencia>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao buscar detalhes do nome: {ex.Message}");
            }

            return new List<NomeFrequencia>();
        }
    }

    // Classes para desserialização JSON
    public class NomeRanking
    {
        public List<RankingItem> res { get; set; }
    }

    public class RankingItem
    {
        public string nome { get; set; }
        public int frequencia { get; set; }
        public int ranking { get; set; }
    }

    public class NomeData
    {
        public string nome { get; set; }
        public string sexo { get; set; }
        public string localidade { get; set; }
        public List<NomeFrequencia> res { get; set; }
    }

    public class NomeFrequencia
    {
        public string periodo { get; set; }
        public int frequencia { get; set; }
    }
}

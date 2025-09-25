using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using AppParidade.Models;
using AppParidade.Services;
using Microsoft.Maui.Devices.Sensors;

namespace AppParidade.ViewModels
{
    public class ItemsViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<Item> Items { get; set; } = new();
        private ObservableCollection<Item> _filteredItems;
        public ObservableCollection<Item> FilteredItems
        {
            get => _filteredItems;
            set { _filteredItems = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> NomesIBGE { get; set; } = new();
        private readonly ApiService _apiService = new();

        private string _locationText;
        public string LocationText { get => _locationText; set { _locationText = value; OnPropertyChanged(); } }

        private string _searchQuery;
        public string SearchQuery { get => _searchQuery; set { _searchQuery = value; OnPropertyChanged(); ApplyFilter(); } }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public Command BuscarNomesCommand { get; }
        public Command GetLocationCommand { get; }
        public Command CarregarNomesPopularesCommand { get; }

        public ItemsViewModel()
        {
            FilteredItems = new ObservableCollection<Item>(Items);

            BuscarNomesCommand = new Command(async () => await BuscarNomes());
            GetLocationCommand = new Command(async () => await GetLocation());
            CarregarNomesPopularesCommand = new Command(async () => await CarregarNomesPopulares());

            // Carrega automaticamente os nomes populares ao iniciar
            Task.Run(async () => await CarregarNomesPopulares());
        }

        private async Task CarregarNomesPopulares()
        {
            IsLoading = true;

            try
            {
                Items.Clear();
                var nomesPopulares = await _apiService.GetPopularNamesAsync();

                foreach (var nome in nomesPopulares)
                {
                    // Busca detalhes do nome para obter informações mais completas
                    var detalhes = await _apiService.GetNameDetailsAsync(nome);
                    var frequenciaTotal = detalhes.Sum(d => d.frequencia);

                    var item = new Item
                    {
                        Name = nome,
                        Description = $"Frequência total: {frequenciaTotal:N0}\n" +
                                     $"Períodos disponíveis: {detalhes.Count}"
                    };

                    Items.Add(item);
                }

                FilteredItems = new ObservableCollection<Item>(Items);
            }
            catch (Exception ex)
            {
                // Adiciona alguns itens de fallback em caso de erro
                Items.Add(new Item { Name = "Erro ao carregar", Description = ex.Message });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
            {
                FilteredItems = new ObservableCollection<Item>(Items);
            }
            else
            {
                FilteredItems = new ObservableCollection<Item>(
                    Items.Where(i => i.Name.ToLower().Contains(SearchQuery.ToLower())));
            }
        }

        private async Task BuscarNomes()
        {
            NomesIBGE.Clear();
            var nomes = await _apiService.GetPopularNamesAsync();
            foreach (var nome in nomes)
                NomesIBGE.Add(nome);
        }

        private async Task GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var loc = await Geolocation.GetLocationAsync(request);
                LocationText = loc != null ? $"Lat: {loc.Latitude}, Lon: {loc.Longitude}" : "Localização não disponível";
            }
            catch (Exception ex)
            {
                LocationText = $"Erro: {ex.Message}";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}

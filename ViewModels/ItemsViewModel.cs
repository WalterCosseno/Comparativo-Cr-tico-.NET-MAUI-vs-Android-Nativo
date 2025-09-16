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
        public ObservableCollection<Item> Items { get; set; }
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

        public Command BuscarNomesCommand { get; }
        public Command GetLocationCommand { get; }

        public ItemsViewModel()
        {
            Items = new ObservableCollection<Item>
{
                new Item { Name = " Ana Clara ", Description = "Idade:25 \nLocalização: Rio de Janeiro" },
                new Item { Name = " Bruno Henrique ", Description = "Idade:32 \nLocalização: Rio de Janeiro " },
                new Item { Name = " Camila Souza ", Description = "Idade:28  \nLocalização: São Luís " },
                new Item { Name = " Daniel Oliveira ", Description = "Idade:40  \nLocalização: Belém " },
                new Item { Name = " Elisa Martins ", Description = "Idade:22  \nLocalização: Florianópolis " },
                new Item { Name = " Felipe Santos ", Description = "Idade:35  \nLocalização: Vitória " },
                new Item { Name = " Gabriela Lima ", Description = "Idade:27  \nLocalização: Goiânia " },
                new Item { Name = " Henrique Costa ", Description = "Idade:30  \nLocalização: Manaus " },
                new Item { Name = " Isabela Rocha ", Description = "Idade:26  \nLocalização: Campinas " },
                new Item { Name = " João Pedro ", Description = "Idade:31  \nLocalização: Brasília " },
                new Item { Name = " Karina Alves ", Description = "Idade:24  \nLocalização: Recife " },
                new Item { Name = " Leonardo Pereira ", Description = "Idade:38  \nLocalização: Fortaleza " },
                new Item { Name = " Mariana Fernandes ", Description = "Idade:29  \nLocalização: Porto Alegre " },
                new Item { Name = " Nicolas Ribeiro ", Description = "Idade:33  \nLocalização: Curitiba " },
                new Item { Name = " Olivia Carvalho ", Description = "Idade:23  \nLocalização: Salvador " },
                new Item { Name = " Paulo Victor ", Description = "Idade:36  \nLocalização: Belo Horizonte " },
                new Item { Name = " Quésia Duarte ", Description = "Idade:21  \nLocalização: São Paulo " },
                new Item { Name = " Rafael Gomes ", Description = "Idade:34  \nLocalização: Natal " },
                new Item { Name = " Sabrina Teixeira ", Description = "Idade:28  \nLocalização: Porto Velho " },
                new Item { Name = " Thiago Andrade ", Description = "Idade: 37  \nLocalização: João Pessoa " }
};
            FilteredItems = new ObservableCollection<Item>(Items);

            BuscarNomesCommand = new Command(async () => await BuscarNomes());
            GetLocationCommand = new Command(async () => await GetLocation());
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchQuery))
                FilteredItems = new ObservableCollection<Item>(Items);
            else
                FilteredItems = new ObservableCollection<Item>(
                    Items.Where(i => i.Name.ToLower().Contains(SearchQuery.ToLower())));
        }

        private async Task BuscarNomes()
        {
            NomesIBGE.Clear();
            var nomes = await _apiService.GetPopularNamesAsync();
            foreach(var nome in nomes)
                NomesIBGE.Add(nome);
        }

        private async Task GetLocation()
        {
            try
            {
                var loc = await Geolocation.GetLocationAsync();
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
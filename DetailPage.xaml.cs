using AppParidade.Models;

namespace AppParidade
{
    public partial class DetailPage : ContentPage
    {
        public DetailPage(Item item)
        {
            InitializeComponent();
            BindingContext = item;
        }

        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
    }
}
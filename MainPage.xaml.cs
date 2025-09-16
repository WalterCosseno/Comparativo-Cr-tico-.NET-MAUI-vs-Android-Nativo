using AppParidade.Models;
using AppParidade.ViewModels;

namespace AppParidade
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection?.FirstOrDefault() is Item selected)
            {
                await Navigation.PushAsync(new DetailPage(selected));
            }
        }
    }
}
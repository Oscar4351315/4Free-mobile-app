using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using IAB330.ViewModels;

namespace IAB330.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {
        public MapPage()
        {
            InitializeComponent(); 
            BindingContext = new MapViewModel();
        }
    }
}
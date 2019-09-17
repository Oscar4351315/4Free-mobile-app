using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using IAB330.ViewModel;

namespace IAB330.View
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using Xamarin.Essentials;


namespace IAB330.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
        }




        private async void ButtonOpenCoords_Clicked(object sender, EventArgs e)
        {

            var request = new GeolocationRequest(GeolocationAccuracy.Medium);
            var locationget = await Geolocation.GetLocationAsync(request);

            var location = new Location(locationget.Latitude, locationget.Longitude);

            Map.OpenAsync(location);

            

            //Console.WriteLine($"Latitude: {location.Latitude}, Longitude: {location.Longitude}, Altitude: {location.Altitude}");

            //LatitudeLabel.Text = $"{location.Latitude}";
            //LongitudeLabel.Text = $"{location.Longitude}";



            //await Map.OpenAsync(location.Latitude, location.Longitude);
        }
    }
}
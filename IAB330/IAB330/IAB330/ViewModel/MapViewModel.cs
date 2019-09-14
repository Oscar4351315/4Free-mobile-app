using System;
using System.Windows.Input;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using Plugin.Geolocator;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using Xamarin.Forms.PlatformConfiguration;

namespace IAB330.ViewModel
{
    public partial class MapViewModel : BaseViewModel
    {
        public MapViewModel()
        {
            Title = "Map";
            Map = new Xamarin.Forms.Maps.Map();

            PostOnButtonCommand = new Command(async () => await OpenMap(), () => !IsBusy);
            OpenMapCommand = new Command(async () => await OpenMap(), () => !IsBusy);
        }

        public Command OpenMapCommand { get; }
        public Command PostOnButtonCommand { get; }
        public Xamarin.Forms.Maps.Map Map { get; private set; }

        async Task OpenMap()
        {
            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await Application.Current.MainPage.DisplayAlert("Need location", "Gunna need that location", "Quit"); // call function to open location settings
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                }

                //status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            }

            if (status == PermissionStatus.Granted)
            {
                var location = CrossGeolocator.Current;
                if (location.IsGeolocationEnabled && location.IsGeolocationAvailable)
                {
                    // Grabs the user's lat and lng
                    var position = await location.GetPositionAsync();

                    // Move map to start on user's location
                    Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(
                        position.Latitude, position.Longitude), Distance.FromMeters(100)));
                    Map.IsShowingUser = true;
                }
            }
        }
    }
}

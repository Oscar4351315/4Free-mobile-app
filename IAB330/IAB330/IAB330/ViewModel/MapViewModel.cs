using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using CustomRenderer;

namespace IAB330.ViewModel
{
    public partial class MapViewModel : BaseViewModel
    {
        public CustomMap Map { get; set; }
        public Command GeneralCommand { get; }
        public Command PostCommand { get; }
        public Command CancelPostCommand { get; }

        public MapViewModel()
        {
            Title = "Map Page";
            bool isAllowLocation = CheckLocationPermission();

            if (isAllowLocation)
            {
                SetupMap();
                SetupPin();
                GetUserPosition();

                GeneralCommand = new Command(async () => await DoSomething(), () => !IsBusy);
                PostCommand = new Command(async () => await TooglePostWindow(), () => !IsBusy);
                CancelPostCommand = new Command(async () => await TooglePostWindow(), () => !IsBusy);
            }
            else
            {
                // Force closes app on first Visual Studio execution without location permissions
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        // Shows post window if true
        bool isPostVisible;

        public bool IsPostVisible
        {
            get { return isPostVisible; }
            set
            {
                SetProperty(ref isPostVisible, value);
            }
        }

        // Moves map to user's location
        async void GetUserPosition()
        {
            // QUT coordinates
            double lat = -27.47735;
            double lng = 153.028414;

            var location = CrossGeolocator.Current;
            location.DesiredAccuracy = 50;
            //var position = await location.GetPositionAsync(TimeSpan.FromSeconds(10));
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(
                lat, lng), Distance.FromMeters(120)));
           // position.Latitude, position.Longitude), Distance.FromMeters(120)));
        }


        // Setup and draws map
        void SetupMap()
        {
            Map = new CustomMap
            {
                MapType = MapType.Street,
                IsShowingUser = true,
                HasZoomEnabled = false
            };
        }


        // Creates a test marker
        void SetupPin()
        {
            var pin = new CustomPin
            {
                Type = PinType.Place,
                Position = new Position(-27.47735, 153.028414),
                Label = "Test Marker",
                Address = "Insert details",
                MarkerId = "01",
            };

            Map.CustomPins = new List<CustomPin> { pin };
            Map.Pins.Add(pin);
        }


        // Returns true if user allow locations for app
        bool CheckLocationPermission()
        {
            CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            var task = Task.Run(async () => await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location));
            var result = task.Result;

            if (result == PermissionStatus.Granted)
            {
                return true;
            }

            return false;
        }


        // Temporary status bar button command
        async Task DoSomething()
        {
            await Application.Current.MainPage.DisplayAlert("Doing Something", "I don't know what.", "Close");
        }

        // Temporary status bar button command
        async Task TooglePostWindow()
        {
            IsPostVisible = !isPostVisible;
        }
    }
}

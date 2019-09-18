using CustomRenderer;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace IAB330.ViewModel
{
    public partial class MapViewModel : BaseViewModel
    {
        public CustomMap Map { get; set; }
        public Command GeneralCommand { get; }
        public Command TogglePostModeCommand { get; }
        public Command ConfirmPinCommand { get; }
        public Command CancelPostCommand { get; }

        public MapViewModel()
        {
            Title = "Map Page";
            bool isAllowLocation = CheckLocationPermission();

            if (isAllowLocation)
            {
                SetupMap();
                //CreateMarker();
                GetUserPosition();
                // setup/display pins

                Map.MapClicked += OnMapClick;
                GeneralCommand = new Command(async () => await DoSomething(), () => !IsBusy);
                TogglePostModeCommand = new Command(() => TogglePostMode(), () => !IsBusy);
                ConfirmPinCommand = new Command(() => ConfirmPin(), () => !IsBusy);
                CancelPostCommand = new Command(() => CancelPost(), () => !IsBusy);
            }
            else
            {
                // Force closes app on first Visual Studio execution without location permissions
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        
        

        // Enters mode that allows user to create marker on map
        private bool isPinPlacing;
        private bool isPostConfirmed = false;
        private int markerID = 0; // ID tracker

        public bool IsPinPlacing
        {
            get { return isPinPlacing; }
            set
            {
                SetProperty(ref isPinPlacing, value);
            }
        }

        // Shows post window after marker confirmation
        public bool IsPostConfirmed
        {
            get { return isPostConfirmed; }
            set
            {
                SetProperty(ref isPostConfirmed, value);
            }
        }


      

        // Creates marker at location on map click
        void OnMapClick(object sender, MapClickedEventArgs e)
        {
            CustomPin pin = new CustomPin(); 

            if (isPinPlacing)
            {
                Map.Pins.Clear();
                pin = CreateMarker(e.Position.Latitude, e.Position.Longitude, "Marker " + markerID, "Details", markerID);
                Map.Pins.Add(pin);
            }

            if (isPostConfirmed)
            {
                markerID += 1;
                Map.CustomPins = new List<CustomPin> { pin };
                IsPinPlacing = false;
            }
        }


        // Creates a marker on map

        CustomPin CreateMarker(double lat, double lng, string title, string details, int ID)
        {
            return new CustomPin
            {
                Type = PinType.Place,
                Position = new Position(lat, lng),
                Label = title,
                Address = details,
                MarkerId = ID,
            };
        }





        // When confirmed is pressed on pin placement window
        void ConfirmPin()
        {
            if (Map.Pins.Count == 1)
            {
                IsPinPlacing = false;
                IsPostConfirmed = true;
            }
        }

        // When cancel is pressed on pin placement/ post window
        void CancelPost()
        {
            IsPinPlacing = false;
            IsPostConfirmed = false;
            Map.Pins.Clear();
        }

        // When the '+' post button is pressed
        void TogglePostMode()
        {
            IsPinPlacing = !isPinPlacing;
            Map.Pins.Clear();
        }

        // Returns true if user allowed locations for app
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

        // Setup and draws map
        void SetupMap()
        {
            Map = new CustomMap
            {
                MapType = MapType.Street,
                IsShowingUser = true,
            };
        }


        // Moves map to user's location
        async void GetUserPosition()
        {
            //var QUTposition = new Position(-27.47735, 153.028414);
            var location = CrossGeolocator.Current;
            var position = await location.GetPositionAsync(TimeSpan.FromSeconds(10));
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                //QUTposition, Distance.FromMeters(120)));
                position.Latitude, position.Longitude), Distance.FromMeters(120)));
        }


        // Generic command
        async Task DoSomething()
        {
            await Application.Current.MainPage.DisplayAlert("Doing Something", "I don't know what.", "Close");
        }

        // Post class
        public class PostInformation
        {
            string category;
            string title;
            string items;
            string description;
            string startTime;
            string endTime;
        }

        // Post's category input
        string categoryEntry;

        public string CategoryEntry
        {
            get { return categoryEntry; }
            set
            {
                SetProperty(ref categoryEntry, value);
            }
        }

        // Post's title input
        string titleEntry;
        public string TitleEntry
        {
            get { return titleEntry; }
            set
            {
                SetProperty(ref titleEntry, value);
            }
        }

        // Post's items input
        string itemsEntry;
        public string ItemsEntry
        {
            get { return itemsEntry; }
            set
            {
                SetProperty(ref itemsEntry, value);
            }
        }

        // Post's description input
        string descriptionEntry;
        public string DescriptionEntry
        {
            get { return descriptionEntry; }
            set
            {
                SetProperty(ref descriptionEntry, value);
            }
        }

        // Post's start time input
        string startTimeEntry;
        public string StartTimeEntry
        {
            get { return startTimeEntry; }
            set
            {
                SetProperty(ref startTimeEntry, value);
            }
        }

        // Post's end time input
        string endTimeEntry;
        public string EndTimeEntry
        {
            get { return endTimeEntry; }
            set
            {
                SetProperty(ref endTimeEntry, value);
            }
        }
    }
}

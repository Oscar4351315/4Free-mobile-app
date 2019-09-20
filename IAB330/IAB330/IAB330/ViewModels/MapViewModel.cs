using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IAB330.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using CustomRenderer;
using System.Diagnostics;
using System.Linq;

namespace IAB330.ViewModels
{
    public partial class MapViewModel : BaseViewModel
    {
        // Binding to view
        public CustomMap Map { get; set; }
        public Command GeneralCommand { get; }
        public Command ConfirmPinCommand { get; }
        public Command TogglePostModeCommand { get; }
        public Command CancelPinOrFormCommand { get; }
        public Command GetFormInfoCommand { get; set; }


        // Get the entry field values
        private string itemsEntry;
        private string titleEntry;
        private string endTimeEntry;
        private string categoryEntry;
        private string startTimeEntry;
        private string descriptionEntry;

        public string ItemsEntry { get { return itemsEntry; } set { SetProperty(ref itemsEntry, value); } }
        public string TitleEntry { get { return titleEntry; } set { SetProperty(ref titleEntry, value); } }
        public string EndTimeEntry { get { return endTimeEntry; } set { SetProperty(ref endTimeEntry, value); } }
        public string CategoryEntry { get { return categoryEntry;  } set { SetProperty(ref categoryEntry, value); } }
        public string StartTimeEntry { get { return startTimeEntry; } set { SetProperty(ref startTimeEntry, value); } }
        public string DescriptionEntry { get { return descriptionEntry; } set { SetProperty(ref descriptionEntry, value); } }

        //public string CategoryEntry;

        // Declaring variables:
        // Enters mode that allows user to create marker on map if true
        private bool isPinPlacing;
        private bool isPinConfirm;
        private int markerID = 0; // ID tracker
        private CustomPin TempCustomPin = new CustomPin();
        public List<CustomPin> CustomPinList = new List<CustomPin>();
        private List<PostInfo> PostInfoList = new List<PostInfo>();

        //private List<CustomPin> Pins;

        public bool IsPinPlacing
        {
            get { return isPinPlacing; }
            set { SetProperty(ref isPinPlacing, value); }
        }

        // Shows post window if true
        public bool IsPinConfirm
        {
            get { return isPinConfirm; }
            set { SetProperty(ref isPinConfirm, value); }
        }

        // Constructor that initiates and creates map
        public MapViewModel()
        {
            TitleEntry = "Map Page";
            bool isAllowLocation = CheckLocationPermission();

            if (isAllowLocation)
            {
                SetupMap();
                GetUserPosition();


                Map.MapClicked += OnMapClick;
                GeneralCommand = new Command(async () => await DoSomething(), () => !IsBusy);
                TogglePostModeCommand = new Command(() => TogglePostMode(), () => !IsBusy);
                ConfirmPinCommand = new Command(() => ConfirmPin(), () => !IsBusy);
                CancelPinOrFormCommand = new Command(() => CancelPinOrForm(), () => !IsBusy);
                GetFormInfoCommand = new Command(() => SaveFormInfo(), () => !IsBusy);
                //GetFormInfoCommand = new Command<string> (GetFormInfo);


            }
            else
            {
                // Force closes app on first Visual Studio execution without location permissions
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        // Returns true if user allowed locations for app
        bool CheckLocationPermission()
        {
            CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            var task = Task.Run(async () => await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location));
            var result = task.Result;

            if (result == PermissionStatus.Granted) return true;
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
            var QUTposition = new Position(-27.47735, 153.028414);
            //var location = CrossGeolocator.Current;
            //var position = await location.GetPositionAsync(TimeSpan.FromSeconds(10));
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(
                QUTposition, Distance.FromMeters(120)));
            // new Postition(position.Latitude, position.Longitude), Distance.FromMeters(120))));
        }


        // Creates marker at location on map click
        void OnMapClick(object sender, MapClickedEventArgs e)
        {
            //CustomPin pin = new CustomPin();

            if (isPinPlacing)
            {
                Map.Pins.Clear();
                TempCustomPin = CreateCustomPin(e.Position.Latitude, e.Position.Longitude, "Marker " + markerID, "Details", markerID);
                Map.Pins.Add(TempCustomPin);
                //TempCustomPin = pin;

            }

    // this doesn't do anything
    //if (isPinConfirm)
    //{
    //    markerID += 1;
    //    Task.Run(async () => await DoSomething());
    //    //CustomPinList.Add(pin);
    //    //Map.CustomPins = new List<CustomPin> { pin };
    //    IsPinPlacing = false;
    //}
        }


        // Creates a pin
        Pin CreatePin(double lat, double lng, string title, string details, int ID)
        {
            return new Pin
            {
                Type = PinType.Place,
                Position = new Position(lat, lng),
                Label = title,
                Address = details,
                MarkerId = ID,
            };
        }

        CustomPin CreateCustomPin(double lat, double lng, string title, string details, int ID)
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


        // When '+' button is pressed, enters or leaves pin placement window
        void TogglePostMode()
        {
            IsPinPlacing = !isPinPlacing;
            IsPinConfirm = false;
            Map.Pins.Clear();
        }


        // When confirmed is pressed on pin placement window
        void ConfirmPin()
        {
            if (Map.Pins.Count == 1)
            {
                IsPinPlacing = false;
                IsPinConfirm = true;
            }
        }


        // When cancel is pressed on pin placement/ form window
        void CancelPinOrForm()
        {
            Map.Pins.Clear();
            IsPinPlacing = false;
            IsPinConfirm = false;
        }

        // might not need this
        void CancelForm()
        {
            Map.Pins.Clear();
            IsPinPlacing = false;
            IsPinConfirm = false;
        }


        // Generic command
        async Task DoSomething()
        {
            await Application.Current.MainPage.DisplayAlert("Doing Something", "I don't know what.", "Close");
        }

        // function to display all pins on the map

        //async Task AddPinsToMap()
        void AddPinsToMap()
        {
            Application.Current.MainPage.DisplayAlert("Doing Something", "add pins to map ran", "Close");
            Map.Pins.Clear();
            foreach (CustomPin Pin in CustomPinList)
            {
                Map.Pins.Add(Pin);
            }
            //CustomPinList.ForEach((x) => Map.Pins.Add(x));
        }

        // function to get information from form
        void SaveFormInfo()
        {
            if (Map.Pins.Count != 1)
            {
                Application.Current.MainPage.DisplayAlert("Error!", "Something Failed Misserably #pinrelated", "Close");
                return;
            }

            //Pin oldPin = Map.Pins.ElementAt(0);
            //CustomPin newPin = new CustomPin();
            //newPin = TempCustomPin;


            Debug.WriteLine("post: " + categoryEntry);
            //Application.Current.MainPage.DisplayAlert("Doing Something", "hi " + categoryEntry, "Close");

            PostInfo newPost = new PostInfo(categoryEntry, titleEntry, itemsEntry, descriptionEntry, startTimeEntry, endTimeEntry);
            PostInfoList.Add(newPost);

            TempCustomPin.Label = newPost.TitleEntry;
            CustomPinList.Add(TempCustomPin);
            //Map.CustomPins.Add(TempCustomPin);
            IsPinConfirm = false;

            //Task.Run(async () => await AddPinsToMap());
            AddPinsToMap();
            //Map.CustomPins = new List<CustomPin> {TempCustomPin};

            Application.Current.MainPage.DisplayAlert("Doing Something", "number of marker in list: " + CustomPinList.Count(), "Close");


        }

    }
}

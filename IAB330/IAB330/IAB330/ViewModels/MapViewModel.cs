using System.Collections.Generic;
using System.Threading.Tasks;
using IAB330.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using CustomRenderer;

namespace IAB330.ViewModels
{
    public partial class MapViewModel : BaseViewModel
    {
        public MapViewModel()
        {
            Title = "Map Page";
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
                SaveFormInfoCommand = new Command(() => SaveFormInfo(), () => !IsBusy);

            }
            else
            {
                // Force closes app on first Visual Studio execution without location permissions
                System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
            }
        }

        // Variable declarations
        private bool isPinPlacing;
        private bool isPinConfirm;
        private CustomPin TempCustomPin = new CustomPin();
        private List<PostInfo> PostInfoList = new List<PostInfo>();
        public List<CustomPin> CustomPinList = new List<CustomPin>();

        // View bindings
        public CustomMap Map { get; set; }
        public Command GeneralCommand { get; }
        public Command ConfirmPinCommand { get; }
        public Command TogglePostModeCommand { get; }
        public Command CancelPinOrFormCommand { get; }
        public Command SaveFormInfoCommand { get; set; }
        public bool IsPinPlacing { get { return isPinPlacing; } set { SetProperty(ref isPinPlacing, value); } }
        public bool IsPinConfirm { get { return isPinConfirm; } set { SetProperty(ref isPinConfirm, value); } }
       
        // Setup and draws map
        void SetupMap()
        {
            Map = new CustomMap
            {
                MapType = MapType.Street,
                IsShowingUser = true,
            };
        }

        // Requests user for locations permission
        bool CheckLocationPermission()
        {
            CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            var task = Task.Run(async () => await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location));
            var result = task.Result;

            if (result == PermissionStatus.Granted) return true;
            return false;
        }

        // Moves map to user location
        async void GetUserPosition()
        {
            var location = CrossGeolocator.Current;
            var position = await location.GetPositionAsync();
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMeters(120)));
        }

        // Creates pin at location on map click
        void OnMapClick(object sender, MapClickedEventArgs e)
        {
            if (isPinPlacing)
            {
                Map.Pins.Clear();
                TempCustomPin = new CustomPin(e.Position.Latitude, e.Position.Longitude, "Marker " + pinID, pinID);
                Map.Pins.Add(TempCustomPin);
            }
        }

        // Displays all saved pins on map
        void AddPinsToMap()
        {
            CustomPinList.ForEach((pin) => Map.Pins.Add(pin));
        }

        // Reset entry field values
        void ResetEntryFields()
        {
            TitleEntry = "";
            EndTimeEntry = "";
            CategoryEntry = "";
            StartTimeEntry = "";
            DescriptionEntry = "";
        }

        // When '+' button is pressed, enters or leaves pin placement window
        void TogglePostMode()
        {
            Map.Pins.Clear();
            IsPinPlacing = !isPinPlacing;
            IsPinConfirm = false;

            if (!IsPinPlacing) AddPinsToMap();
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
            AddPinsToMap();
            IsPinPlacing = false;
            IsPinConfirm = false;
        }

        // Form entry fields
        private int pinID = 0;
        private string itemsEntry;
        private string titleEntry;
        private string endTimeEntry;
        private string categoryEntry;
        private string startTimeEntry;
        private string descriptionEntry;

        public string ItemsEntry { get { return itemsEntry; } set { SetProperty(ref itemsEntry, value); } }
        public string TitleEntry { get { return titleEntry; } set { SetProperty(ref titleEntry, value); } }
        public string EndTimeEntry { get { return endTimeEntry; } set { SetProperty(ref endTimeEntry, value); } }
        public string CategoryEntry { get { return categoryEntry; } set { SetProperty(ref categoryEntry, value); } }
        public string StartTimeEntry { get { return startTimeEntry; } set { SetProperty(ref startTimeEntry, value); } }
        public string DescriptionEntry { get { return descriptionEntry; } set { SetProperty(ref descriptionEntry, value); } }

        // Saves form inputs
        void SaveFormInfo()
        {
            PostInfo newPost = new PostInfo(pinID, categoryEntry, titleEntry, itemsEntry, descriptionEntry, startTimeEntry, endTimeEntry);

            if (newPost.TitleEntry == null) // checks title exists
            {
                Application.Current.MainPage.DisplayAlert("Missing fields", "You need to provide a title", "Close");
            }
            else
            {
                // Add data to the pin from the entry form
                TempCustomPin.Label = newPost.TitleEntry; // Add title to pin
                string png = CategoryToImage(newPost.CategoryEntry);
                Application.Current.MainPage.DisplayAlert("info", "selected: " + png, "Close");
                TempCustomPin.Address = png;

                //TempCustomPin.Address = newPost.


                // Add pin and post to the lists
                PostInfoList.Add(newPost);
                CustomPinList.Add(TempCustomPin);
                IsPinConfirm = false;
                pinID += 1;

                // remove temp pin, reset the entry fields and add all pins to the map
                Map.Pins.Clear();
                ResetEntryFields();
                AddPinsToMap();
            }
        }

        // Generic command
        async Task DoSomething()
        {
            await Application.Current.MainPage.DisplayAlert("Doing Something", "I don't know what.", "Close");
        }

        string CategoryToImage(string category)
        {
            string png = "not set";
            switch (category)
            {
                case ("Food / Drinks"):
                    png = "food_icon.png";
                    break;
                case ("Stationary / School"):
                    png = "pen_icon.png";
                    break;
                case ("Hygiene products"):
                    png = "health_icon.png";
                    break;
                case ("Sports equipment"):
                    png = "sports_icon.png";
                    break;
                case ("Misc / Others"):
                    png = "misc_icon.png";
                    break;
            }

            return png;
        }
    }
}

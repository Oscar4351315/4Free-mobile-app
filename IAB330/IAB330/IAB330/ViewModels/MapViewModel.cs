using System.Collections.Generic;
using System.Threading.Tasks;
using IAB330.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using CustomRenderer;
using System;


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
                CancelPinOrFormCommand = new Command(() => ResetAll(), () => !IsBusy);
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
        private bool isConfirmButtonEnabled;
        private string formBackgroundColour;
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
        public bool IsConfirmButtonEnabled { get {  return isConfirmButtonEnabled; } set { SetProperty(ref isConfirmButtonEnabled, value); } }
        public string FormBackgroundColour { get { return formBackgroundColour; } set { SetProperty(ref formBackgroundColour, value); } }

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
                IsConfirmButtonEnabled = true;
                TempCustomPin = new CustomPin(e.Position.Latitude, e.Position.Longitude, "Marker " + pinID, pinID);
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(e.Position.Latitude, e.Position.Longitude), Distance.FromMeters(120)));
                Map.Pins.Add(TempCustomPin);
            }
        }

        // When '+' button is pressed, enters or leaves pin placement window
        void TogglePostMode()
        {
            Map.Pins.Clear();
            ResetEntryFields();
            IsPinPlacing = !isPinPlacing;
            IsPinConfirm = false;
            IsConfirmButtonEnabled = false;

            if (!IsPinPlacing) AddPinsToMap();
        }

        // Displays all saved pins on map
        void AddPinsToMap()
        {
            CustomPinList.ForEach((pin) => Map.Pins.Add(pin));
        }

        // When confirmed is pressed on pin placement window
        void ConfirmPin()
        {
            if (Map.Pins.Count == 1)
            {
                IsPinPlacing = false;
                IsPinConfirm = true;
                IsConfirmButtonEnabled = false;
            }
        }

        // When cancel is pressed on pin placement/ form window
        void ResetAll()
        {
            IsPinPlacing = false;
            IsPinConfirm = false;
            IsConfirmButtonEnabled = false;
            Map.Pins.Clear();
            ResetEntryFields();
            AddPinsToMap();
        }

        // Form entry fields
        private int pinID = 0;
        private string categoryEntry;
        private string titleEntry;
        private string itemsEntry;
        private string descriptionEntry;
        private TimeSpan startTimeEntry;
        private TimeSpan endTimeEntry;

        
        // get-set's for the entry form data
        public string CategoryEntry { get { return categoryEntry; } set { SetProperty(ref categoryEntry, value); FormBackgroundColour = categoryEntry; } }
        public string TitleEntry { get { return titleEntry; } set { SetProperty(ref titleEntry, value); _ = (TitleEntry.Length > 0) ? IsConfirmButtonEnabled = true : IsConfirmButtonEnabled = false; } }
        public string ItemsEntry { get { return itemsEntry; } set { SetProperty(ref itemsEntry, value); } }
        public string DescriptionEntry { get { return descriptionEntry; } set { SetProperty(ref descriptionEntry, value); } }
        public TimeSpan StartTimeEntry { get { return startTimeEntry; } set { SetProperty(ref startTimeEntry, value); } }
        public TimeSpan EndTimeEntry { get { return endTimeEntry; } set { SetProperty(ref endTimeEntry, value); } }

        // Reset entry field values
        void ResetEntryFields()
        {
            CategoryEntry = "default";
            TitleEntry = "";
            ItemsEntry = "";
            DescriptionEntry = "";
            StartTimeEntry = TimeSpan.Zero;
            EndTimeEntry = TimeSpan.Zero;
        }

        // Saves entry form inputs
        void SaveFormInfo()
        {
            PostInfo newPost = new PostInfo(pinID, categoryEntry, titleEntry, itemsEntry, descriptionEntry, startTimeEntry, endTimeEntry);

            if (newPost.TitleEntry != null && newPost.TitleEntry.Length > 0) // checks title exists
            {

                // Add data to the pin from the entry form
                TempCustomPin.Label = newPost.TitleEntry; // Add title to pin
                string png = CategoryToImage(newPost.CategoryEntry);
                //Application.Current.MainPage.DisplayAlert("info", "selected: " + endTimeEntry, "Close");
                TempCustomPin.Address = png;


                // Add pin and post to the lists
                PostInfoList.Add(newPost);
                CustomPinList.Add(TempCustomPin);
                pinID += 1;

                // remove temp pin, reset the entry fields and add all pins to the map
                ResetAll();
            }
        }

        // this function gets the category from the entry form and returns
        //      the corresponding image filename
        string CategoryToImage(string category)
        {
            string png = "pin.png";
            switch (category)
            {
                case ("Food / Drink"):
                    png = "food_icon.png";
                    break;
                case ("Health"):
                    png = "health_icon.png";
                    break;
                case ("Stationary"):
                    png = "pen_icon.png";
                    break;
                case ("Sports"):
                    png = "sport_icon.png";
                    break;
                case ("Misc"):
                    png = "misc_icon.png";
                    break;
            }
            // add error handeling here?
            //if (png == "not set") error??;
            return png;
        }

        // Generic command
        async Task DoSomething()
        {
            await Application.Current.MainPage.DisplayAlert("Notice", "Feature not yet implemented", "Close");
        }
    }
}

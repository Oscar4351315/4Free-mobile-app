using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using IAB330.Models;
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
                CreateFakeMarkers();

                Map.MapClicked += OnMapClick;
                GeneralCommand = new Command(async () => await DoSomething(), () => !IsBusy);
                ShowSettingsCommand = new Command(async () => await ShowSettings(), () => !IsBusy);
                TogglePostModeCommand = new Command(() => TogglePostMode(), () => !IsBusy);
                ConfirmPinCommand = new Command(() => ConfirmPin(), () => !IsBusy);
                CancelPinOrFormCommand = new Command(() => ResetAll(), () => !IsBusy);
                SaveFormInfoCommand = new Command(() => SaveFormInfo(), () => !IsBusy);
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
        public Command ShowSettingsCommand { get; }
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

        void CreateFakeMarkers()
        {
            double[] posX = new double[] { -27.472831442, -27.473040, -27.471817 };
            double[] posY = new double[] { 153.023499906, 153.024960, 153.023329 };
            string[] title = new string[] { "Bandaids", "Plushies", "Redbull" };
            string[] category = new string[] { "health_icon.png", "misc_icon.png", "food_icon.png" };

            for (int i = 0; i < 3; i++)
            {
                CustomPin fakePin = new CustomPin(posX[i], posY[i], title[i], pinID, category[i]);
                PostInfo newPost = new PostInfo(pinID, category[i], title[i], "", "", startTimeEntry, endTimeEntry);

                // Add pin and post to the lists
                PostInfoList.Add(newPost);
                CustomPinList.Add(fakePin);
                pinID += 1;
            }

            ResetAll();
        }

        // Requests user for locations permission
        bool CheckLocationPermission()
        {
            CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            do { } while (CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location).Result != PermissionStatus.Granted);
            return true;
        }

        // Moves map to user location
        async void GetUserPosition()
        {
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(-27.472831442, 153.023499906), Distance.FromMeters(120)));

            //var location = CrossGeolocator.Current;
            //var position = await location.GetPositionAsync();
            //Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude), Distance.FromMeters(120)));
        }

        // Creates pin at location on map click
        void OnMapClick(object sender, MapClickedEventArgs e)
        {
            if (isPinPlacing)
            {
                Map.Pins.Clear();
                IsConfirmButtonEnabled = true;
                TempCustomPin = new CustomPin(e.Position.Latitude, e.Position.Longitude, "", pinID);
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

        int GetCurrentTime()
        {
            return DateTime.Now.ToLocalTime().Hour;
        }
        
        // Get-set's for form data
        public string CategoryEntry { get { return categoryEntry; } set { SetProperty(ref categoryEntry, value); FormBackgroundColour = categoryEntry; } }
        public string TitleEntry { get { return titleEntry; } set { SetProperty(ref titleEntry, value); _ = (TitleEntry.Length > 0) ? IsConfirmButtonEnabled = true : IsConfirmButtonEnabled = false; } }
        public string ItemsEntry { get { return itemsEntry; } set { SetProperty(ref itemsEntry, value); } }
        public string DescriptionEntry { get { return descriptionEntry; } set { SetProperty(ref descriptionEntry, value); } }
        public TimeSpan StartTimeEntry { get { return startTimeEntry; } set { SetProperty(ref startTimeEntry, value); } }
        public TimeSpan EndTimeEntry { get { return endTimeEntry; } set { SetProperty(ref endTimeEntry, value); } }

        // Reset form entry fields
        void ResetEntryFields()
        {
            CategoryEntry = "default";
            TitleEntry = "";
            ItemsEntry = "";
            DescriptionEntry = "";

            int currentTime = DateTime.Now.ToLocalTime().Hour;
            StartTimeEntry = TimeSpan.FromHours(currentTime);
            EndTimeEntry = TimeSpan.FromHours(currentTime + 1);
        }

        // Saves entry form inputs
        void SaveFormInfo()
        {
            if (TitleEntry != null && TitleEntry.Length > 0) // checks title exists
            {
                // Add data to the pin from the entry form
                PostInfo newPost = new PostInfo(pinID, categoryEntry, titleEntry, itemsEntry, descriptionEntry, startTimeEntry, endTimeEntry);
                TempCustomPin.Label = newPost.TitleEntry;
                string png = CategoryToImage(newPost.CategoryEntry);
                TempCustomPin.Address = png;


                // Add pin and post to the lists
                PostInfoList.Add(newPost);
                CustomPinList.Add(TempCustomPin);
                pinID += 1;

                ResetAll();
            }
        }

        // Retrives category from entry form and returns the corresponding image filename
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

            return png;
        }


        // Settings Window
        async Task ShowSettings()
        {
            await Application.Current.MainPage.DisplayAlert("Settings", "4Free. Version 1.1.\n\nDeveloped By:\n" +
                "Markus Henrikson, Steven Hua, & Oscar Li", "Close");
        }

        // Generic command
        async Task DoSomething()
        {
            await Application.Current.MainPage.DisplayAlert("Notice", "Feature not yet implemented", "Close");
        }
    }
}

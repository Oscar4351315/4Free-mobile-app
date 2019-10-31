using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using IAB330.Models;
using IAB330.Services;
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
                CreateFakePins();

                Map.MapClicked += OnMapClick;
                ShowSettingsCommand = new Command(async () => await ShowSettings(), () => !IsBusy);
                ToggleQuickAccessCommand = new Command(() => ToggleQuickAccess(), () => !IsBusy);
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
        private bool isShowQuickAccess;
        private string formBackgroundColour;
        private CustomPin selectedPinListItem;
        private CustomPin tempCustomPin = new CustomPin();
        
        // View bindings
        public CustomMap Map { get; set; }
        public ObservableCollection<CustomPin> CustomPinList { get; set; }
        private ObservableCollection<PostInfo> PostInfoList { get; set; }
        public Command ConfirmPinCommand { get; }
        public Command ShowSettingsCommand { get; }
        public Command ToggleQuickAccessCommand { get; }
        public Command TogglePostModeCommand { get; }
        public Command CancelPinOrFormCommand { get; }
        public Command SaveFormInfoCommand { get; set; }
        public bool IsPinPlacing { get { return isPinPlacing; } set { SetProperty(ref isPinPlacing, value); } }
        public bool IsPinConfirm { get { return isPinConfirm; } set { SetProperty(ref isPinConfirm, value); } }
        public bool IsShowQuickAccess { get { return isShowQuickAccess; } set { SetProperty(ref isShowQuickAccess, value); } }
        public bool IsConfirmButtonEnabled { get {  return isConfirmButtonEnabled; } set { SetProperty(ref isConfirmButtonEnabled, value); } }

        // Repositions map when an item from quick access is selected
        public CustomPin SelectedPinListItem
        {
            get { return selectedPinListItem; }
            set
            {
                SetProperty(ref selectedPinListItem, value);
                var pinPosition = selectedPinListItem.Position;
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(pinPosition, Distance.FromMeters(120)));
            }
        }

        // Returns a colour based on the category the user has selected
        public string FormBackgroundColour
        {
            get { return formBackgroundColour; }
            set
            {
                string colour;
                switch (value)
                {
                    case ("Food / Drink"): colour = "#4286F5"; break;
                    case ("Health"): colour = "#EA4235"; break;
                    case ("Stationary"): colour = "#FABD03"; break;
                    case ("Sports"): colour = "#34A853"; break;
                    case ("Misc"): colour = "#A142F4"; break;
                    default: colour = "SlateGray"; break;
                }

                SetProperty(ref formBackgroundColour, colour);
            }
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
                tempCustomPin = new CustomPin { Position = new Position(e.Position.Latitude, e.Position.Longitude), Label = "hi" };
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(e.Position.Latitude, e.Position.Longitude), Distance.FromMeters(120)));
                Map.Pins.Add(tempCustomPin);
            }
        }

        // When the Settings button is pressed, displays message in popup window
        async Task ShowSettings()
        {
            await Application.Current.MainPage.DisplayAlert("Settings", "4Free. Version 1.1\n\nDeveloped By:\n" +
                "Markus Henrikson, Steven Hua, & Oscar Li", "Close");
        }

        // When '+' button is pressed, enters or leaves pin placement window
        void TogglePostMode()
        {
            Map.Pins.Clear();
            ResetEntryFields();
            IsPinPlacing = !isPinPlacing;
            IsPinConfirm = false;
            IsShowQuickAccess = false;
            IsConfirmButtonEnabled = false;
            if (!IsPinPlacing) AddPinsToMap();
        }

        // When the Quick Access button is pressed, toggles its visibility 
        void ToggleQuickAccess()
        {
            if (!IsShowQuickAccess) { ResetAll(); IsShowQuickAccess = true; }
            else IsShowQuickAccess = false;
        }

        // Displays all saved pins on map
        void AddPinsToMap()
        {
            CustomPinList.ForEach((pin) => Map.Pins.Add(pin));
        }

        // When confirmed is pressed on pin placement window
        void ConfirmPin()
        {
            IsPinPlacing = false;
            IsPinConfirm = true;
            IsConfirmButtonEnabled = false;
        }

        // When cancel is pressed on pin placement/ form window
        void ResetAll()
        {
            Map.Pins.Clear();
            AddPinsToMap();
            ResetEntryFields();
            IsPinPlacing = false;
            IsPinConfirm = false;
            IsShowQuickAccess = false;
            IsConfirmButtonEnabled = false;
        }

        // Create fake pins on the map for testing purposes
        void CreateFakePins()
        {
            PostInfoList = new ObservableCollection<PostInfo>();
            CustomPinList = new ObservableCollection<CustomPin>()
            {
                new CustomPin() { Position = new Position(-27.472831, 153.023499), Label = "Bandaids", Address = "icon_health.png", PinID = 0 },
                new CustomPin() { Position = new Position(-27.473040, 153.024960), Label = "Plushies", Address = "icon_misc.png", PinID = 1 },
                new CustomPin() { Position = new Position(-27.471817, 153.023329), Label = "Redbull", Address = "icon_food.png", PinID = 2 },
                new CustomPin() { Position = new Position(-27.472831, 153.023699), Label = "Football", Address = "icon_sport.png", PinID = 3 },
                new CustomPin() { Position = new Position(-27.473040, 153.024910), Label = "Pens", Address = "icon_pen.png", PinID = 4 },
                new CustomPin() { Position = new Position(-27.473040, 153.023329), Label = "Panadol", Address = "icon_health.png", PinID = 5 },
                new CustomPin() { Position = new Position(-27.471817, 153.023499), Label = "Headbands", Address = "icon_misc.png", PinID = 6 },
            };

            CustomPinList.ForEach((pin) => PostInfoList.Add(new PostInfo(pin.PinID, pin.Address, pin.Label, "", "", startTimeEntry, endTimeEntry)));
            pinID = CustomPinList.Count + 1; // Prepares for next pin
            ResetAll();
        }



        // Form entry fields
        private int pinID = 0;
        private string categoryEntry;
        private string titleEntry;
        private string itemsEntry;
        private string descriptionEntry;
        private TimeSpan startTimeEntry;
        private TimeSpan endTimeEntry;

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
            tempCustomPin.Label = TitleEntry;
            tempCustomPin.Address = new ImageService().CategoryToImage(CategoryEntry);
            CustomPinList.Add(tempCustomPin);
            PostInfoList.Add(new PostInfo(pinID, categoryEntry, titleEntry, itemsEntry, descriptionEntry, startTimeEntry, endTimeEntry));
            pinID += 1;
            ResetAll();
        }
    }
}

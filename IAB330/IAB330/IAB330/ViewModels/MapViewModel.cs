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

using IAB330.Services;
using CustomRenderer;
using System.Diagnostics;

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
        private CustomPin tempCustomPin;
        
        // View bindings
        public CustomMap Map { get; set; }
        public ObservableCollection<CustomPin> CustomPinList { get; set; }
        public Command ConfirmPinCommand { get; }
        public Command ShowSettingsCommand { get; }
        public Command ToggleQuickAccessCommand { get; }
        public Command TogglePostModeCommand { get; }
        public Command CancelPinOrFormCommand { get; }
        public Command SaveFormInfoCommand { get; }
        public bool IsPinPlacing { get { return isPinPlacing; } set { SetProperty(ref isPinPlacing, value); } }
        public bool IsPinConfirm { get { return isPinConfirm; } set { SetProperty(ref isPinConfirm, value); } }
        public bool IsShowQuickAccess { get { return isShowQuickAccess; } set { SetProperty(ref isShowQuickAccess, value); UpdatePinTimeRemaining(); } }
        public bool IsConfirmButtonEnabled { get {  return isConfirmButtonEnabled; } set { SetProperty(ref isConfirmButtonEnabled, value); } }
        public string FormBackgroundColour { get { return formBackgroundColour; } set { SetProperty(ref formBackgroundColour, new ImageService().FormBackgroundColour(value)); } }

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

        // Setup and draws map
        void SetupMap()
        {
            Map = new CustomMap { MapType = MapType.Street, IsShowingUser = true, };
        }

        // Creates pin at location on map click
        void OnMapClick(object sender, MapClickedEventArgs e)
        {
            if (isPinPlacing)
            {
                tempCustomPin = new CustomPin
                {
                    Position = new Position(e.Position.Latitude, e.Position.Longitude),
                    Label = " ",
                    Address = new ImageService().CategoryToImage(CategoryEntry)
                };

                Map.Pins.Clear();
                Map.Pins.Add(tempCustomPin);
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(tempCustomPin.Position, Distance.FromMeters(120)));
                IsConfirmButtonEnabled = true;
            }
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

        // Create fake pins on the map for testing purposes
        async Task CreateFakePins()
        {
            CustomPinList = new ObservableCollection<CustomPin>();
            var mockPins = await DataStore.GetItemsAsync(true);
            ResetAll();

            foreach (var pin in mockPins)
            {
                pin.MarkerID = pinID;
                CustomPinList.Add(pin);
                Map.Pins.Add(pin);
                pinID += 1;
            }
        }

        // When the Settings button is pressed, displays message in popup window
        async Task ShowSettings()
        {
            await Application.Current.MainPage.DisplayAlert("Settings", "4Free. Version 1.1\n\nDeveloped By:\nMarkus Henrikson, Steven Hua, & Oscar Li", "Close");
        }

        // When '+' button is pressed, enters or leaves pin placement window
        void TogglePostMode()
        {
            IsPinPlacing = !isPinPlacing;
            IsPinConfirm = false;
            IsConfirmButtonEnabled = false;
            IsShowQuickAccess = false;
            Map.Pins.Clear();
            ResetEntryFields();

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
            IsPinPlacing = false;
            IsPinConfirm = false;
            IsConfirmButtonEnabled = false;
            IsShowQuickAccess = false;
            Map.Pins.Clear();
            AddPinsToMap();
            ResetEntryFields();
        }


        // Form variable declarations
        private int pinID = 1;
        private string categoryEntry;
        private string titleEntry;
        private string itemsEntry;
        private string descriptionEntry;
        private TimeSpan startTimeEntry;
        private TimeSpan endTimeEntry;

        // Form view bindings
        public string CategoryEntry { get { return categoryEntry; } set { SetProperty(ref categoryEntry, value); UpdateFormVisualState(); } }
        public string TitleEntry { get { return titleEntry; } set { SetProperty(ref titleEntry, value); UpdateFormVisualState(); } }
        public string ItemsEntry { get { return itemsEntry; } set { SetProperty(ref itemsEntry, value); } }
        public string DescriptionEntry { get { return descriptionEntry; } set { SetProperty(ref descriptionEntry, value); } }
        public TimeSpan StartTimeEntry { get { return startTimeEntry; } set { SetProperty(ref startTimeEntry, value); } }
        public TimeSpan EndTimeEntry { get { return endTimeEntry; } set { SetProperty(ref endTimeEntry, value); } }

        // Updates the form's visual state based on category and title inputs
        void UpdateFormVisualState()
        {
            FormBackgroundColour = CategoryEntry;
            if (CategoryEntry != null && TitleEntry != null && TitleEntry.Length > 0) IsConfirmButtonEnabled = true;
            else IsConfirmButtonEnabled = false;
        }

        // Reset form entry fields
        void ResetEntryFields()
        {
            CategoryEntry = "";
            TitleEntry = "";
            ItemsEntry = "";
            DescriptionEntry = "";

            // Default start time's minute is rounded floor to nearest 10. Ending time is an hour after.
            TimeSpan currentTime = DateTime.Now.TimeOfDay;
            TimeSpan currentHour = TimeSpan.FromHours(currentTime.Hours);
            TimeSpan flooredMinute = TimeSpan.FromMinutes(((int)Math.Floor((double)currentTime.Minutes / 10)) * 10);
            StartTimeEntry = currentHour + flooredMinute;
            EndTimeEntry = currentHour + flooredMinute + TimeSpan.FromHours(1);
        }

        // Saves entry form inputs in a new pin
        void SaveFormInfo()
        {
            tempCustomPin.MarkerID = pinID;
            tempCustomPin.Category = CategoryEntry;
            tempCustomPin.Label = TitleEntry;
            tempCustomPin.Description = DescriptionEntry;
            tempCustomPin.StartTime = StartTimeEntry;
            tempCustomPin.EndTime = EndTimeEntry;
            tempCustomPin.Address = new ImageService().CategoryToImage(CategoryEntry);
            CustomPinList.Add(tempCustomPin);
            pinID += 1;
            ResetAll();
        }

        // String format of time remaining
        public string FormatTimeRemainingToString(TimeSpan end, TimeSpan start)
        {
            TimeSpan timeLeft = end - start;
            int hours = timeLeft.Hours;
            int minutes = timeLeft.Minutes;

            if (hours <= 0 && minutes <= 0) return "expired";
            else if (hours >= 1) return hours + "h " + minutes + "m left";
            else return minutes + "m left";
        }

        // Updates all pins' remaining time. If 0, remove from list
        public void UpdatePinTimeRemaining()
        {
            // Changing a property doesn't fire INotifyPropertyChange (dev bug), replacing pin does
            for (int i = 0; i < CustomPinList.Count; i++)
            {
                CustomPin tempPin = CustomPinList[i];
                string timeLeft = FormatTimeRemainingToString(tempPin.EndTime, DateTime.Now.TimeOfDay);

                if (timeLeft == "expired") CustomPinList.RemoveAt(i);
                else
                {
                    tempPin.TimeRemaining = timeLeft;
                    CustomPinList[i] = tempPin;
                }
            }
        }
    }
}

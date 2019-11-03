using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using IAB330.Services;
using CustomRenderer;

namespace IAB330.ViewModels
{
    public partial class MapViewModel : BaseViewModel
    {
        public MapViewModel()
        {
            Title = "Map Page";
            SortButtonText = "Distance";
            bool isAllowLocation = CheckLocationPermission();

            if (isAllowLocation)
            {
                Map = new CustomMap { MapType = MapType.Street, IsShowingUser = true, };

                Map.CustomPins = new List<CustomPin> {  };
                CustomPinList = new ObservableCollection<CustomPin>();


                GetUserPosition();
                CreateFakePins();

                Map.MapClicked += OnMapClick;
                ShowSettingsCommand = new Command(() => ShowSettings(), () => !IsBusy);
                ToggleQuickAccessCommand = new Command(() => ToggleQuickAccess(), () => !IsBusy);
                TogglePostModeCommand = new Command(() => TogglePostMode(), () => !IsBusy);
                ConfirmPinCommand = new Command(() => ConfirmPin(), () => !IsBusy);
                CancelPinOrFormCommand = new Command(() => ResetAll(), () => !IsBusy);
                SaveFormInfoCommand = new Command(() => SaveFormInfo(), () => !IsBusy);
                SortButtonCommand = new Command(() => UpdateSort(true), () => !IsBusy);
            }
        }



        // Variable declarations
        private bool isPinPlacing;
        private bool isPinConfirm;
        private bool isConfirmButtonEnabled;
        private bool isShowQuickAccess;
        private string formBackgroundColour;
        private string sortButtonText;
        private CustomPin selectedPinListItem;
        private CustomPin tempCustomPin;
        private Position userPosition;

        // View bindings
        public CustomMap Map { get; set; }
        public ObservableCollection<CustomPin> CustomPinList { get; set; }
        public Command ConfirmPinCommand { get; }
        public Command ShowSettingsCommand { get; }
        public Command ToggleQuickAccessCommand { get; }
        public Command TogglePostModeCommand { get; }
        public Command CancelPinOrFormCommand { get; }
        public Command SaveFormInfoCommand { get; }
        public Command SortButtonCommand { get; }
        public bool IsPinPlacing { get { return isPinPlacing; } set { SetProperty(ref isPinPlacing, value); } }
        public bool IsPinConfirm { get { return isPinConfirm; } set { SetProperty(ref isPinConfirm, value); } }
        public bool IsShowQuickAccess { get { return isShowQuickAccess; } set { SetProperty(ref isShowQuickAccess, value); UpdatePinTimeRemaining(); UpdatePinDistance(); } }
        public bool IsConfirmButtonEnabled { get {  return isConfirmButtonEnabled; } set { SetProperty(ref isConfirmButtonEnabled, value); } }
        public string FormBackgroundColour { get { return formBackgroundColour; } set { SetProperty(ref formBackgroundColour, new ImageService().FormBackgroundColour(value)); } }
        public string SortButtonText { get { return sortButtonText; } set { SetProperty(ref sortButtonText, value); } }


    // Repositions map when an item from quick access is selected
    public CustomPin SelectedPinListItem
        {
            get { return selectedPinListItem; }
            set
            {
                SetProperty(ref selectedPinListItem, value);
                var pinPosition = selectedPinListItem.Position;
                Map.MoveToRegion(MapSpan.FromCenterAndRadius(pinPosition, Distance.FromMeters(100)));
            }
        }


    

        // Creates pin at location on map click
        void OnMapClick(object sender, MapClickedEventArgs e)
        {
            if (isPinPlacing)
            {
                tempCustomPin = new CustomPin
                {
                    Position = new Position(e.Position.Latitude, e.Position.Longitude),
                    Label = "",
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
            var location = CrossGeolocator.Current;
            var position = await location.GetPositionAsync();
            userPosition = new Position(position.Latitude, position.Longitude);
            Map.MoveToRegion(MapSpan.FromCenterAndRadius(userPosition, Distance.FromMeters(120)));
        }

        // Create fake pins from MockDataStore on the map
        async void CreateFakePins()
        {
            CustomPinList = new ObservableCollection<CustomPin>();
            var mockPins = await DataStore.GetItemsAsync(true);
            
            foreach (var pin in mockPins)
            {
                // MockPin's position and end time is an random offset. Add to current for randomness
                double lat = pin.Position.Latitude + userPosition.Latitude;
                double lng = pin.Position.Longitude + userPosition.Longitude;
                pin.Position = new Position(lat, lng);
                pin.EndTime += DateTime.Now.TimeOfDay;

                pin.MarkerID = pinID;
                CustomPinList.Add(pin);
                pinID += 1;
            }

            ResetAll();
        }

        // When the Settings button is pressed, displays message in popup window
        void ShowSettings()
        {
            Application.Current.MainPage.DisplayAlert("Settings", "4Free. Version 1.1\n\nDeveloped By:\nMarkus Henrikson, Steven Hua, & Oscar Li", "Close");
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
            for (int i = 0; i < CustomPinList.Count; i++) {
                Map.Pins.Add(CustomPinList[i]);
                Map.CustomPins.Add(CustomPinList[i]);
            }
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
            UpdateSort();
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

        // Saves entry form inputs into a new pin
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

        

        // Updates all pins' remaining time. If 0, remove from list
        public void UpdatePinTimeRemaining()
        {
            // Changing a property doesn't fire INotifyPropertyChange (dev bug), replacing pin does
            for (int i = 0; i < CustomPinList.Count; i++)
            {
                CustomPin pin = CustomPinList[i];
                string timeLeft = new FormatService().FormatTimeRemainingToString(pin.EndTime, DateTime.Now.TimeOfDay);

                if (timeLeft == "expired") CustomPinList.RemoveAt(i);
                else
                {
                    pin.TimeRemaining = timeLeft;
                    CustomPinList[i] = pin;
                }
            }
        }

        

        // Updates all pins' distance
        public void UpdatePinDistance()
        {
            // Changing a property doesn't fire INotifyPropertyChange (dev bug), replacing pin does
            for (int i = 0; i < CustomPinList.Count; i++)
            {
                CustomPin pin = CustomPinList[i];
                pin.DistanceFromUser = new FormatService().FormatDistanceToString(userPosition, pin.Position).ToString();
                CustomPinList[i] = pin;
            }
        }
        public void UpdateSort(bool toggleMode = false)
        {
            List<CustomPin> tempList = CustomPinList.ToList();
            List<CustomPin> sortedList = new List<CustomPin>();

            if (toggleMode)
            {
                if (SortButtonText == "Time") SortButtonText = "Distance";
                else if (SortButtonText == "Distance") SortButtonText = "Time";
            }

            if (SortButtonText == "Time") sortedList = tempList.OrderBy(pin => Int32.Parse(pin.TimeRemaining.Replace("hr ", "0").Replace("min left", ""))).ToList();
            else if (SortButtonText == "Distance") sortedList = tempList.OrderBy(pin => Int32.Parse(pin.DistanceFromUser.Trim('m'))).ToList();

            CustomPinList.Clear();
            sortedList.ForEach((pin) => CustomPinList.Add(pin));
        }
    }
}

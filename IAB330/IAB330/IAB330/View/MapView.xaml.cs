using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Xaml;
//using Xamarin.Essentials;
using Plugin.Geolocator;
using Plugin.Permissions.Abstractions;
using Plugin.Permissions;
using Xamarin.Forms.PlatformConfiguration;

namespace IAB330.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Page1 : ContentPage
    {
        public Page1()
        {
            InitializeComponent();
            OpenMap();
        }




        async void OpenMap()
        {


            await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);
            var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location);
            if (status != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlert("Need location", "Gunna need that location", "OK");
                    System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow();
                }

                //status = await CrossPermissions.Current.RequestPermissionAsync<LocationPermission>();
            }

            if (status == PermissionStatus.Granted) {
                var location = CrossGeolocator.Current;
                if (location.IsGeolocationEnabled && location.IsGeolocationAvailable)
                {
                    //grabs the user's lat and lng
                    var position = await location.GetPositionAsync();

                    //creates map, start on user's location, add it to a stacklayout
                    var map = new Xamarin.Forms.Maps.Map()
                    {
                        MapType = MapType.Street,
                        IsShowingUser = true
                    };

                    map.MoveToRegion(MapSpan.FromCenterAndRadius(new Position(position.Latitude, position.Longitude),
                                                     Distance.FromMeters(100)));

                    var stack = new StackLayout { Spacing = 0 };
                    stack.Children.Add(map);

                    Content = stack;
                }
            }

            

        }
    }
}
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;
using Android.Content;
using Android.Gms.Maps.Model;
using CustomRenderer.Droid;
using CustomRenderer;


[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CustomRenderer.Droid
{
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {
        List<CustomPin> customPins;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            //Application.Current.MainPage.DisplayAlert("Marker Info Window", "Feature not yet implemented", "Close");
            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                // do something when info window is clicked
                //NativeMap.InfoWindowClick -= OnInfoWindowClick;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);
            
            // do something when the info window is clicked
            //NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);

            map.UiSettings.ZoomControlsEnabled = false;
        }

        // this function is called automatically when the Add(pin) is used to add pins to the map
        protected override MarkerOptions CreateMarker(Pin CustomPin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(CustomPin.Position.Latitude, CustomPin.Position.Longitude));
            marker.SetTitle(CustomPin.Label);
            //marker.SetSnippet(CustomPin.Address);
            //marker.SetIcon(CustomPin.Icon);
            // address is actually the image name for the image to use for the pin
            marker.SetIcon(BitmapDescriptorFactory.FromAsset(CustomPin.Address));
            //marker.SetIcon(BitmapDescriptorFactory.FromAsset("icon_food.png"));
            return marker;
        }

        // self made function, not useful at all
        MarkerOptions CreateCustomMarker(CustomPin customPin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(customPin.Position.Latitude, customPin.Position.Longitude));
            marker.SetTitle(customPin.Label);
            marker.SetSnippet(customPin.Address);
            //marker.SetIcon(CustomPin.Icon);
            marker.SetIcon(BitmapDescriptorFactory.FromAsset(customPin.Icon));
            //marker.SetIcon(BitmapDescriptorFactory.FromAsset("icon_food.png"));
            return marker;
        }

    }
}
using System.Collections.Generic;
using System.Threading.Tasks;
using IAB330.Models;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using CustomRenderer;
using Xamarin.Forms.Maps.Android;
using Android.Gms.Maps;
using Android.Content;
using Android.Gms.Maps.Model;
using Android.Views;
using CustomRenderer.Droid;


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
            throw new System.NotImplementedException();
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

          

            if (e.OldElement != null)
            {
                // do shit when info window is clicked
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
            
            // do shit when the info window is clicked
            //NativeMap.InfoWindowClick += OnInfoWindowClick;
            NativeMap.SetInfoWindowAdapter(this);

            map.UiSettings.ZoomControlsEnabled = false;
        }

        protected override MarkerOptions CreateMarker(Pin pin)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            marker.SetTitle(pin.Label);
            marker.SetSnippet(pin.Address);
            marker.SetIcon(BitmapDescriptorFactory.FromAsset("icon_food.png"));
            return marker;
        }
    }
}
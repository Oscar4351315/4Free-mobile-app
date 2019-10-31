using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.Android;

using CustomRenderer;
using CustomRenderer.Droid;
using IAB330.Droid;
using IAB330.ViewModels;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CustomRenderer.Droid
{
    public class CustomMapRenderer : MapRenderer
    {
        List<CustomPin> customPins;

        public CustomMapRenderer(Context context) : base(context) { }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        protected override void OnElementChanged(Xamarin.Forms.Platform.Android.ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                ;
            }

            if (e.NewElement != null)
            {
                var formsMap = (CustomMap)e.NewElement;
                customPins = formsMap.CustomPins;
                Control.GetMapAsync(this);
            }
        }

        // Disables zoom controls
        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);
            map.UiSettings.ZoomControlsEnabled = false;
        }

        // this function is called automatically when the Add(pin) is used to add pins to the map
        protected override MarkerOptions CreateMarker(Pin annotation)
        {
            var marker = new MarkerOptions();

            marker.SetPosition(new LatLng(annotation.Position.Latitude, annotation.Position.Longitude));
            marker.SetTitle(annotation.Label);
            //marker.SetSnippet("wadhawiduid");
            //marker.SetIcon(CustomPin.Icon);
            // address is actually the image name for the image to use for the pin
            marker.SetIcon(BitmapDescriptorFactory.FromAsset(annotation.Address));
            //marker.SetIcon(BitmapDescriptorFactory.FromAsset("icon_food.png"));
            return marker;
        }

    }
}
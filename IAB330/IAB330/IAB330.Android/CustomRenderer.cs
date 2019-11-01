using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public CustomMapRenderer(Context context) : base(context) { }

        // Disables default controls
        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);
            map.UiSettings.ZoomControlsEnabled = false;
            map.UiSettings.MapToolbarEnabled = false;
        }

        // Automatically called when new pin is added to map
        protected override MarkerOptions CreateMarker(Pin annotation)
        {
            var marker = new MarkerOptions();
            marker.SetPosition(new LatLng(annotation.Position.Latitude, annotation.Position.Longitude));
            marker.SetTitle(annotation.Label);
            marker.SetIcon(BitmapDescriptorFactory.FromAsset(annotation.Address)); // Address is actually the image name for the pin
            return marker;
        }
    }
}
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
    public class CustomMapRenderer : MapRenderer, GoogleMap.IInfoWindowAdapter
    {


        List<CustomPin> customPins;
        public CustomMapRenderer(Context context) : base(context) { }

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


        

        // Disables default controls
        protected override void OnMapReady(GoogleMap map)
        {
            base.OnMapReady(map);
            NativeMap.SetInfoWindowAdapter(this);
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



        

        public Android.Views.View GetInfoContents(Marker marker)
        {
            Console.WriteLine("Calledlmao");
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;


                var customPin = GetCustomPin(marker);

                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }


                view = inflater.Inflate(Resource.Layout.InfoWindow, null);



                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowTitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = customPin.Label;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = customPin.Description + "\n" + customPin.TimeRemaining;
                }

                return view;
            }

            return null;
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        CustomPin GetCustomPin(Marker annotation)
        {
            var position = new Position(annotation.Position.Latitude, annotation.Position.Longitude);
            foreach (var pin in customPins)
            {
                if (pin.Position == position)
                {
                    return pin;
                }
            }
            return null;
        }


 
    }
}
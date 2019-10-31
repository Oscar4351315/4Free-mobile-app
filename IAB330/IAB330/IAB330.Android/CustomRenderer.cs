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
using IAB330.ViewModels;
using System;
using IAB330.Droid;
using Android.Widget;
using System.Linq;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CustomRenderer.Droid
{
    public class CustomMapRenderer : MapRenderer
    {
        List<CustomPin> customPins;

        public CustomMapRenderer(Context context) : base(context)
        {
        }

        public Android.Views.View GetInfoWindow(Marker marker)
        {
            return null;
        }

        public Android.Views.View GetInfoContents(Marker marker)
        {
            var inflater = Android.App.Application.Context.GetSystemService(Context.LayoutInflaterService) as Android.Views.LayoutInflater;
            if (inflater != null)
            {
                Android.Views.View view;

                var customPin = GetCustomPin(marker);
                
                if (customPin == null)
                {
                    throw new Exception("Custom pin not found");
                }


                view = inflater.Inflate(Resource.Layout.Infowindow, null);



                var infoTitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);
                var infoSubtitle = view.FindViewById<TextView>(Resource.Id.InfoWindowSubtitle);

                if (infoTitle != null)
                {
                    infoTitle.Text = customPin.TitleEntry;
                }
                if (infoSubtitle != null)
                {
                    infoSubtitle.Text = customPin.CategoryEntry;
                }

                return view;
            }

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
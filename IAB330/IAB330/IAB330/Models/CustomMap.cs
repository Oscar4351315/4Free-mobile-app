using System.Collections.Generic;
using Xamarin.Forms.Maps;
//using CustomRenderer.Droid;

namespace CustomRenderer
{
    public class CustomMap : Map
    {
        public CustomMap() : base()
        {
        }
        public List<CustomPin> CustomPins { get; set; }
    }
}

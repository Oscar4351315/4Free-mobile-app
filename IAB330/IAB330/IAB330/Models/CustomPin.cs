using Xamarin.Forms.Maps;

namespace CustomRenderer
{
    public class CustomPin : Pin
    {
        public string Icon { get; set; }
        public CustomPin() { }
        public CustomPin(double lat, double lng, string title, int ID, string category = "pin.png")
        {
            Type = PinType.Place;
            Position = new Position(lat, lng);
            Label = title;
            // aight new idea, don't know how to override the custom render with a custom pin so address is now the 
            // address is the name of the image to use for the sustom pin
            Address = category;
            MarkerId = ID;
        }
    }
}

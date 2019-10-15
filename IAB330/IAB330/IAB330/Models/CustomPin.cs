using Xamarin.Forms.Maps;

namespace CustomRenderer
{
    public class CustomPin : Pin
    {
        public string Icon { get; set; }
        public int MarkerId { get; set; }
        public CustomPin() { }
        public CustomPin(double lat, double lng, string title, int ID, string category = "pin.png")
        {
            Type = PinType.Place;
            Position = new Position(lat, lng);
            Label = title;
            Address = category;
            MarkerId = ID;
        }
    }
}

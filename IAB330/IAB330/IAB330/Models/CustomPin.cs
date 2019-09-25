using Xamarin.Forms.Maps;

namespace CustomRenderer
{
    public class CustomPin : Pin
    {
        public CustomPin() { }
        public CustomPin(double lat, double lng, string title, string details, int ID)
        {
            Type = PinType.Place;
            Position = new Position(lat, lng);
            Label = title;
            Address = details;
            MarkerId = ID;
        }

        
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using IAB330.Interfaces;
using Xamarin.Essentials;
using Xamarin.Forms.Maps;

namespace IAB330.Services
{
    public class FormatService : IFormatService{

        // Formats time remaining as a string
        public string FormatTimeRemainingToString(TimeSpan end, TimeSpan start)
        {
            TimeSpan timeLeft = end - start;
            int hours = timeLeft.Hours;
            int minutes = timeLeft.Minutes;

            if (hours <= 0 && minutes <= 0) return "expired";
            else if (hours >= 1) return hours + "hr " + minutes + "min left";
            else return minutes + "min left";
        }


        // Formats the distance the user is from a pin as a string
        public string FormatDistanceToString(Position src, Position dst)
        {
            double distanceKM = Location.CalculateDistance(src.Latitude, src.Longitude, dst.Latitude, dst.Longitude, 0);
            int distanceM = (int)(distanceKM * 1000);
            return distanceM.ToString() + 'm';
        }


    }
}

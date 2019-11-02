using System;
using System.ComponentModel;
using Xamarin.Forms.Maps;

namespace CustomRenderer
{
    public class CustomPin : Pin
    {
        public int MarkerID { get; set; }
        public string Category { get; set; }
        public string Items { get; set; }
        public string Description { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string TimeRemaining { get; set; }
        public string DistanceFromUser { get; set; }
    }
}

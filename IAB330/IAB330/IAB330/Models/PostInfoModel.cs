using System;

namespace IAB330.Models
{
    public class PostInfo : CustomRenderer.CustomPin
    {
        public int PostID { get; set; }
        
        public PostInfo(int pinID, string category, string title, string items, string description, TimeSpan startTime, TimeSpan endTime)
        {
            PinID = pinID;
            Category = category;
            Title = title;
            Items = items;
            Description = description;
            StartTime = startTime;
            EndTime = endTime;
        }
    }
}

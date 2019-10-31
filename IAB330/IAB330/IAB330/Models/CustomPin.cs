using System;
using Xamarin.Forms.Maps;

namespace CustomRenderer
{
    public class CustomPin : Pin
    {
        public string CategoryEntry { get; set; }
        public string TitleEntry { get; set; }
        public string ItemsEntry { get; set; }
        public string DescriptionEntry { get; set; }
        public TimeSpan StartTimeEntry { get; set; }
        public TimeSpan EndTimeEntry { get; set; }
    }
}

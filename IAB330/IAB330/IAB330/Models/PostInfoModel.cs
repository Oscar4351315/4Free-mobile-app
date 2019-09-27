using System;

namespace IAB330.Models
{
    public class PostInfo
    {
        public int PostID { get; set; }
        public string CategoryEntry { get; set; }
        public string TitleEntry { get; set; }
        public string ItemsEntry { get; set; }
        public string DescriptionEntry { get; set; }
        public TimeSpan StartTimeEntry { get; set; }
        public TimeSpan EndTimeEntry { get; set; }

        public PostInfo(
            int postID, string categoryEntry, string titleEntry, string itemsEntry, 
            string descriptionEntry, TimeSpan startTimeEntry, TimeSpan endTimeEntry)
        {
            PostID = postID;
            CategoryEntry = categoryEntry;
            TitleEntry = titleEntry;
            ItemsEntry = itemsEntry;
            DescriptionEntry = descriptionEntry;
            StartTimeEntry = startTimeEntry;
            EndTimeEntry = endTimeEntry;
        }
    }
}

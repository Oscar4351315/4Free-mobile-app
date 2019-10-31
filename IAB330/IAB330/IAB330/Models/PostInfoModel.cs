using System;

namespace IAB330.Models
{
    public class PostInfo : CustomRenderer.CustomPin
    {
        public int PostID { get; set; }
        
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

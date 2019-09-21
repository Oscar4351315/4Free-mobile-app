namespace IAB330.Models
{
    public class PostInfo
    {
        public int PostID { get; set; }
        public string CategoryEntry { get; set; }
        public string TitleEntry { get; set; }
        public string ItemsEntry { get; set; }
        public string DescriptionEntry { get; set; }
        public string StartTimeEntry { get; set; }
        public string EndTimeEntry { get; set; }

        public PostInfo(
            int postID, string categoryEntry, string titleEntry, string itemsEntry, 
            string descriptionEntry, string startTimeEntry, string endTimeEntry)
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

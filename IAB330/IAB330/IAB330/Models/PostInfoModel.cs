using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

using CustomRenderer;

namespace IAB330.Models
{
    // Post class
    public class PostInfo
    {
        private int postID;
        private string categoryEntry;
        private string titleEntry;
        private string itemsEntry;
        private string descriptionEntry;
        private string startTimeEntry;
        private string endTimeEntry;



        // posts ID
        public int PostID
        {
            get { return postID; }
            set { postID = value; }
        }

        // Post's category input
        public string CategoryEntry
        {
            get { return categoryEntry; }
            //set { SetProperty(ref categoryEntry, value); }
            set { categoryEntry = value; }
        }

        // Post's title input
        public string TitleEntry
        {
            get { return titleEntry; }
            //set { SetProperty(ref titleEntry, value); }
            set { titleEntry = value; }
        }

        // Post's items input
        public string ItemsEntry
        {
            get { return itemsEntry; }
            //set { SetProperty(ref itemsEntry, value); }
            set { itemsEntry = value; }
        }

        // Post's description input
        public string DescriptionEntry
        {
            get { return descriptionEntry; }
            //set { SetProperty(ref descriptionEntry, value); }
            set { descriptionEntry = value; }
        }

        // Post's start time input
        public string StartTimeEntry
        {
            get { return startTimeEntry; }
            //set { SetProperty(ref startTimeEntry, value); }
            set { startTimeEntry = value; }
        }

        // Post's end time input
        public string EndTimeEntry
        {
            get { return endTimeEntry; }
            //set { SetProperty(ref endTimeEntry, value); }
            set { endTimeEntry = value; }
        }

        public PostInfo(int postID, string categoryEntry, string titleEntry, string itemsEntry, string descriptionEntry,
                        string startTimeEntry, string endTimeEntry)
        {
            this.postID = postID;
            this.categoryEntry = categoryEntry;
            this.titleEntry = titleEntry;
            this.itemsEntry = itemsEntry;
            this.descriptionEntry = descriptionEntry;
            this.startTimeEntry = startTimeEntry;
            this.endTimeEntry = endTimeEntry;
        }
    }
}

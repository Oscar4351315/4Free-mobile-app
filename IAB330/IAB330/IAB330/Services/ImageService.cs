using System;
using System.Collections.Generic;
using System.Text;

using IAB330.Interfaces;

namespace IAB330.Services
{
    public class ImageService : IImageService
    {
        // Returns a colour based on the category the user has selected
        public string FormBackgroundColour(string category)
        {
            string colour;
            switch (category)
            {
                case ("Food / Drink"): colour = "#4286F5"; break;
                case ("Health"): colour = "#EA4235"; break;
                case ("Stationary"): colour = "#FABD03"; break;
                case ("Sports"): colour = "#34A853"; break;
                case ("Misc"): colour = "#A142F4"; break;
                default: colour = "SlateGray"; break;
            }

            return colour;
        }

        // Retrives a image filename based on the category
        public string CategoryToImage(string category)
        {
            string png;
            switch (category)
            {
                case ("Food / Drink"): png = "icon_food.png"; break;
                case ("Health"): png = "icon_health.png"; break;
                case ("Stationary"): png = "icon_pen.png"; break;
                case ("Sports"): png = "icon_sport.png"; break;
                case ("Misc"): png = "icon_misc.png"; break;
                default: png = "icon_pin.png"; break;
            }

            return png;
        }
    }
}

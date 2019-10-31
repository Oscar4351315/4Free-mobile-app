using IAB330.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace IAB330.Services
{
    public class ImageService : IImageService
    {
        // Retrives category from entry form and returns the corresponding image filename
        public string CategoryToImage(string category)
        {
            string png = "pin.png";
            switch (category)
            {
                case ("Food / Drink"): png = "icon_food.png"; break;
                case ("Health"): png = "icon_health.png"; break;
                case ("Stationary"): png = "icon_pen.png"; break;
                case ("Sports"): png = "icon_sport.png"; break;
                case ("Misc"): png = "icon_misc.png"; break;
            }

            return png;
        }
    }
}

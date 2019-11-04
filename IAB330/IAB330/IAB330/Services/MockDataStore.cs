using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms.Maps;

using CustomRenderer;

namespace IAB330.Services
{
    public class MockDataStore : IDataStore<CustomPin>
    {
        public List<CustomPin> customPinList;

        public MockDataStore()
        {
            customPinList = new List<CustomPin>();

            // Pre-defined items
            var mockPins = new List<CustomPin>
            {
                new CustomPin() { Label = "Bandaids", Address = "icon_health.png", Category = "Health"},
                new CustomPin() { Label = "Plushie", Address = "icon_misc.png", Category = "Misc" },
                new CustomPin() { Label = "Redbull", Address = "icon_food.png", Category = "Food / Drink"},
                new CustomPin() { Label = "Frisbee", Address = "icon_sport.png", Category = "Sports"},
                new CustomPin() { Label = "Pens", Address = "icon_pen.png", Category = "Stationary"},
                new CustomPin() { Label = "Panadol", Address = "icon_health.png", Category = "Health"},
                new CustomPin() { Label = "HairWax", Address = "icon_misc.png", Category = "Misc"},
                new CustomPin() { Label = "Eraser", Address = "icon_pen.png", Category = "Stationary"},
            };

            // Giving mock pins random offsets for position and end time
            // Used in conjunction with user's position and device time
            foreach (CustomPin pin in mockPins)
            {
                Random random = new Random();

                // Random position offset
                int sign = random.Next(0, 2) * 2 - 1;
                double lat = sign * (random.NextDouble() * 0.001303);
                sign = random.Next(0, 2) * 2 - 1;
                double lng = sign * (random.NextDouble() * 0.000519);
                pin.Position = new Position(lat, lng);

                // Random end time offset
                int time = random.Next(5, 121);
                pin.EndTime = TimeSpan.FromMinutes(time);
                pin.DistanceFromUser = "0m";
            }

            mockPins.ForEach((pin) => customPinList.Add(pin));
        }

        public async Task<bool> AddItemAsync(CustomPin pin)
        {
            customPinList.Add(pin);

            return await Task.FromResult(true);
        }

        public async Task<bool> UpdateItemAsync(CustomPin pin)
        {
            var oldItem = customPinList.Where((CustomPin arg) => arg.MarkerID == pin.MarkerID).FirstOrDefault();
            customPinList.Remove(oldItem);
            customPinList.Add(pin);

            return await Task.FromResult(true);
        }

        public async Task<bool> DeleteItemAsync(string id)
        {
            var oldItem = customPinList.Where((CustomPin arg) => arg.MarkerID.ToString() == id).FirstOrDefault();
            customPinList.Remove(oldItem);

            return await Task.FromResult(true);
        }

        public async Task<CustomPin> GetItemAsync(string id)
        {
            return await Task.FromResult(customPinList.FirstOrDefault(s => s.MarkerID.ToString() == id));
        }

        public async Task<IEnumerable<CustomPin>> GetItemsAsync(bool forceRefresh = false)
        {
            return await Task.FromResult(customPinList);
        }
    }
}
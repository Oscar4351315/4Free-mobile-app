using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms.Maps;
using CustomRenderer;

namespace IAB330.Services
{
    public class MockDataStore : IDataStore<CustomPin>
    {
        List<CustomPin> customPinList;

        public MockDataStore()
        {
            customPinList = new List<CustomPin>();
            var mockPins = new List<CustomPin>
            {
                new CustomPin() { Position = new Position(-27.472831, 153.023499), Label = "Bandaids", Address = "icon_health.png"},
                new CustomPin() { Position = new Position(-27.473040, 153.024960), Label = "Plushies", Address = "icon_misc.png" },
                new CustomPin() { Position = new Position(-27.471817, 153.023329), Label = "Redbull", Address = "icon_food.png"},
                new CustomPin() { Position = new Position(-27.472831, 153.023699), Label = "Football", Address = "icon_sport.png"},
                new CustomPin() { Position = new Position(-27.473040, 153.024910), Label = "Pens", Address = "icon_pen.png"},
                new CustomPin() { Position = new Position(-27.473040, 153.023329), Label = "Panadol", Address = "icon_health.png"},
                new CustomPin() { Position = new Position(-27.471817, 153.023499), Label = "Headbands", Address = "icon_misc.png"},
            };

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
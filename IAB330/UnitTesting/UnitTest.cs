using IAB330.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomRenderer;
using Xamarin.Forms.Maps;
using System.Collections.Generic;

namespace UnitTesting
{

    [TestClass]
    public class CutomPinTesting
    {

        [TestMethod]
        public void TestCuomPinConstruction()
        {
            //Arrange

            var pin = new CustomPin
            {
                Label = "snickers",
                Category = "Health"
                
            };

            //Act
            //Assert
            Assert.IsTrue(pin.Label == "snickers", "Title is wrong!, its actually" + pin.Label);
        }

        [TestMethod]
        public void TestAddressChange()
        {
            //Arrange

            var pin = new CustomPin
            {
                Label = "snickers",
                Category = "Health",
                Address = "pin.png"
            };
            //Act
            pin.Address = new ImageService().CategoryToImage("Sports");
            //Assert
            Assert.IsTrue(pin.Address == "icon_sport.png", "Address is wrong!, its actually" + pin.Address);
        }

        [TestMethod]
        public void TestBackgorundColourChange()
        {
            //Arrange

            var pin = new CustomPin
            {
                Label = "snickers",
                Category = "Food / Drink",
                Address = "pin.png"
            };
            //Act
            pin.Category = new ImageService().FormBackgroundColour("Stationary");
            //Assert
            Assert.IsTrue(pin.Category == "#FABD03", "Colour is wrong!, its actually" + pin.Category);
        }
    }

    [TestClass]
    public class CustomMapTesting {

        [TestMethod]
        public void TestCustompinsInCustommap()
        {
            //Arrange
            
            var Map = new CustomMap { MapType = MapType.Street, IsShowingUser = true, };
            Map.CustomPins = new List < CustomPin > { };

            //Act
            Map.CustomPins.Add( new CustomPin() { Label = "Bandaids", Address = "icon_health.png" });
            Map.CustomPins.Add( new CustomPin() { Label = "Plushie", Address = "icon_misc.png" });
            Map.CustomPins.Add( new CustomPin() { Label = "Redbull", Address = "icon_food.png" });
            Map.CustomPins.Add( new CustomPin() { Label = "Frisbee", Address = "icon_sport.png" });

            //Assert
            Assert.IsTrue(Map.MapType == MapType.Street, "Its actually" + Map.MapType);
            Assert.IsTrue(Map.CustomPins.Count == 4, "Its actually" + Map.CustomPins.Count);
            Assert.IsTrue(Map.CustomPins[1].Label == "Plushie", "Its actually" + Map.CustomPins[1].Label);
        }
    }



    [TestClass]
    public class FormatServiceTesting
    {

        [TestMethod]
        public void TestFormatRemainingTime()
        {
            //Arrange
            var testpin = new CustomPin() { Label = "Bandaids", Address = "icon_health.png", StartTime = new System.TimeSpan(22, 10, 10), EndTime = new System.TimeSpan(23, 45, 10) };

            //Act
            testpin.TimeRemaining = new FormatService().FormatTimeRemainingToString(testpin.EndTime, testpin.StartTime);


            //Assert
            Assert.IsTrue(testpin.TimeRemaining == "1hr 35min left", "Its actually" + testpin.TimeRemaining);
        }


        [TestMethod]
        public void TestFormatDistance()
        {
            //Arrange
            var userposition = new Position(-27.477057, 153.028066);
            var pinposition = new Position(-27.468140, 153.023353);

            var testpin = new CustomPin() { Label = "Bandaids", Address = "icon_health.png", Position = pinposition};

            //Act
            testpin.DistanceFromUser = new FormatService().FormatDistanceToString(userposition, testpin.Position);


            //Assert
            Assert.IsTrue(testpin.DistanceFromUser == "1095m", "Its actually" + testpin.DistanceFromUser);
        }
    }
}

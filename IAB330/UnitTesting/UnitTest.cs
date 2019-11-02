using IAB330.ViewModels;
using IAB330.Models;
using IAB330.Services;
using IAB330.Interfaces;
//using NUnit.Framework;
using Xamarin.Forms.Maps;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CustomRenderer;

namespace UnitTesting
{

    [TestClass]
    public class MapViewModelTesting
    {

        [TestMethod]
        public void TestCuomPinConstruction()
        {
            //Arrange

            var pin = new CustomPin
            {
                TitleEntry = "snickers",
                CategoryEntry = "Health"
                
            };

            //Act
            //Assert
            Assert.IsTrue(pin.TitleEntry == "snickers", "title is wrong!, is actually" + pin.TitleEntry);

        }

        [TestMethod]
        public void TestAddressChange()
        {
            //Arrange

            var pin = new CustomPin
            {
                TitleEntry = "snickers",
                CategoryEntry = "Health",
                Address = "pin.png"
            };
            //Act
            pin.Address = new ImageService().CategoryToImage("Sports");
            //Assert
            Assert.IsTrue(pin.Address == "sport_icon.png", "address is wrong!, is actually" + pin.Address);
        }


    }
}

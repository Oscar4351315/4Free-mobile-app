using IAB330.Services;
//using NUnit.Framework;
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
                Label = "snickers",
                Category = "Health"
                
            };

            //Act
            //Assert
            Assert.IsTrue(pin.Label == "snickers", "title is wrong!, is actually" + pin.Label);

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
            Assert.IsTrue(pin.Address == "icon_sport.png", "address is wrong!, is actually" + pin.Address);
        }


    }
}

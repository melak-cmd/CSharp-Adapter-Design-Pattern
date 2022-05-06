using NUnit.Framework;
using Adapter.DataContracts;
using Adapter.DataObjects;
using Adapter.BusinessObjects;

namespace Adapter.Test
{
    [TestFixture]
    public class AdapterTest
    {
        private readonly Adapter _testAdapter = new Adapter();

        [Test]
        [Category("Adapter")]
        [Description("Conversion from DO to BO with DO to convert referencing another DO")]
        public void ConvertBoToDo_ComplexProperties()
        {
            CalendarBo myCalendarBo = new CalendarBo { CustomerName = "My little test calendar" };
            myCalendarBo.ProductList.Add(new RentalRecordBo { StartDate = new System.DateTime(2011, 12, 24), Comment = "Weihnachten" });
            myCalendarBo.ProductList[0].AdditionalInfo.Add("Info1", new AdditionalInfoBo { InfoDescription = "Event description 1" });
            myCalendarBo.ProductList[0].AdditionalInfo.Add("Info2", new AdditionalInfoBo { InfoDescription = "Event description 2" });

            myCalendarBo.ProductList.Add(new RentalRecordBo { StartDate = new System.DateTime(2012, 12, 31), Comment = "Sylvester" });
            myCalendarBo.ProductList[1].AdditionalInfo.Add("Info3", new AdditionalInfoBo { InfoDescription = "Event description 3" });
            myCalendarBo.ProductList[1].AdditionalInfo.Add("Info4", new AdditionalInfoBo { InfoDescription = "Event description 4" });

            dynamic myCalendarDo = (RentalRecordDto)_testAdapter.Convert(myCalendarBo);

            // Test calendar names
            Assert.AreEqual(myCalendarBo.CustomerName, myCalendarDo.CustomerName);
            // Test collection of holidays (generic list)
            Assert.AreEqual(myCalendarBo.ProductList.Count, myCalendarDo.ProductList.Count);
            Assert.AreEqual(myCalendarBo.ProductList[0].GetType(), typeof(RentalRecordBo));
            Assert.AreEqual(myCalendarDo.ProductList[0].GetType(), typeof(RentalDataDto));
            // Test collection of additional info (generic dictionary)
            Assert.AreEqual(myCalendarBo.ProductList[0].AdditionalInfo.Count, myCalendarBo.ProductList[0].AdditionalInfo.Count);
            Assert.AreEqual(myCalendarBo.ProductList[0].AdditionalInfo["Info2"].GetType(), typeof(AdditionalInfoBo));
            Assert.AreEqual(myCalendarDo.ProductList[0].AdditionalInfo["Info2"].GetType(), typeof(AdditionalInfoDto));
            Assert.AreEqual(myCalendarBo.ProductList[0].AdditionalInfo["Info1"].InfoDescription, myCalendarDo.ProductList[0].AdditionalInfo["Info1"].InfoDescription);

        }

        [Test]
        [Category("Adapter")]
        [Description("Conversion from BO to DO containing simple value properties only")]
        public void ConvertBoToDo_SimplePropertiesOnly()
        {
            RentalRecordBo myBo = new RentalRecordBo
            {
                StartDate = new System.DateTime(2011, 12, 24),
                Comment = "Christmas"
            };
            dynamic myDo = (RentalDataDto)_testAdapter.Convert(myBo);

            foreach (var dataProperty in typeof(IBaseRentalDataContract).GetProperties())
            {
                Assert.AreEqual(dataProperty.GetValue(myDo, null), dataProperty.GetValue(myBo, null));
            }
        }

        [Test]
        [Category("Adapter")]
        [Description("Conversion from DO to BO with DO to convert referencing another DO")]
        public void ConvertDoToBo_ComplexProperties()
        {
            RentalRecordDto rentalRecordDto = new RentalRecordDto { CustomerName = "My little test calendar" };
            rentalRecordDto.ProductList.Add(new RentalDataDto { StartDate = new System.DateTime(2011, 12, 24), Comment = "Weihnachten" });
            rentalRecordDto.ProductList.Add(new RentalDataDto { StartDate = new System.DateTime(2012, 12, 31), Comment = "Sylvester" });

            dynamic myCalendarBo = (CalendarBo)_testAdapter.Convert(rentalRecordDto);

            // Test calendar names
            Assert.AreEqual(myCalendarBo.CustomerName, rentalRecordDto.CustomerName);
            // Test collection of holidays (generic list)
            Assert.AreEqual(myCalendarBo.ProductList.Count, rentalRecordDto.ProductList.Count);
            Assert.AreEqual(myCalendarBo.ProductList[0].GetType(), typeof(RentalRecordBo));
            Assert.AreEqual(rentalRecordDto.ProductList[0].GetType(), typeof(RentalDataDto));
        }

        [Test]
        [Category("Adapter")]
        [Description("Conversion from DO to BO containing simple value properties only")]
        public void ConvertDtoToBo_SimplePropertiesOnly()
        {
            RentalDataDto rentalDataDto = new RentalDataDto { StartDate = new System.DateTime(2011, 12, 24), Comment = "Weihnachten" };
            dynamic bo = (RentalRecordBo)_testAdapter.Convert(rentalDataDto);

            foreach (var property in typeof(IBaseRentalDataContract).GetProperties())
            {
                Assert.AreEqual(property.GetValue(rentalDataDto, null), property.GetValue(bo, null));
            }
        }

        [TestFixtureSetUp]
        public void TestsSetup()
        {
            _testAdapter.Register(typeof(IRentalRecordContract), typeof(RentalRecordDto), typeof(CalendarBo));
            _testAdapter.Register(typeof(IRentalRecordContract), typeof(CalendarBo), typeof(RentalRecordDto));
            _testAdapter.Register(typeof(IBaseRentalDataContract), typeof(RentalDataDto), typeof(RentalRecordBo));
            _testAdapter.Register(typeof(IBaseRentalDataContract), typeof(RentalRecordBo), typeof(RentalDataDto));
            _testAdapter.Register(typeof(ICContract), typeof(AdditionalInfoDto), typeof(AdditionalInfoBo));
            _testAdapter.Register(typeof(ICContract), typeof(AdditionalInfoBo), typeof(AdditionalInfoDto));
        }
    }
}

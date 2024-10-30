using SortOrders.Entities;

namespace SortOrders.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void FilterBy_WithValidDistrictAndDeliveryTime_FiltersOrders()
        {
            var orderManager = new OrdersManager([new DeliveryOrder { District = "District1", DeliveryTime = new DateTime(2023, 1, 1) }]);

            orderManager.FilterBy("District1", new DateTime(2023, 1, 1));

            Assert.Single(orderManager.Orders);
        }

        [Fact]
        public void FilterBy_WithInvalidDistrict_DoesNotFilterOrders()
        {
            var orderManager = new OrdersManager([new DeliveryOrder { District = "District1", DeliveryTime = new DateTime(2023, 1, 1) }]);

            orderManager.FilterBy("InvalidDistrict", new DateTime(2023, 1, 1));

            Assert.Empty(orderManager.Orders);
        }

        [Fact]
        public void FilterBy_WithInvalidDeliveryTime_DoesNotFilterOrders()
        {
            var orderManager = new OrdersManager([new DeliveryOrder { District = "District1", DeliveryTime = new DateTime(2023, 1, 1) }]);

            orderManager.FilterBy("District1", new DateTime(2023, 1, 2));

            Assert.Empty(orderManager.Orders);
        }
    }
}

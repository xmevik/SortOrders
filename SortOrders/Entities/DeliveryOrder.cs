namespace SortOrders.Entities
{
    public class DeliveryOrder : Order
    {
        public string District { get; set; } = null!;
        public DateTime DeliveryTime { get; set; }
    }
}

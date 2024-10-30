namespace SortOrders.Entities
{
    public abstract class Order
    {
        public string OrderId { get; set; } = null!;
        public double Weight { get; set; }
    }
}

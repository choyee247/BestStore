namespace Example2.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }
        public DateTime? ShippedDate { get; set; }
        public enum OrderStatus { Pending, Completed, Cancelled }
        public OrderStatus Status { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
    }

}

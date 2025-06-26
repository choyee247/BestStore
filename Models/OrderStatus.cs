namespace Example2.Models
{
    public class OrderStatus
    {
        public static Order.OrderStatus Completed { get; internal set; }
        public static Order.OrderStatus Pending { get; internal set; }
        public static Order.OrderStatus Cancelled { get; internal set; }
    }
}
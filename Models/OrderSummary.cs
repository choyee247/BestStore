namespace Example2.Models
{
    public class OrderSummary
    {
        public int TotalOrders { get; set; }
        public int Completed { get; set; }
        public int Pending { get; set; }
        public int Cancelled { get; set; }
    }
}

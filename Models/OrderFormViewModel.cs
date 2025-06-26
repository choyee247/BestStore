namespace Example2.Models
{
    public class OrderFormViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public string ShippingMethod { get; set; }
        public decimal ShippingCost { get; set; }

        public List<OrderItem>? OrderItems { get; set; }

        public List<ProductSelection> Items { get; set; } = new List<ProductSelection>();
        
    }
}

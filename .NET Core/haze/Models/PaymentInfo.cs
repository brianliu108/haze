namespace haze.Models
{
    public class PaymentInfo
    {
        public int Id { get; set; }
        public string CreditCardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string Address { get; set; }
        public string ShippingAddress { get; set; }
    }
}

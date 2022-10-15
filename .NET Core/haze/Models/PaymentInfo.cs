namespace haze.Models
{
    public class PaymentInfo
    {
        public int Id { get; set; }
        public string CreditCardNumber { get; set; }
        public string ExpiryDate { get; set; }
        public string BillingAddress { get; set; }
        public string ShippingAddress { get; set; }
    }
}

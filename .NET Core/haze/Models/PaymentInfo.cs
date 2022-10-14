namespace haze.Models
{
    public class PaymentInfo
    {
        private int? _paymentInfoId;                

        public int? PaymentInfoId { get { return _paymentInfoId; } }
        public string? CreditCardNumber { get; set; }
        public string? ExpiryDate { get; set; }
        public string? Address { get; set; }
        public string? ShippingAddress { get; set; }
    }
}

namespace haze.Models
{
    public class ProductUserReview
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Description { get; set; }
        public bool Approved{ get; set; }
        public User User { get; set; }
    }
}

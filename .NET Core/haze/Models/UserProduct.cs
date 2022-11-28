namespace haze.Models;

public class UserProduct
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public DateTime DatePurchased { get; set; }
    public virtual Product Product { get; set; }
}
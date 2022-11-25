namespace haze.Models;

public class UserFriend
{
    public int Id { get; set; }
    public virtual Friend Friend { get; set; }
}
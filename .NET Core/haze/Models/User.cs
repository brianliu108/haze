using Microsoft.AspNetCore.Identity;

namespace haze.Models
{
    public class User
    {
        public int Id { get; set; }
        public string stringEmail { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Verified { get; }
        public bool Newsletter { get; set; }
        public string Role { get; }
        public ICollection<PaymentInfo>? PaymentInfos { get; set; }

    }
}

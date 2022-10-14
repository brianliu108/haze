using Microsoft.AspNetCore.Identity;

namespace haze.Models
{
    public class User
    {
        private int? _userId;              

        public int? UserId { get { return _userId; } }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public bool Verified { get; }
        public bool Newsletter { get; set; }
        public string? Role { get; }
        public ICollection<PaymentInfo>? PaymentInfos { get; set; }

    }
}

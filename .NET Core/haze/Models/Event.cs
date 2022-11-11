using haze.DataAccess;
namespace haze.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<EventProduct> Products { get; set; }
        public List<EventUser> RegisteredUsers { get; set; }
    }
}

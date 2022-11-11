namespace haze.Models
{
    public class EventJSON
    {
        public int Id { get; set; }
        public string EventName { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<int> ProductIds { get; set; }
    }
}

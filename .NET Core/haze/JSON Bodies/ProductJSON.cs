namespace haze.Models
{
    public class ProductJSON
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public List<int> CategoryIds { get; set; }
        public List<int> PlatformIds { get; set; }
        public string Description { get; set; }
        public float Price{ get; set; }
    }
}

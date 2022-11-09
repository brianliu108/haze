namespace haze.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public List<Category> Categories { get; set; }
        public List<Platform> Platforms { get; set; }
        public string Description { get; set; }
        public float Price{ get; set; }
    }
}

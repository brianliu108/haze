namespace haze.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public List<ProductCategory> Categories { get; set; }
        public List<ProductPlatform> Platforms { get; set; }
        public string Description { get; set; }
        public float Price{ get; set; }
    }
}

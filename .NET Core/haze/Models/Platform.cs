namespace haze.Models
{
    public class Platform
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum PlatformId
    {
        PC = 1,
        XBox,
        Playstation,
        Nintendo,
        Mobile
    }
}

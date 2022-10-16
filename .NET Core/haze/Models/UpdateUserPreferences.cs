namespace haze.Models
{
    public class UpdateUserPreferences
    {
        public UpdateUserPreferences()
        {
            PlatformIds = new List<int>();
            CategoryIds = new List<int>();
        }

        public List<int> PlatformIds { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}

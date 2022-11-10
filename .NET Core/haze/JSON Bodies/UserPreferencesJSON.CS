namespace haze.Models
{
    public class UserPreferencesJSON
    {
        public UserPreferencesJSON()
        {
            PlatformIds = new List<int>();
            CategoryIds = new List<int>();
        }

        public List<int> PlatformIds { get; set; }
        public List<int> CategoryIds { get; set; }
    }
}

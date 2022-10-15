namespace haze.Models
{
    public class UserPreferences
    {
        public int Id { get; set; }        
        public List<Platform> FavouritePlatforms { get; set; }
        public List<Category> FavouriteCategories { get; set; }
    }
}

namespace haze.Models
{
    public class Category
    {
        public CategoryId Id { get; set; }
        public string Name { get; set; }
    }

    public enum CategoryId
    {
        Shooter,
        Platformer,
        Rogue_Like,
        Strategy,
        MOBA,
        Role_Playing_Game,
        Action_Adventure
    }
}

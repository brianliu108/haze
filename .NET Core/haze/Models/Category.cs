namespace haze.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public enum CategoryId
    {
        Shooter = 1,
        Platformer,
        Rogue_Like,
        Strategy,
        MOBA,
        Role_Playing_Game,
        Action_Adventure
    }
}

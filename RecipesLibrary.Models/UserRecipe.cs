namespace RecipesLibrary.Models
{
    public class UserRecipe
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }
    }
}

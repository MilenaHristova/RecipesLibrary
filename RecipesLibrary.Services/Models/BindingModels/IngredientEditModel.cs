namespace RecipesLibrary.Services.Models
{
    public class IngredientEditModel
    {
        public string Name { get; set; }

        public string Measurement { get; set; }

        public double Quantity { get; set; }

        public bool IsDeleted { get; set; }
    }
}

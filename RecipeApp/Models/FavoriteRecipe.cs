using System;

namespace RecipeApp.Models
{
    public class FavoriteRecipe
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string RecipeId { get; set; }
        public DateTime DateAdded { get; set; }
    }
} 
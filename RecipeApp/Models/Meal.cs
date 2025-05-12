using System.Text.Json.Serialization;

namespace RecipeApp.Models
{
    public class Meal
    {
        [JsonPropertyName("idMeal")]
        public string IdMeal { get; set; }

        [JsonPropertyName("strMeal")]
        public string Name { get; set; }

        [JsonPropertyName("strCategory")]
        public string Category { get; set; }

        [JsonPropertyName("strArea")]
        public string Area { get; set; }

        [JsonPropertyName("strInstructions")]
        public string Instructions { get; set; }

        [JsonPropertyName("strMealThumb")]
        public string Thumbnail { get; set; }

        [JsonPropertyName("strTags")]
        public string Tags { get; set; }

        [JsonPropertyName("strYoutube")]
        public string YoutubeUrl { get; set; }

        [JsonPropertyName("strIngredient1")]
        public string Ingredient1 { get; set; }
        [JsonPropertyName("strIngredient2")]
        public string Ingredient2 { get; set; }
        [JsonPropertyName("strIngredient3")]
        public string Ingredient3 { get; set; }
        [JsonPropertyName("strIngredient4")]
        public string Ingredient4 { get; set; }
        [JsonPropertyName("strIngredient5")]
        public string Ingredient5 { get; set; }
        [JsonPropertyName("strIngredient6")]
        public string Ingredient6 { get; set; }
        [JsonPropertyName("strIngredient7")]
        public string Ingredient7 { get; set; }
        [JsonPropertyName("strIngredient8")]
        public string Ingredient8 { get; set; }
        [JsonPropertyName("strIngredient9")]
        public string Ingredient9 { get; set; }
        [JsonPropertyName("strIngredient10")]
        public string Ingredient10 { get; set; }

        [JsonPropertyName("strMeasure1")]
        public string Measure1 { get; set; }
        [JsonPropertyName("strMeasure2")]
        public string Measure2 { get; set; }
        [JsonPropertyName("strMeasure3")]
        public string Measure3 { get; set; }
        [JsonPropertyName("strMeasure4")]
        public string Measure4 { get; set; }
        [JsonPropertyName("strMeasure5")]
        public string Measure5 { get; set; }
        [JsonPropertyName("strMeasure6")]
        public string Measure6 { get; set; }
        [JsonPropertyName("strMeasure7")]
        public string Measure7 { get; set; }
        [JsonPropertyName("strMeasure8")]
        public string Measure8 { get; set; }
        [JsonPropertyName("strMeasure9")]
        public string Measure9 { get; set; }
        [JsonPropertyName("strMeasure10")]
        public string Measure10 { get; set; }
    }

    public class MealResponse
    {
        [JsonPropertyName("meals")]
        public List<Meal> Meals { get; set; }
    }
} 
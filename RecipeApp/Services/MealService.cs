using System.Net.Http.Json;
using RecipeApp.Models;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Diagnostics;

namespace RecipeApp.Services
{
    public interface IMealService
    {
        Task<List<Meal>> GetMealsAsync();
        Task<List<Category>> GetCategoriesAsync();
        Task<List<string>> GetAreasAsync();
        Task<List<Meal>> SearchMealsAsync(string searchTerm);
        Task<List<Meal>> FilterMealsAsync(List<string> categories, List<string> areas);
        Task<List<Meal>> GetRandomMealsAsync(int count = 10);
        Task<List<string>> GetCategoryNamesAsync();
        Task<Meal> GetMealByIdAsync(string idMeal);
        Task<List<Meal>> GetMealsByFirstLetterAsync(char letter);
    }

    public class MealService : IMealService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MealService> _logger;
        private const string BaseUrl = "https://www.themealdb.com/api/json/v1/1";

        public MealService(HttpClient httpClient, ILogger<MealService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<List<Meal>> GetMealsAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<MealResponse>($"{BaseUrl}/search.php?s=");
                return response?.Meals ?? new List<Meal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching meals");
                throw;
            }
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<CategoryResponse>($"{BaseUrl}/categories.php");
                return response?.Categories ?? new List<Category>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories");
                throw;
            }
        }

        public async Task<List<string>> GetAreasAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<AreaListResponse>("https://www.themealdb.com/api/json/v1/1/list.php?a=list");
            return response?.Meals?.Select(a => a.Area).ToList() ?? new List<string>();
        }

        public async Task<List<Meal>> SearchMealsAsync(string searchTerm)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<MealResponse>($"{BaseUrl}/search.php?s={searchTerm}");
                return response?.Meals ?? new List<Meal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching meals");
                throw;
            }
        }

        public async Task<List<Meal>> GetRandomMealsAsync(int count = 10)
        {
            var meals = new List<Meal>();
            try
            {
                for (int i = 0; i < count; i++)
                {
                    var response = await _httpClient.GetFromJsonAsync<MealResponse>($"{BaseUrl}/random.php");
                    if (response?.Meals?.FirstOrDefault() != null)
                    {
                        meals.Add(response.Meals.First());
                    }
                }
                return meals;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting random meals");
                throw;
            }
        }

        public async Task<List<Meal>> FilterMealsAsync(List<string> categories, List<string> areas)
        {
            try
            {
                string url = null;
                if (categories != null && categories.Any())
                {
                    url = $"{BaseUrl}/filter.php?c={Uri.EscapeDataString(categories.First())}";
                }
                else if (areas != null && areas.Any())
                {
                    url = $"{BaseUrl}/filter.php?a={Uri.EscapeDataString(areas.First())}";
                }
                else
                {
                    return await GetMealsAsync();
                }

                var response = await _httpClient.GetFromJsonAsync<MealResponse>(url);
                return response?.Meals ?? new List<Meal>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering meals");
                throw;
            }
        }

        public async Task<List<string>> GetCategoryNamesAsync()
        {
            var response = await _httpClient.GetFromJsonAsync<CategoryListResponse>("https://www.themealdb.com/api/json/v1/1/list.php?c=list");
            return response?.Meals?.Select(c => c.Name).ToList() ?? new List<string>();
        }

        public async Task<Meal> GetMealByIdAsync(string idMeal)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<MealResponse>($"{BaseUrl}/lookup.php?i={idMeal}");
                return response?.Meals?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "Error fetching meal by id");
                throw;
            }
        }

        public async Task<List<Meal>> GetMealsByFirstLetterAsync(char letter)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<MealResponse>($"{BaseUrl}/search.php?f={letter}");
                return response?.Meals ?? new List<Meal>();
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, $"Error fetching meals by first letter: {letter}");
                throw;
            }
        }
    }

    public class MealComparer : IEqualityComparer<Meal>
    {
        public bool Equals(Meal x, Meal y)
        {
            if (ReferenceEquals(x, y)) return true;
            if (x is null || y is null) return false;
            return x.IdMeal == y.IdMeal;
        }

        public int GetHashCode(Meal obj)
        {
            return obj.IdMeal.GetHashCode();
        }
    }

    public class MealResponse
    {
        public List<Meal> Meals { get; set; }
    }

    public class CategoryResponse
    {
        public List<Category> Categories { get; set; }
    }

    public class AreaListResponse
    {
        public List<AreaListItem> Meals { get; set; }
    }

    public class AreaListItem
    {
        [JsonPropertyName("strArea")]
        public string Area { get; set; }
    }

    public class CategoryListResponse
    {
        public List<CategoryListItem> Meals { get; set; }
    }

    public class CategoryListItem
    {
        [JsonPropertyName("strCategory")]
        public string Name { get; set; }
    }
} 
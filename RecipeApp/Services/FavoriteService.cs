using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RecipeApp.Models;

namespace RecipeApp.Services
{
    public interface IFavoriteService
    {
        Task<bool> AddFavoriteAsync(string recipeId);
        Task<bool> RemoveFavoriteAsync(string recipeId);
        Task<List<string>> GetFavoriteRecipeIdsAsync();
        Task<bool> IsFavoriteAsync(string recipeId);
    }

    public class FavoriteService : IFavoriteService
    {
        private readonly FirestoreService _firestoreService;
        private readonly IAuthService _authService;
        private const string CollectionName = "favorites";

        public FavoriteService(FirestoreService firestoreService, IAuthService authService)
        {
            _firestoreService = firestoreService;
            _authService = authService;
        }

        public async Task<bool> AddFavoriteAsync(string recipeId)
        {
            if (!_authService.IsUserSignedIn())
                return false;

            var userId = _authService.GetCurrentUserId();
            var favorite = new FavoriteRecipe
            {
                Id = $"{userId}_{recipeId}", // Create a unique ID combining user and recipe
                UserId = userId,
                RecipeId = recipeId,
                DateAdded = DateTime.UtcNow
            };

            System.Diagnostics.Debug.WriteLine($"FavoriteService: Adding favorite with Id={favorite.Id}, UserId={userId}, RecipeId={recipeId}");
            return await _firestoreService.CreateDocumentAsync(CollectionName, favorite.Id, favorite);
        }

        public async Task<bool> RemoveFavoriteAsync(string recipeId)
        {
            if (!_authService.IsUserSignedIn())
                return false;

            var userId = _authService.GetCurrentUserId();
            var documentId = $"{userId}_{recipeId}";
            return await _firestoreService.DeleteDocumentAsync(CollectionName, documentId);
        }

        public async Task<List<string>> GetFavoriteRecipeIdsAsync()
        {
            if (!_authService.IsUserSignedIn())
                return new List<string>();

            var userId = _authService.GetCurrentUserId();
            return await _firestoreService.QueryFavoriteRecipeIdsAsync(CollectionName, userId);
        }

        public async Task<bool> IsFavoriteAsync(string recipeId)
        {
            if (!_authService.IsUserSignedIn())
                return false;

            var userId = _authService.GetCurrentUserId();
            var documentId = $"{userId}_{recipeId}";
            var favorite = await _firestoreService.GetDocumentAsync<FavoriteRecipe>(CollectionName, documentId);
            return favorite != null;
        }
    }
} 
# Recipe App

A modern recipe application built with .NET MAUI that helps users discover, save, and share their favorite recipes.

## Features

- Browse recipes by categories and tags
- Search functionality
- Recipe details with ingredients and instructions
- Favorites system
- User authentication
- Profile management

## API Integration Checklist

### 1. Recipe API Integration
- [ ] Choose and integrate a recipe API (e.g., Spoonacular, Edamam, or TheMealDB)
- [ ] Create API service interfaces:
  ```csharp
  public interface IRecipeService
  {
      Task<List<Recipe>> GetFeaturedRecipesAsync();
      Task<List<Recipe>> GetTrendingRecipesAsync();
      Task<List<RecipeCategory>> GetPopularCategoriesAsync();
      Task<List<Recipe>> GetRecipesByCategoryAsync(string categoryId);
      Task<Recipe> GetRecipeDetailsAsync(string recipeId);
      Task<List<Recipe>> SearchRecipesAsync(string query);
  }
  ```
- [ ] Implement API client with proper error handling
- [ ] Add caching mechanism for offline support
- [ ] Implement rate limiting and API key management
- [ ] Add image loading and caching

### 2. Authentication & User Management
- [ ] Set up Firebase Authentication or custom backend
- [ ] Implement Google Sign-In:
  ```csharp
  public interface IAuthService
  {
      Task<bool> SignInWithGoogleAsync();
      Task<bool> SignOutAsync();
      Task<UserProfile> GetCurrentUserAsync();
  }
  ```
- [ ] Add email/password authentication
- [ ] Implement password reset functionality
- [ ] Add email verification
- [ ] Create user profile management

### 3. Database Integration
- [ ] Set up Firebase Realtime Database or custom backend
- [ ] Create data models for:
  - User profiles
  - Favorites
  - Recipe collections
  - User preferences
- [ ] Implement data synchronization
- [ ] Add offline support with local SQLite database

### 4. Cloud Storage TENTATIVE (this is we implement creating a custom recipe probably not)
- [ ] Set up Firebase Storage or alternative
- [ ] Implement image upload for:
  - User profile pictures
  - Custom recipe images
- [ ] Add image compression and optimization
- [ ] Implement secure file access

### 5. API Preparation Tasks

#### Recipe Service
- [ ] Create API configuration class:
  ```csharp
  public class ApiConfig
  {
      public string BaseUrl { get; set; }
      public string ApiKey { get; set; }
      public int TimeoutSeconds { get; set; }
  }
  ```
- [ ] Implement HTTP client factory
- [ ] Add request/response logging
- [ ] Create API response models
- [ ] Add retry policies for failed requests

#### Authentication Service
- [ ] Set up OAuth configuration
- [ ] Implement token management
- [ ] Add secure storage for credentials
- [ ] Create session management

#### Database Service
- [ ] Create database context
- [ ] Implement repository pattern
- [ ] Add migration support
- [ ] Create data access layer

### 6. Security Implementation
- [ ] Implement secure storage for API keys
- [ ] Add request signing
- [ ] Implement proper token management
- [ ] Add data encryption for sensitive information
- [ ] Implement proper error handling and logging


### 7. Performance Optimization
- [ ] Implement lazy loading
- [ ] Add image caching
- [ ] Optimize API calls
- [ ] Implement proper error handling
- [ ] Add analytics and monitoring

## Getting Started

1. Clone the repository
2. Install required dependencies
3. Set up API keys and configurations
4. Run the application

## Configuration

Create a `appsettings.json` file with the following structure:

```json
{
  "ApiSettings": {
    "RecipeApi": {
      "BaseUrl": "https://api.recipe-service.com",
      "ApiKey": "your-api-key",
      "TimeoutSeconds": 30
    },
    "Firebase": {
      "ApiKey": "your-firebase-key",
      "AuthDomain": "your-app.firebaseapp.com",
      "ProjectId": "your-project-id",
      "StorageBucket": "your-bucket.appspot.com"
    }
  }
}
```

## Dependencies

- .NET MAUI
- Firebase Authentication
- Firebase Realtime Database
- Firebase Storage
- SQLite
- Xamarin.Essentials
- Newtonsoft.Json

## Contributing

1. Fork the repository
2. Create a feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## License

This project is licensed under the MIT License - see the LICENSE file for details. 

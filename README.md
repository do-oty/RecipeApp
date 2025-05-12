# Recipe App

A cross-platform mobile application built with .NET MAUI that helps users discover, save, and manage recipes.

## Features

- **User Authentication**
  - Login and Sign Up functionality
  - Google Authentication support
  - User profile management

- **Recipe Management**
  - Browse and explore recipes
  - View detailed recipe information
  - Save favorite recipes
  - Filter and search capabilities

- **Categories and Meals**
  - Organized recipe categories
  - Meal type classification
  - Easy navigation through different food types

## Technical Stack

- **Framework**: .NET MAUI 9.0
- **Architecture**: MVVM (Model-View-ViewModel)
- **Key Dependencies**:
  - CommunityToolkit.Mvvm (8.4.0)
  - CommunityToolkit.Maui (11.2.0)
  - Newtonsoft.Json (13.0.3)
  - Firebase Authentication (for Android)

## Project Structure

```
RecipeApp/
├── Pages/                 # UI Pages
│   ├── HomePage          # Main landing page
│   ├── ExplorePage       # Recipe discovery
│   ├── FavoritesPage     # Saved recipes
│   ├── ProfilePage       # User profile
│   ├── LoginPage         # Authentication
│   └── SignUpPage        # User registration
├── Models/               # Data models
│   ├── Recipe           # Recipe information
│   ├── Category         # Recipe categories
│   ├── Meal            # Meal types
│   └── FavoriteRecipe   # Saved recipes
├── ViewModels/          # View models for MVVM
├── Services/            # Business logic and services
├── Resources/           # App resources
│   ├── Images          # App images and icons
│   ├── Fonts           # Custom fonts
│   └── Raw             # Other resources
└── Platforms/          # Platform-specific code
```

## Supported Platforms

- Android (API 21+)
- iOS (15.0+)
- macOS (15.0+)
- Windows (10.0.17763.0+)

## Getting Started

1. Clone the repository
2. Install .NET 9.0 SDK
3. Install Visual Studio 2022 with .NET MAUI workload
4. Open the solution in Visual Studio
5. Restore NuGet packages
6. Build and run the application

## Development

The application follows the MVVM pattern and uses the CommunityToolkit.Mvvm package for simplified MVVM implementation. The UI is built using XAML and follows modern design principles.

## Dependencies

- Microsoft.Maui.Controls (9.0.60)
- Microsoft.Maui.Graphics (9.0.60)
- CommunityToolkit.Mvvm (8.4.0)
- CommunityToolkit.Maui (11.2.0)
- Newtonsoft.Json (13.0.3)
- Firebase Authentication (Android only)


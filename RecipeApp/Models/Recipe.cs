using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RecipeApp.Models
{
    public class Recipe : INotifyPropertyChanged
    {
        private string id;
        private string name;
        private string image;
        private string caption;
        private string cookTime;
        private string difficulty;
        private string servings;
        private string category;
        private bool isFeatured;
        private double rating;
        private int totalReviews;
        private int trendingScore;
        private DateTime publishedDate;
        private bool isFavorite;
        private List<string> ingredients;
        private List<string> instructions;

        public string Id
        {
            get => id;
            set => SetProperty(ref id, value);
        }

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Image
        {
            get => image;
            set => SetProperty(ref image, value);
        }

        public string Caption
        {
            get => caption;
            set => SetProperty(ref caption, value);
        }

        public string CookTime
        {
            get => cookTime;
            set => SetProperty(ref cookTime, value);
        }

        public string Difficulty
        {
            get => difficulty;
            set => SetProperty(ref difficulty, value);
        }

        public string Servings
        {
            get => servings;
            set => SetProperty(ref servings, value);
        }

        public string Category
        {
            get => category;
            set => SetProperty(ref category, value);
        }

        public bool IsFeatured
        {
            get => isFeatured;
            set => SetProperty(ref isFeatured, value);
        }

        public double Rating
        {
            get => rating;
            set => SetProperty(ref rating, value);
        }

        public int TotalReviews
        {
            get => totalReviews;
            set => SetProperty(ref totalReviews, value);
        }

        public int TrendingScore
        {
            get => trendingScore;
            set => SetProperty(ref trendingScore, value);
        }

        public DateTime PublishedDate
        {
            get => publishedDate;
            set => SetProperty(ref publishedDate, value);
        }

        public bool IsFavorite
        {
            get => isFavorite;
            set => SetProperty(ref isFavorite, value);
        }

        public List<string> Ingredients
        {
            get => ingredients;
            set => SetProperty(ref ingredients, value);
        }

        public List<string> Instructions
        {
            get => instructions;
            set => SetProperty(ref instructions, value);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string? propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
} 
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RecipeApp.Models;

public class SelectableArea : INotifyPropertyChanged
{
    private bool _isSelected;
    private string _area;

    public SelectableArea(string area)
    {
        _area = area;
    }

    public string Area
    {
        get => _area;
        set
        {
            if (_area != value)
            {
                _area = value;
                OnPropertyChanged();
            }
        }
    }

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected != value)
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public override string ToString()
    {
        return Area;
    }
} 
namespace FitnessTracker.Mobile.Views;

using FitnessTracker.Mobile.Models;
using FitnessTracker.Mobile.Services;

public partial class WorkoutsPage : ContentPage
{
    private readonly ApiService _api;

    public WorkoutsPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        var items = await _api.GetWorkoutsAsync();
        WorkoutsList.ItemsSource = items;
    }
}

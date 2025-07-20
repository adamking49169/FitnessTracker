namespace FitnessTracker.Mobile.Views;

using FitnessTracker.Mobile.Services;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _api;

    public LoginPage(ApiService api)
    {
        InitializeComponent();
        _api = api;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        bool success = await _api.LoginAsync(UsernameEntry.Text ?? "", PasswordEntry.Text ?? "");
        if (success)
            await Shell.Current.GoToAsync("workouts");
        else
            await DisplayAlert("Login", "Login failed", "OK");
    }
}

namespace FitnessTracker.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        Routing.RegisterRoute("login", typeof(Views.LoginPage));
        Routing.RegisterRoute("workouts", typeof(Views.WorkoutsPage));
    }
}

using SQLite;

namespace WGUapp;

public partial class LoginPage : ContentPage
{
    public LoginPage()
    {
        ModelViews.createTable();
        InitializeComponent();
    }
    private void btnLogin_Clicked(object sender, EventArgs e)
    {

        FnLogin();

    }
    private async void FnLogin()
    {
        try
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            User user = null;
            string userName = login_email.Text.Trim().ToLower();
            user = ModelViews.GetUserByEmailorUserName(db, userName);
            if (user != null)
            {
                if (user.Password == login_password.Text)
                {
                    App.Current.MainPage = new NavigationPage(new MainPage());  // Start with LoginPage

                    fnClear();
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Error", "Invalid email or password", "OK");
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Invalid email or password", "OK");
            }
        }
        catch (Exception ex)
        {
            await App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}. Something went wrong please try again or contact technilcal team", "OK");
        }
    }

    private void fnClear()
    {
        login_email.Text = "";
        login_password.Text = "";
    }
    private async void btnRegister_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegisterPage());
    }

}
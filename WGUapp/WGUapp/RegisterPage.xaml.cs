using SQLite;

namespace WGUapp;

public partial class RegisterPage : ContentPage
{
    public RegisterPage()
    {
        InitializeComponent();
    }

    private void fnClear()
    {
        RegUserName.Text = "";
        RegEmail.Text = "";
        RegPassword.Text = "";
        RegPhoneNumber.Text = "";
        RegFullName.Text = "";
        btnLogin_Clicked(null, null);
    }

    private void btnRegister_Clicked(object sender, EventArgs e)
    {
        fnRegister();
    }
    private void btnLogin_Clicked(object sender, EventArgs e)
    {
        App.Current.MainPage = new NavigationPage(new LoginPage());

    }
    private void fnRegister()
    {
        try
        {

            if (fnVAlidate())
            {
                User user = new User();
                user.Username = RegUserName.Text;
                user.Email = RegEmail.Text;
                user.Password = RegPassword.Text;
                user.PhoneNo = RegPhoneNumber.Text;
                user.FullName = RegFullName.Text;
                var db = new SQLiteConnection(MainPage.databasePath);
                if (ModelViews.AddUser(db, user) == 1)
                {
                    App.Current.MainPage.DisplayAlert("Success", "New user account successfully created", "OK");
                    fnClear();
                }
                else
                {
                    App.Current.MainPage.DisplayAlert("Error", "Something went wrong please try again or contact technilcal team", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            App.Current.MainPage.DisplayAlert("Error", $"{ex.Message}. Something went wrong please try again or contact technilcal team, ", "OK");

        }

    }


    private bool fnVAlidate()
    {
        var db = new SQLiteConnection(MainPage.databasePath);
        User user = null;
        string userName = RegUserName.Text.Trim().ToLower();
        string email = RegEmail.Text.Trim().ToLower();
        string phoneNumber = RegPhoneNumber.Text.Trim().ToLower();
        string password = RegPassword.Text;
        string confirmPassword = RegConfirPassword.Text;
        user = ModelViews.GetUserByEmail(db, email);
        if (user == null || !string.IsNullOrWhiteSpace(email))
        {
            user = user = ModelViews.GetUserByUserName(db, userName);
            if (user == null || !string.IsNullOrWhiteSpace(userName))
            {
                user = user = ModelViews.GetUserByPhoneNumber(db, phoneNumber);
                if (user == null || !string.IsNullOrWhiteSpace(phoneNumber))
                {
                    if (password.Length >= 5)
                    {

                        if (password == confirmPassword)
                        {
                            return true;
                        }
                        else
                        {
                            App.Current.MainPage.DisplayAlert("Error", "Password and Confirm password dose not match", "OK");
                        }
                    }
                    else
                    {
                        App.Current.MainPage.DisplayAlert("Error", "Password must be atleast 5 charecters", "OK");
                    }

                }
                else
                {
                    if (string.IsNullOrWhiteSpace(phoneNumber))
                    {
                        App.Current.MainPage.DisplayAlert("Error", "The phone number can not be empty or white space", "OK");
                    }
                    else
                        App.Current.MainPage.DisplayAlert("Error", "This phone number is already registred", "OK");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(userName))
                {
                    App.Current.MainPage.DisplayAlert("Error", "The user name can not be empty or white space", "OK");
                }
                else
                    App.Current.MainPage.DisplayAlert("Error", "This user name is already taken", "OK");
            }
        }
        else
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                App.Current.MainPage.DisplayAlert("Error", "The email can not be empty or white space", "OK");
            }
            else
            {
                App.Current.MainPage.DisplayAlert("Error", "This email is already registered", "OK");
            }
        }


        return false;

    }


}
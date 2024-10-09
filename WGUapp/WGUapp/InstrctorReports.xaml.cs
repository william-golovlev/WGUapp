using Android.Telecom;
using SQLite;

namespace WGUapp;

public partial class InstrctorReports : ContentPage
{
    public InstrctorReports()
    {
        InitializeComponent();
        FnLoadData();
        InstructorName.Text = "";
        fnGenerateReport();

    }

    private void FnLoadData()
    {

    }

    private void fnGenerateReport()
    {
        try
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            var instructor = ModelViews.getInstructorReport(db);


            string instructorNameFilter = InstructorName.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(instructorNameFilter))
                instructorNameFilter = "";
            var FilterItem = instructor.Where(x => (x.instructorName.ToLower().Contains(instructorNameFilter) || instructorNameFilter == "")).ToList();
            InstructorCollectionView.ItemsSource = FilterItem;
        }
        catch
        (Exception ex)
        {
            App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }

    }

    private void InstructorName_TextChanged(object sender, TextChangedEventArgs e)
    {
        fnGenerateReport();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirmLogout = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirmLogout)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }
}
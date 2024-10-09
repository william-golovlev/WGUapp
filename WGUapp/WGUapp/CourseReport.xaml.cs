using SQLite;

namespace WGUapp;

public partial class CourseReport : ContentPage
{
    public CourseReport()
    {
        InitializeComponent();
        CourseName.Text = "";
        FnLoadData();
        fnGenerateReport();
    }

    private void FnLoadData()
    {
        var db = new SQLiteConnection(MainPage.databasePath);
        List<Term> terms = ModelViews.getTerm(db);
        List<Term> Resultterms = new List<Term>();
        Resultterms.Add(new Term("All Term", DateTime.Now, DateTime.Now) { termId = 0 });
        foreach (Term term in terms)
        {
            Resultterms.Add(term);
        }
        TermPicker.ItemsSource = Resultterms;
    }

    private void fnGenerateReport()
    {
        try
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            var courses = ModelViews.getCoursesReport(db);

            string courseNameFilter = CourseName.Text.Trim().ToLower();
            if (string.IsNullOrWhiteSpace(courseNameFilter))
                courseNameFilter = "";
            var FilterItem = courses.Where(x => (x.termId == SeletectedTermID || SeletectedTermID == 0) && (x.courseName.ToLower().Contains(courseNameFilter) || courseNameFilter == "")).ToList();
            CoursesCollectionView.ItemsSource = FilterItem;
        }
        catch
        (Exception ex)
        {
            App.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }

    }
    int SeletectedTermID = 0;
    private void OnSelectedTemChange(object sender, EventArgs e)
    {
        SeletectedTermID = ((Term)TermPicker.SelectedItem).termId;
        fnGenerateReport();
    }

    private void CourseName_TextChanged(object sender, TextChangedEventArgs e)
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
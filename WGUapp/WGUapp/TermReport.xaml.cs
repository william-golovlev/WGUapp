using SQLite;

namespace WGUapp;

public partial class TermReport : ContentPage
{
    public TermReport()
    {
        InitializeComponent();
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
            var terms = ModelViews.getTermReport(db);

            var FilterItem = terms.Where(x => (x.termId == SeletectedTermID || SeletectedTermID == 0)).ToList();
            TermCollectionView.ItemsSource = FilterItem;
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
    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirmLogout = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirmLogout)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }

}
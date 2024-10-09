using SQLite;

namespace WGUapp;

public partial class CourseView : ContentPage
{
    public Course currentCourse;
    public Instructor currentInstructor;
    public Assessment PA, OA;
    public SQLiteConnection db = new SQLiteConnection(MainPage.databasePath);
    public CourseView(int courseId)
    {
        Console.WriteLine("THIS COURSE VIEW ID IS: " + courseId);
        InitializeComponent();
        Course course = MainPage.courseList[courseId];
        currentCourse = course;
        currentInstructor = MainPage.instructors[course.instructorId];
        Console.WriteLine(currentInstructor.instructorId + " IS THE CURRENT VIEW INSTRUCOR? OK?");
        PA = MainPage.assessments[course.pa];
        OA = MainPage.assessments[course.oa];
        courseTitle.Text = course.courseName;
        courseStart.Date = course.start;
        courseEnd.Date = course.end;
        courseStatus.ItemsSource = MainPage.statusValues;
        courseStatus.SelectedItem = course.status;
        courseStartNotif.ItemsSource = MainPage.notificationValues;
        courseStartNotif.SelectedItem = course.startNotification;
        courseEndNotif.ItemsSource = MainPage.notificationValues;
        courseEndNotif.SelectedItem = course.endNotification;
        paEndNotif.ItemsSource = MainPage.notificationValues;
        oaEndNotif.ItemsSource = MainPage.notificationValues;
        paStartNotif.ItemsSource = MainPage.notificationValues;
        oaStartNotif.ItemsSource = MainPage.notificationValues;
        oaStart.Date = OA.start;
        oaEnd.Date = OA.end.AddMonths(1);
        paStart.Date = PA.start;
        paEnd.Date = PA.end.AddMonths(1);
        oaDueDate.Date = OA.dueDate;
        paDueDate.Date = PA.dueDate;
        paEndNotif.SelectedItem = PA.endNotif;
        paStartNotif.SelectedItem = PA.startNotif;
        oaEndNotif.SelectedItem = OA.endNotif;
        oaStartNotif.SelectedItem = OA.startNotif;
        instructorName.Text = currentInstructor.instructorName;
        instructorPhone.Text = currentInstructor.instructorPhone;
        instructorEmail.Text = currentInstructor.instructorEmail;
        paName.Text = PA.assessmentName;
        oaName.Text = OA.assessmentName;
        courseDetails.Text = course.courseDetails;

        populateNotes();

    }
    public async Task ShareText(string text)
    {
        await Share.Default.RequestAsync(new ShareTextRequest { Text = text });
    }

    private void populateNotes()
    {
        noteStack.Children.Clear();

        foreach (Note note in MainPage.notes.Values)
        {
            if (note.courseId == currentCourse.courseId)
            {
                SwipeItem shareItem = new SwipeItem
                {
                    Text = "Share",
                    BindingContext = note,
                    BackgroundColor = Colors.LightGray
                };
                SwipeItem deleteItem = new SwipeItem
                {
                    Text = "Delete",
                    BindingContext = note,
                    BackgroundColor = Colors.LightCoral,
                };
                shareItem.Invoked += onShareInvoked;
                deleteItem.Invoked += onDeleteInvoked;
                List<SwipeItem> items = new List<SwipeItem>() { shareItem, deleteItem };
                Grid grid = new Grid
                {
                    BackgroundColor = Colors.LightYellow,

                    //Margin = 5,
                    //Padding = 3

                };
                grid.Add(new Label
                {
                    Text = note.content
                });
                SwipeView swp = new SwipeView
                {
                    RightItems = new SwipeItems(items),
                    Content = grid
                };
                noteStack.Add(swp);

            }
        }
    }

    private void onDeleteInvoked(object sender, EventArgs e)
    {
        var item = sender as SwipeItem;
        var note = item.BindingContext as Note;
        ModelViews.deleteNote(note);
        MainPage.sync_db();
        populateNotes();
    }

    private async void onShareInvoked(object sender, EventArgs e)
    {
        var item = sender as SwipeItem;
        var note = item.BindingContext as Note;
        await ShareText(note.content);
        MainPage.sync_db();
    }

    private void courseStart_DateSelected(object sender, DateChangedEventArgs e)
    {
        bool valid = MainPage.isValidDate(courseStart.Date, courseEnd.Date);
        if (valid)
        {
            currentCourse.start = e.NewDate;
            ModelViews.updateCourse(db, currentCourse);
            MainPage.sync_db();
        }
        else
        {
            DisplayAlert("Error", "Start date must be before end date", "OK");
        }

    }

    private void courseEnd_DateSelected(object sender, DateChangedEventArgs e)
    {
        //var db = new SQLiteConnection(MainPage.databasePath);
        bool valid = MainPage.isValidDate(courseStart.Date, courseEnd.Date);
        if (valid)
        {
            currentCourse.end = e.NewDate;
            ModelViews.updateCourse(db, currentCourse);
            MainPage.sync_db();
        }
        else
        {
            DisplayAlert("Error", "End date must be after start date", "OK");
        }
    }

    private void courseStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        var db = new SQLiteConnection(MainPage.databasePath);
        currentCourse.status = courseStatus.SelectedItem as string;
        ModelViews.updateCourse(db, currentCourse);
        MainPage.sync_db();
    }

    private void courseTitle_TextChanged(object sender, TextChangedEventArgs e)
    {
        var db = new SQLiteConnection(MainPage.databasePath);
        currentCourse.courseName = courseTitle.Text;
        ModelViews.updateCourse(db, currentCourse);
        MainPage.sync_db();
    }

    int emailLen = 0;
    int phoneLen = 0;
    int nameLen = 0;
    protected override bool OnBackButtonPressed()
    {
        if (emailLen == 0 || phoneLen == 0 || nameLen == 0)
        {
            DisplayAlert("Error", "Name, Email, and Phone cannot be empty", "OK");
            return true;
        }
        return base.OnBackButtonPressed();
    }
    private void instructorEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
        emailLen = e.NewTextValue.Length;
        var db = new SQLiteConnection(MainPage.databasePath);
        if (e.NewTextValue != null)
        {
            currentInstructor.instructorEmail = e.NewTextValue;
            ModelViews.updateInstructor(db, currentInstructor);
            MainPage.sync_db();
        }
        else
        {
            DisplayAlert("Error", "Email cannot be empty", "OK");
        }
    }

    private void instructorPhone_TextChanged(object sender, TextChangedEventArgs e)
    {
        phoneLen = e.NewTextValue.Length;
        var db = new SQLiteConnection(MainPage.databasePath);
        if (e.NewTextValue?.Length > 0)
        {
            currentInstructor.instructorPhone = e.NewTextValue;
            ModelViews.updateInstructor(db, currentInstructor);
            MainPage.sync_db();
        }
        else
        {
            DisplayAlert("Error", "Phone number cannot be empty", "OK");
        }
    }

    private void instructorName_TextChanged(object sender, TextChangedEventArgs e)
    {
        nameLen = e.NewTextValue.Length;
        var db = new SQLiteConnection(MainPage.databasePath);
        if (e.NewTextValue?.Length > 0)
        {
            currentInstructor.instructorName = e.NewTextValue;
            ModelViews.updateInstructor(db, currentInstructor);
            MainPage.sync_db();
        }
        else
        {
            DisplayAlert("Error", "Name cannot be empty", "OK");
        }
    }


    private void addNote(object sender, EventArgs e)
    {
        var db = new SQLiteConnection(MainPage.databasePath);
        if (noteInput.Text?.Length > 0)
        {
            Note note = new Note(currentCourse.courseId, noteInput.Text);
            ModelViews.addNote(db, note);
            MainPage.sync_db();
        }
        noteInput.Text = "";
        populateNotes();
    }

    private void courseStartNotif_SelectedIndexChanged(object sender, EventArgs e)
    {
        currentCourse.startNotification = Convert.ToInt32(courseStartNotif.SelectedItem);
        ModelViews.updateCourse(db, currentCourse);
        MainPage.sync_db();

    }
    private void courseEndNotif_SelectedIndexChanged(object sender, EventArgs e)
    {
        currentCourse.endNotification = Convert.ToInt32(courseEndNotif.SelectedItem);
        ModelViews.updateCourse(db, currentCourse);
        MainPage.sync_db();

    }

    private void paStartNotif_SelectedIndexChanged(object sender, EventArgs e)
    {
        PA.startNotif = Convert.ToInt32(paStartNotif.SelectedItem);
        ModelViews.updateAssessment(db, PA);
        MainPage.sync_db();

    }

    private void paEndNotif_SelectedIndexChanged(object sender, EventArgs e)
    {
        PA.endNotif = Convert.ToInt32(paEndNotif.SelectedItem);
        ModelViews.updateAssessment(db, PA);
        MainPage.sync_db();


    }

    private void oaStartNotif_SelectedIndexChanged(object sender, EventArgs e)
    {
        OA.startNotif = Convert.ToInt32(oaStartNotif.SelectedItem);
        ModelViews.updateAssessment(db, OA);
        MainPage.sync_db();

    }

    private void courseDetails_TextChanged(object sender, TextChangedEventArgs e)
    {
        currentCourse.courseDetails = courseDetails.Text;
        ModelViews.updateCourse(db, currentCourse);
        MainPage.sync_db();

    }

    private void oaName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue?.Length > 0)
        {
            OA.assessmentName = e.NewTextValue;
            ModelViews.updateAssessment(db, OA);
            MainPage.sync_db();
        }

    }

    private void paName_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (e.NewTextValue?.Length > 0)
        {
            PA.assessmentName = e.NewTextValue;
            ModelViews.updateAssessment(db, PA);
            MainPage.sync_db();
        }
    }

    private void oaDueDate_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (oaDueDate.Date <= OA.start)
        {
            DisplayAlert("Error", "Due date must be after start date", "OK");
            return;
        }
        OA.dueDate = e.NewDate;
        db.Update(OA);
        MainPage.sync_db();
    }

    private void paDueDate_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (paDueDate.Date <= PA.start)
        {
            DisplayAlert("Error", "Due date must be after start date", "OK");
            return;
        }
        PA.dueDate = e.NewDate;
        db.Update(PA);
        MainPage.sync_db();
    }

    private void oaStart_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate >= OA.end)
        {
            DisplayAlert("Error", "Start date must be before end date", "OK");
            return;
        }
        OA.start = e.NewDate;
        db.Update(OA);
        MainPage.sync_db();
    }

    private void oaEnd_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate <= OA.start)
        {
            DisplayAlert("Error", "End date must be after start date", "OK");
            return;
        }
        OA.end = e.NewDate;
        db.Update(OA);
        MainPage.sync_db();
    }

    private void paStart_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate >= PA.end)
        {
            DisplayAlert("Error", "Start date must be before end date", "OK");
            return;
        }
        PA.start = e.NewDate;
        db.Update(PA);
        MainPage.sync_db();
    }

    private void paEnd_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate <= PA.start)
        {
            DisplayAlert("Error", "End date must be after start date", "OK");
            return;
        }
        PA.end = e.NewDate;
        db.Update(PA);
        MainPage.sync_db();
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        bool confirmLogout = await DisplayAlert("Logout", "Are you sure you want to logout?", "Yes", "No");
        if (confirmLogout)
        {
            App.Current.MainPage = new NavigationPage(new LoginPage());
        }
    }

    private void oaEndNotif_SelectedIndexChanged(object sender, EventArgs e)
    {
        var db = new SQLiteConnection(MainPage.databasePath);
        OA.endNotif = Convert.ToInt32(oaEndNotif.SelectedItem);
        ModelViews.updateAssessment(db, OA);
        MainPage.sync_db();

    }
}
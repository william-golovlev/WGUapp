using Android.Telecom;
using PdfSharp.Fonts;
using Plugin.LocalNotification;
using SQLite;
using System.Collections;
using System.Windows.Input;
using static Android.Graphics.ImageDecoder;

namespace WGUapp
{
    public partial class MainPage : ContentPage
    {
        public static Color darkWGUBlue = Color.FromRgb(0, 48, 87);
        public static Color lightWGUBlue = Color.FromRgb(50, 125, 169);
        public static List<Term> terms = new List<Term>();
        public static Dictionary<Term, List<Course>> courses = new Dictionary<Term, List<Course>>();
        public static Dictionary<int, Course> courseList = new Dictionary<int, Course>();
        public static Dictionary<int, Assessment> assessments = new Dictionary<int, Assessment>();
        public static Dictionary<int, Instructor> instructors = new Dictionary<int, Instructor>();
        public static Dictionary<int, Note> notes = new Dictionary<int, Note>();
        public static Term termSelected;
        public static List<String> statusValues = new List<String>();
        public static List<int> notificationValues = new List<int>();
        public static IList<NotificationRequest> notificationRequestsStatic = new List<NotificationRequest>();
        public static string databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyData.db");
        public static int maxInstructorId = 0;
        public bool IsRegisterPage { get; set; } = false;
        public MainPage()
        {
            InitializeComponent();
            IsRegisterPage = false;
            statusValues = new List<string>();
            notificationValues = new List<int>();
            notificationRequestsStatic = new List<NotificationRequest>();
            courseList = new Dictionary<int, Course>();
            courses = new Dictionary<Term, List<Course>>();
            assessments = new Dictionary<int, Assessment>();
            instructors = new Dictionary<int, Instructor>();
            notes = new Dictionary<int, Note>();
            terms = new List<Term>();
            //addExampleData function will add dummy data. If you don't want that & instead want persisting data, comment out.

            if (ModelViews.getCoursesCount() == 0)
            {
                addExampleData();
                instructors.Add(0, new Instructor("Anika Patel", "555-123-4567", "anika@email.com"));
            }
            load_saved_data();
            load_ui(1);
            statusValues.Add("In Progress");
            statusValues.Add("Completed");
            statusValues.Add("Dropped");
            statusValues.Add("Plan to Take");
            notificationValues.Add(0);
            notificationValues.Add(1);
            notificationValues.Add(2);
            notificationValues.Add(3);
            notificationValues.Add(5);
            notificationValues.Add(7);
            notificationValues.Add(14);
        }

        public async void notificationHander()
        {

            notificationRequestsStatic = await LocalNotificationCenter.Current.GetPendingNotificationList();
            if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
            {
                await LocalNotificationCenter.Current.RequestNotificationPermission();
            }


            List<NotificationRequest> requests = new List<NotificationRequest>();
            List<int> cancelledRequests = new List<int>();
            DateTime tmpdt = DateTime.Now;
            foreach (List<Course> listCourse in courses.Values)
            {
                foreach (Course course in listCourse)
                {
                    //Check if course notif time is 0
                    if (course.startNotification == 0)
                    {
                        cancelledRequests.Add(course.courseId + 1000);
                    }
                    else
                    {
                        NotificationRequest request = new NotificationRequest()
                        {
                            NotificationId = course.courseId + 1000,
                            Title = "Course Starting Reminder",
                            Description = course.courseName + " Starting soon",
                            Schedule = new NotificationRequestSchedule()
                            {
                                NotifyTime = course.start.AddDays(-course.startNotification).AddHours(tmpdt.Hour).AddMinutes(tmpdt.Minute + 1),
                                RepeatType = NotificationRepeat.Daily
                            }
                        };
                        requests.Add(request);
                    }
                    if (course.endNotification == 0)
                    {
                        cancelledRequests.Add(course.courseId + 2000);
                    }
                    else
                    {
                        NotificationRequest request2 = new NotificationRequest()
                        {
                            NotificationId = course.courseId + 2000,
                            Title = "Course Ending Reminder",
                            Description = course.courseName + " Ending soon",
                            Schedule = new NotificationRequestSchedule()
                            {
                                NotifyTime = course.end.AddDays(-course.endNotification).AddHours(tmpdt.Hour).AddMinutes(tmpdt.Minute + 1),
                                RepeatType = NotificationRepeat.Daily
                            }
                        };
                        requests.Add(request2);
                    }

                }
            }

            foreach (Assessment assessment in assessments.Values)
            {
                if (assessment.startNotif == 0)
                {
                    cancelledRequests.Add(assessment.assessmentId + 3000);
                }
                else
                {
                    NotificationRequest request = new NotificationRequest()
                    {
                        NotificationId = assessment.assessmentId + 3000,
                        Title = "Assessment Starting Reminder",
                        Description = assessment.assessmentName + " Starting soon",
                        Schedule = new NotificationRequestSchedule()
                        {
                            NotifyTime = assessment.start.AddDays(-assessment.startNotif).AddHours(tmpdt.Hour).AddMinutes(tmpdt.Minute + 1),
                            RepeatType = NotificationRepeat.Daily
                        }
                    };
                    requests.Add(request);
                }

                if (assessment.startNotif == 0)
                {
                    cancelledRequests.Add(assessment.assessmentId + 4000);
                }
                else
                {
                    NotificationRequest request2 = new NotificationRequest()
                    {
                        NotificationId = assessment.assessmentId + 4000,
                        Title = "Assessment Ending Reminder",
                        Description = assessment.assessmentName + " Ending soon",
                        Schedule = new NotificationRequestSchedule()
                        {
                            NotifyTime = assessment.end.AddDays(-assessment.endNotif).AddHours(tmpdt.Hour).AddMinutes(tmpdt.Minute + 1),
                            RepeatType = NotificationRepeat.Daily
                        }
                    };
                    requests.Add(request2);
                }
            }

            foreach (int i in cancelledRequests)
            {
                foreach (NotificationRequest notfi in notificationRequestsStatic)
                {
                    if (notfi.NotificationId == i)
                    {
                        notfi.Cancel();
                    }
                }
            }
            foreach (NotificationRequest request in requests)
            {
                await LocalNotificationCenter.Current.Show(request);

            }
        }

        protected override void OnAppearing()
        {
            load_ui(termSelected.termId);
            notificationHander();
        }

        public void addExampleData()
        {
            File.Delete(databasePath);
            ModelViews.createTable();
            var db = new SQLiteConnection(databasePath);
            Term term1 = new Term("Term 1", DateTime.Now, DateTime.Now.AddMonths(6));
            //Term term2 = new Term("Term 2", DateTime.Now, DateTime.Now.AddMonths(6));
            ModelViews.addTerm(db, term1);
            //ModelViews.addTerm(db, term2);
            Course course1 = new Course(1, 1, "Discrete Math", DateTime.Now, DateTime.Now.AddMonths(4), "In Progress", "Enter Course Details Here:", 1, 2);
            //Course course2 = new Course(1, 1, "Advanced C#", DateTime.Now, DateTime.Now.AddMonths(4), "In Progress", "Enter Course Details Here:", 3, 4);
            //Course course3 = new Course(1, 1, "UI/UX Design", DateTime.Now, DateTime.Now.AddMonths(4), "In Progress", "Enter Course Details Here:", 1, 1);
            //Course course4 = new Course(1, 1, "Cloud Foundations", DateTime.Now, DateTime.Now.AddMonths(4), "In Progress", "Enter Course Details Here:", 1, 1);
            //Course course5 = new Course(1, 1, "Data Management", DateTime.Now, DateTime.Now.AddMonths(4), "In Progress", "Enter Course Details Here:", 1, 1);
            //Course course6 = new Course(1, 1, "Software Pentesting", DateTime.Now, DateTime.Now.AddMonths(4), "In Progress", "Enter Course Details Here:", 1, 1);
            ModelViews.addCourse(db, course1);

            Assessment PA = new Assessment(1, "Performance Assessment #1", DateTime.Now, DateTime.Now.AddMonths(3), "Enter details about assessment:", 1);
            Assessment OA = new Assessment(0, "Objective Assessment #1", DateTime.Now, DateTime.Now.AddMonths(3), "Enter details about assessment:", 1);

            ModelViews.addAssessment(db, PA);
            Instructor instructor = new Instructor("Anika Patel", "555-123-4567", "anika.patel@strimeuniversity.edu");
            ModelViews.addInstructor(db, instructor);
            Note note1 = new Note(1, "This is a note about the course");
            ModelViews.addNote(db, note1);
            note1 = new Note(1, "SWIPE TO DELETE ME");
            ModelViews.addNote(db, note1);
        }

        private void load_saved_data()
        {

            var db = new SQLiteConnection(databasePath);

            var tmpterm = db.Query<Term>("SELECT * FROM Terms");
            foreach (Term term in tmpterm)
            {
                terms.Add(term);
            }

            foreach (Term term in terms)
            {
                var tmpcourses = db.Query<Course>($"SELECT * FROM Courses WHERE termId={term.termId}");
                List<Course> CourseList = new List<Course>();
                foreach (Course course in tmpcourses)
                {
                    CourseList.Add(course);
                    courseList.Add(course.courseId, course);
                }
                courses.Add(term, CourseList);
            }
            var tmpAssessment = db.Query<Assessment>("SELECT * FROM Assessments");
            foreach (Assessment assessment in tmpAssessment)
            {
                assessments.Add(assessment.assessmentId, assessment);
            }
            var tmpInstructor = db.Query<Instructor>("SELECT * FROM Instructors");
            foreach (Instructor instructor in tmpInstructor)
            {
                instructors.Add(instructor.instructorId, instructor);
            }
            var tmpNote = db.Query<Note>("SELECT * FROM Notes");
            foreach (Note note in tmpNote)
            {
                notes.Add(note.noteId, note);
            }
            db.Close();
        }

        public static void sync_db()
        {
            terms = new List<Term>();
            courses = new Dictionary<Term, List<Course>>();
            courseList = new Dictionary<int, Course>();
            assessments = new Dictionary<int, Assessment>();
            instructors = new Dictionary<int, Instructor>();
            notes = new Dictionary<int, Note>();
            var db = new SQLiteConnection(databasePath);
            var tmpterm = db.Query<Term>("SELECT * FROM Terms");
            foreach (Term term in tmpterm)
            {
                terms.Add(term);
            }

            foreach (Term term in terms)
            {
                var tmpcourses = db.Query<Course>($"SELECT * FROM Courses WHERE termId={term.termId}");
                List<Course> CourseList = new List<Course>();
                foreach (Course course in tmpcourses)
                {
                    CourseList.Add(course);
                    courseList.Add(course.courseId, course);
                }
                courses.Add(term, CourseList);
            }
            var tmpAssessment = db.Query<Assessment>("SELECT * FROM Assessments");
            foreach (Assessment assessment in tmpAssessment)
            {
                assessments.Add(assessment.assessmentId, assessment);
            }
            var tmpInstructor = db.Query<Instructor>("SELECT * FROM Instructors");
            foreach (Instructor instructor in tmpInstructor)
            {
                instructors.Add(instructor.instructorId, instructor);
            }
            var tmpNote = db.Query<Note>("SELECT * FROM Notes");
            foreach (Note note in tmpNote)
            {
                notes.Add(note.noteId, note);
            }
            db.Close();
        }

        public void load_ui(int term)
        {

            termStack.Children.Clear();
            courseStack.Children.Clear();
            // ERROR HERE
            //Console.WriteLine(termSelected.termId + " IS THE ID OK?");
            Console.WriteLine(term + " IS THE TERM #");
            termSelected = terms.FirstOrDefault(x => x.termId == term);
            Console.WriteLine(termSelected.termId + " IS THE NEW NEW  ID OK?");

            //Foreach Term
            foreach (Term tmpterm in terms)
            {
                Button button = new Button
                {
                    Text = tmpterm.termName,
                    Padding = 5,
                    BackgroundColor = darkWGUBlue,
                    TextColor = Colors.White,
                    CornerRadius = 5,
                };
                button.Clicked += void (sender, args) => load_ui(tmpterm.termId);
                termStack.Children.Add(button);
            }

            Button buttonTermAdd = new Button()
            {
                Text = "Add Term",
                Padding = 5,
                BackgroundColor = darkWGUBlue,
                TextColor = Colors.White,
                CornerRadius = 5,
            };
            buttonTermAdd.Clicked += void (sender, args) => onNewTerm();
            termStack.Children.Add(buttonTermAdd);

            //Foreach Course in Term
            foreach (Course course in courses[termSelected])
            {
                Grid grid = new Grid
                {
                    BackgroundColor = Colors.White
                };
                //Add Course Button
                Button button = new Button
                {
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.WhiteSmoke,
                    Text = course.courseName,
                    BackgroundColor = lightWGUBlue,
                };
                grid.Add(button);
                //Adds swipe functionality to delete course, aka Red button
                SwipeItem deleteItem = new SwipeItem
                {
                    Text = "Delete",
                    BindingContext = course,
                    BackgroundColor = Colors.LightCoral

                };
                deleteItem.Invoked += onDeleteInvoked;
                List<SwipeItem> items = new List<SwipeItem>() { deleteItem };
                SwipeView swp = new SwipeView
                {
                    RightItems = new SwipeItems(items),
                    Content = grid
                };
                button.Clicked += async (sender, args) => await Navigation.PushAsync(new CourseView(course.courseId));

                courseStack.Children.Add(swp);
            }
            if (courses[termSelected].Count < 6)
            {
                Button buttonCourseAdd = new Button()
                {
                    Text = "Add Course",
                    FontSize = 18,
                    BackgroundColor = Colors.WhiteSmoke,
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Colors.Black
                };
                buttonCourseAdd.Clicked += void (sender, args) => onNewCourse();
                courseStack.Children.Add(buttonCourseAdd);
            }
            if (courses[termSelected].Count == 0)
            {
                Button buttonTermRemove = new Button()
                {
                    Text = "Delete Term",
                    BackgroundColor = Colors.Red
                };
                buttonTermRemove.Clicked += void (sender, args) => onTermDelete();
                courseStack.Children.Add(buttonTermRemove);
            }



            termStart.Date = termSelected.start;
            termEnd.Date = termSelected.end;
            termTitle.Text = termSelected.termName;
        }

        public void onNewCourse()
        {
            ModelViews.addNewCourse(termSelected.termId);
            load_ui(termSelected.termId);
        }
        public void onNewTerm()
        {
            ModelViews.addNewTerm();
            load_ui(termSelected.termId);
        }
        public async void onTermDelete()
        {
            var lastTerm = terms.OrderBy(x => x.termId).Last();
            if (termSelected.termId != lastTerm.termId)
            {
                await DisplayAlert("Error", "You cannot remove a term that is not the most recent/last term... It isn't logical.", "OK");
                return;
            }
            else if (terms.Count <= 1)
            {
                await DisplayAlert("Error", "You cannot remove the last term", "OK");
                return;
            }
            var db = new SQLiteConnection(databasePath);
            db.Delete(termSelected);
            sync_db();
            load_ui(1);
        }
        private void onDeleteInvoked(object sender, EventArgs e)
        {
            var item = sender as SwipeItem;
            var course = item.BindingContext as Course;
            ModelViews.deleteCourse(course);
            MainPage.sync_db();
            load_ui(termSelected.termId);
        }
        public static bool isValidDate(DateTime start, DateTime end)
        {
            if (end < start)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void termTitleChange(object sender, TextChangedEventArgs e)
        {
            var db = new SQLiteConnection(databasePath);

            if (e.NewTextValue != null)
            {
                termSelected.termName = e.NewTextValue;
                ModelViews.updateTerm(db, termSelected);
                load_ui(termSelected.termId);
            }
        }

        public DateTime prevTermStartDate;
        public DateTime prevTermEndDate;

        public async void termEnd_DateSelected(object sender, DateChangedEventArgs e)
        {
            var db = new SQLiteConnection(databasePath);
            if (termStart.Date > e.NewDate)
            {
                Console.WriteLine("ILLEGAL DATE END CHOSEN");
                await DisplayAlert("Error", "End date cannot be before start date", "OK");
                termEnd.Date = prevTermEndDate;
                return;
            }
            prevTermEndDate = termEnd.Date;
            termEnd.Date = e.NewDate;
            termSelected.end = e.NewDate;
            ModelViews.updateTerm(db, termSelected);
        }

        public async void termStart_DateSelected(object sender, DateChangedEventArgs e)
        {
            var db = new SQLiteConnection(databasePath);
            if (e.NewDate > termEnd.Date)
            {
                Console.WriteLine("ILLEGAL START DATE CHOSEN");
                await DisplayAlert("Error", "Start date cannot be after end date", "OK");
                termStart.Date = prevTermStartDate;
                return;
            }
            prevTermStartDate = termStart.Date;
            termStart.Date = e.NewDate;
            termSelected.start = e.NewDate;
            ModelViews.updateTerm(db, termSelected);

        }

        private void btnTermReport_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new TermReport());
        }

        private void btnCourseReport_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CourseReport());
        }

        private void btnInstructorReport_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new InstrctorReports());
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
}
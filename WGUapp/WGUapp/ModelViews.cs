using SQLite;
using System.Linq;


namespace WGUapp
{
    public static class ModelViews
    {
        public static void deleteNote(Note note)
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            db.Delete(note);
        }

        public static void deleteCourse(Course course)
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            db.Delete(course);
        }

        public static void addNewCourse(int termId)
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            Assessment PA = new Assessment(1, "PerformanceAssessmentExample", DateTime.Now, DateTime.Now.AddMonths(3), "Enter details about assessment here:", 1);
            Assessment OA = new Assessment(0, "ObjectiveAssessmentExample", DateTime.Now, DateTime.Now.AddMonths(3), "Enter details about assessment here:", 1);

            //test for max id
            int max_id = 0;
            foreach (int id in MainPage.instructors.Keys)
            {
                if (id >= max_id)
                {
                    Console.WriteLine("ONE ID IS: " + id);
                    max_id = id;
                }
            }
            Console.WriteLine("AIAIAIAIIAIAIA");
            Console.WriteLine(max_id);
            Console.WriteLine("THIS IS THE ID BEING ASSIGNED!!!!");
            MainPage.maxInstructorId = max_id;

            //used to be 1, then wa
            Course course1 = new Course(termId, max_id + 1, "New Course", DateTime.Now, DateTime.Now.AddMonths(4), "Plan to Take", "Example Details Here", 1, 2);
            addCourse(db, course1);
            MainPage.instructors.Add(max_id + 1, new Instructor("WGU Instructor", "TBD", "TBD"));
            addInstructor(db, MainPage.instructors[max_id + 1]);
            List<Course> resp = db.Query<Course>($"SELECT courseId FROM Courses WHERE courseName='New Course'");
            PA.courseId = resp[0].courseId;
            OA.courseId = resp[0].courseId;
            PA.end = PA.start.AddMonths(1);
            OA.end = OA.start.AddMonths(1);
            PA.dueDate = PA.end;
            OA.dueDate = OA.end;
            addAssessment(db, PA);
            addAssessment(db, OA);
            List<Assessment> resp2 = db.Query<Assessment>($"SELECT assessmentId FROM Assessments WHERE courseId='{resp[0].courseId.ToString()}'");
            foreach (Assessment a in resp2)
            {
                if (a.type == 1)
                {
                    course1.pa = a.assessmentId;
                }
                else
                {
                    course1.oa = a.assessmentId;
                }
            }
            db.Update(course1);
            MainPage.sync_db();
            db.Close();

        }
        public static void addNewTerm()
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            var resp = db.Query<Term>($"SELECT * FROM Terms ORDER BY termId DESC LIMIT 1");
            Term nt = resp.First();

            var count = db.Table<Term>().Count();
            string termName = "Term " + (count + 1);
            Term rt = new Term(termName, DateTime.Now, DateTime.Now.AddDays(60));
            db.Insert(rt);
            MainPage.sync_db();
            db.Close();
        }

        //public static void addNewCourse(int termId)
        //{
        //    var db = new SQLiteConnection(MainPage.databasePath);
        //    Assessment PA = new Assessment(1, "PerformanceAssessmentExample", DateTime.Now, DateTime.Now.AddMonths(3), "Enter details about assessment here:", 1);
        //    Assessment OA = new Assessment(0, "ObjectiveAssessmentExample", DateTime.Now, DateTime.Now.AddMonths(3), "Enter details about assessment here:", 1);
        //    Course course1 = new Course(termId, 1, "NewCourseExample", DateTime.Now, DateTime.Now.AddMonths(4), "Plan to Take", "Example Details Here", 1, 2);
        //    addCourse(db, course1);
        //    List<Course> resp = db.Query<Course>($"SELECT courseId FROM Courses WHERE courseName='NewCourseExample'");
        //    PA.courseId = resp[0].courseId;
        //    OA.courseId = resp[0].courseId;
        //    addAssessment(db, PA);
        //    addAssessment(db, OA);
        //    List<Assessment> resp2 = db.Query<Assessment>($"SELECT assessmentId FROM Assessments WHERE courseId='{resp[0].courseId.ToString()}'");
        //    foreach (Assessment a in resp2)
        //    {
        //        if (a.type == 1)
        //        {
        //            course1.pa = a.assessmentId;
        //        }
        //        else
        //        {
        //            course1.oa = a.assessmentId;
        //        }
        //    }
        //    db.Update(course1);
        //    MainPage.sync_db();


        //}
        public static void createTable()
        {
            var databasePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MyData.db");
            var db = new SQLiteConnection(databasePath);
            db.CreateTable<Term>();
            db.CreateTable<Course>();
            db.CreateTable<Assessment>();
            db.CreateTable<Instructor>();
            db.CreateTable<Note>();
            db.CreateTable<User>();
        }
        public static int AddUser(SQLiteConnection db, User user)
        {
            return db.Insert(user);
        }

        public static User GetUserByEmail(SQLiteConnection db, string email)
        {
            email = email.Trim().ToLower();
            var allusers = db.Table<User>().ToList();
            return allusers.Where(u => u.Email.ToLower() == email).FirstOrDefault();

        }
        public static User GetUserByPhoneNumber(SQLiteConnection db, string phoneNumber)
        {
            phoneNumber = phoneNumber.Trim().ToLower();
            var allusers = db.Table<User>().ToList();
            return allusers.Where(u => u.PhoneNo.ToLower() == phoneNumber).FirstOrDefault();
        }
        public static User GetUserByEmailorUserName(SQLiteConnection db, string emailorUserName)
        {
            emailorUserName = emailorUserName.Trim().ToLower();
            var allusers = db.Table<User>().ToList();
            return allusers.Where(u => u.Email.ToLower() == emailorUserName || u.Username.ToLower() == emailorUserName).FirstOrDefault();
        }
        public static User GetUserByUserName(SQLiteConnection db, string userName)
        {
            userName = userName.Trim().ToLower();
            var allusers = db.Table<User>().ToList();
            return allusers.Where(u => u.Username.ToLower() == userName).FirstOrDefault();
        }

        public static List<User> GetUsers(SQLiteConnection db)
        {
            return db.Table<User>().ToList();
        }
        public static void addTerm(SQLiteConnection db, Term term)
        {
            db.Insert(term);

        }
        public static List<Term> getTerm(SQLiteConnection db)
        {
            return db.Table<Term>().ToList();

        }
        public static Term getTermbyTermID(SQLiteConnection db, int termID)
        {
            return db.Table<Term>().Where(x => x.termId == termID).FirstOrDefault();

        }
        public static void updateTerm(SQLiteConnection db, Term term)
        {
            db.Update(term);
        }
        public static void addCourse(SQLiteConnection db, Course course)
        {
            db.Insert(course);
        }
        public static void updateCourse(SQLiteConnection db, Course course)
        {
            db.Update(course);
        }
        public static void addAssessment(SQLiteConnection db, Assessment assessment)
        {
            db.Insert(assessment);
        }
        public static void updateAssessment(SQLiteConnection db, Assessment assessment)
        {
            db.Update(assessment);
        }
        public static void addInstructor(SQLiteConnection db, Instructor instructor)
        {
            db.Insert(instructor);
        }
        public static void updateInstructor(SQLiteConnection db, Instructor instructor)
        {
            db.Update(instructor);
        }
        public static void addNote(SQLiteConnection db, Note note)
        {
            db.Insert(note);
        }
        public static void updateNote(SQLiteConnection db, Note note)
        {
            db.Update(note);
        }

        public static List<Course> getCoursesReport(SQLiteConnection db)
        {
            List<Course> result = new List<Course>();

            var allcourses = db.Table<Course>().ToList();
            foreach (var course in allcourses)
            {
                if (course.termId != null)
                    course.Term = getTermbyTermID(db, course.termId);
                result.Add(course);
            }
            return result;
        }

        public static List<Course> getCoursesReportByTermID(SQLiteConnection db, int termID)
        {
            return db.Table<Course>().Where(x => x.termId == termID).ToList();

        }

        public static List<Term> getTermReport(SQLiteConnection db)
        {
            List<Term> result = new List<Term>();

            var allTerms = db.Table<Term>().ToList();
            foreach (var term in allTerms)
            {
                term.courses = getCoursesReportByTermID(db, term.termId);
                result.Add(term);
            }
            return result;
        }
        public static List<Instructor> getInstructorReport(SQLiteConnection db)
        {
            List<Instructor> result = new List<Instructor>();

            var allInstructor = db.Table<Instructor>().ToList();
            foreach (var instructor in allInstructor)
            {
                instructor.courses = getCoursesByInstructorID(db, instructor.instructorId);
                result.Add(instructor);
            }
            return result;
        }
        //public static List<Course> getCoursesByTerm(SQLiteConnection db, Term term)
        //{
        //    var allcourses = db.Table<Course>().ToList();
        //    return allcourses.Where(x => x.termId == term.termId).ToList();

        //}
        public static List<Course> getCoursesByInstructorID(SQLiteConnection db, int instructorID)
        {

            return db.Table<Course>().Where(x => x.instructorId == instructorID).ToList();

        }
        public static int getCoursesCount()
        {
            var db = new SQLiteConnection(MainPage.databasePath);
            var count = db.Table<Course>().Count();
            db.Close();
            return count;
        }
    }
}

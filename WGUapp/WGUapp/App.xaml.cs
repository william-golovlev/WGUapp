using Plugin.LocalNotification;
using Plugin.LocalNotification.EventArgs;

namespace WGUapp
{
    public partial class App : Application
    {
        public App()
        {
            try
            {
                InitializeComponent();

                MainPage = new NavigationPage(new AppShell());

                LocalNotificationCenter.Current.NotificationActionTapped += OnNotificationActionTapped;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
        private void OnNotificationActionTapped(NotificationActionEventArgs e)
        {
            if (e.IsDismissed)
            {
               //Dismiss notificaiton
                return;
            }
            if (e.IsTapped)
            {
                // Dismiss Notification
                return;
            }
           
        }
    }
}
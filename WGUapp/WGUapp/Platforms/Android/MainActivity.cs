using Android.App;
using Android.Content.PM;
using Android.OS;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;

namespace WGUapp
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            Window.SetStatusBarColor(Color.FromArgb("#003057").ToAndroid());
            Window.SetNavigationBarColor(Color.FromArgb("#003057").ToAndroid());
            base.OnCreate(savedInstanceState);
        }
    }
}
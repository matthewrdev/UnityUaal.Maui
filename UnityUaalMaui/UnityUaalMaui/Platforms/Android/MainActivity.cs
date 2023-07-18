using Android.App;
using Android.Content.PM;
using Android.OS;

namespace UnityUaalMaui;

[Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);
        Android.Util.Log.Info(GetType().Name, nameof(OnCreate) + "|" + GetHashCode());
    }

    protected override void OnPause()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnPause) + "|" + GetHashCode());
        base.OnPause();
    }

    protected override void OnResume()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnResume) + "|" + GetHashCode());
        base.OnResume();
    }

    protected override void OnStart()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnStart) + "|" + GetHashCode());
        base.OnStart();
    }

    protected override void OnStop()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnStop) + "|" + GetHashCode());
        base.OnStop();
    }

    protected override void OnDestroy()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnDestroy) + "|" + GetHashCode());
        base.OnDestroy();
    }
}


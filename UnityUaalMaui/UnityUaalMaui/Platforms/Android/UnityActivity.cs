
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Content.Res;
using Android.Graphics.Fonts;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.Navigation;
using Com.Unity3d.Player;
using Java.Lang;
using UnityUaalMaui.Unity;
using static Microsoft.Maui.ApplicationModel.Platform;

namespace UnityUaalMaui;

[Activity(Label = "UnityActivity",
          MainLauncher = false,
          ConfigurationChanges = ConfigChanges.Mcc | ConfigChanges.Mnc | ConfigChanges.Locale | ConfigChanges.Touchscreen | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.Navigation | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.FontScale | ConfigChanges.LayoutDirection | ConfigChanges.Density,
          //ScreenOrientation = ScreenOrientation.FullUser,
          ResizeableActivity = false,
          LaunchMode = LaunchMode.SingleTask)]
[MetaData(name: "unityplayer.UnityActivity", Value ="true")]
[MetaData(name: "notch_support", Value = "true")]
public class UnityActivity : Activity,
                             IUnityPlayerLifecycleEvents,
                             INativeUnityBridge,
                             IUnityPermissionRequestSupport
{
    private UnityPlayerForActivityOrService player;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        //RequestWindowFeature(WindowFeatures.NoTitle);

        base.OnCreate(savedInstanceState);
        player = new UnityPlayerForActivityOrService(this, this);

        this.SetContentView(player.FrameLayout);
        player.FrameLayout.RequestFocus();

        UnityUaalMaui.Unity.UnityBridge.RegisterNativeBridge(this);
    }


    // Configuration changes are used by Video playback logic in Unity
    public override void OnConfigurationChanged(Configuration newConfig)
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnConfigurationChanged) + "|" + GetHashCode() + "|" + newConfig);
        base.OnConfigurationChanged(newConfig);
        player.ConfigurationChanged(newConfig);
    }

    // Notify Unity of the focus change.
    public override void OnWindowFocusChanged(bool hasFocus)
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnWindowFocusChanged) + "|" + GetHashCode() + "|" + "hasFocus=" + hasFocus);
        base.OnWindowFocusChanged(hasFocus);
        player.WindowFocusChanged(hasFocus);
    }

    protected override void OnNewIntent(Android.Content.Intent intent)
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnNewIntent) + "|" + GetHashCode() + "|" + "Intent=" + intent.Action + "," + intent.Flags);
        Intent = intent;
        player.NewIntent(intent);
    }
    protected override void OnStop()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnStop) + "|" + GetHashCode() + "|");
        base.OnStop();

        if (!MultiWindowSupport.GetAllowResizableWindow(this))
        {
            return;
        }

        Android.Util.Log.Info(GetType().Name, "UnityPlayer.Pause");
        player.Pause();
    }

    protected override void OnPause()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnPause) + "|" + GetHashCode() + "|");
        base.OnPause();

        MultiWindowSupport.SaveMultiWindowMode(this);

        if (MultiWindowSupport.GetAllowResizableWindow(this))
        {
            return;
        }

        Android.Util.Log.Info(GetType().Name, "UnityPlayer.Pause");
        player.Pause();
    }

    protected override void OnStart()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnStart) + "|" + GetHashCode() + "|");
        base.OnStart();

        if (!MultiWindowSupport.GetAllowResizableWindow(this))
        {
            return;
        }

        Android.Util.Log.Info(GetType().Name, "UnityPlayer.Resume");
        player.Resume();
    }

    protected override void OnResume()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnResume) + "|" + GetHashCode() + "|");
        base.OnResume();

        if (MultiWindowSupport.GetAllowResizableWindow(this) && !MultiWindowSupport.IsMultiWindowModeChangedToTrue(this))
        {
            return;
        }

        Android.Util.Log.Info(GetType().Name, "UnityPlayer.Resume");
        player.Resume();
    }

    protected override void OnDestroy()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnDestroy) + "|" + GetHashCode() + "|");
        base.OnDestroy();

        UnityUaalMaui.Unity.UnityBridge.RegisterNativeBridge(null);
    }

    // TODO: Input events etc?

    public void OnUnityPlayerQuitted()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnUnityPlayerQuitted) + "|" + GetHashCode() + "|");
    }

    public void OnUnityPlayerUnloaded()
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnUnityPlayerUnloaded) + "|" + GetHashCode() + "|");
        MoveTaskToBack(true);
    }

    public void SendContent(string eventName, string eventContent)
    {
        var content = eventName + "|" + (eventContent ?? string.Empty);

        UnityPlayer.UnitySendMessage("Bridge", "ReceiveContent", content);
    }

    public void RequestPermissions(PermissionRequest request)
    {
        int requestCode = player.AddPermissionRequest(request);

        // request.getPermissionNames => Not accesible? => Reach in via reflection?

        //this.RequestPermissions(request. request.getPermissionNames(), requestCode);
    }

    public override bool DispatchKeyEvent(KeyEvent e)
    {
        Android.Util.Log.Info(GetType().Name, nameof(DispatchKeyEvent) + "|" + GetHashCode() + "|" + e.Action);
        if (e.Action == KeyEventActions.Multiple)
        {
            return player.InjectEvent(e);
        }

        return base.DispatchKeyEvent(e);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        player.PermissionResponse(this, requestCode, permissions, grantResults?.Select(gr => (int)gr)?.ToArray() ?? Array.Empty<int>());
    }

    // Pass any events not handled by (unfocused) views straight to UnityPlayer
    public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnKeyUp));
        return player.FrameLayout.OnKeyUp(keyCode, e);
    }

    public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnKeyDown));
        return player.FrameLayout.OnKeyDown(keyCode, e);
    }

    public override bool OnTouchEvent(MotionEvent e)
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnTouchEvent));
        return player.FrameLayout.OnTouchEvent(e);
    }

    public override bool OnGenericMotionEvent(MotionEvent e)
    {
        Android.Util.Log.Info(GetType().Name, nameof(OnGenericMotionEvent));
        return player.FrameLayout.OnGenericMotionEvent(e);
    }
}

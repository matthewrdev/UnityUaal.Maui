
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

namespace UnityUaalMaui;

[Activity(Label = "UnityActivity",
          MainLauncher = false,
          ConfigurationChanges = ConfigChanges.Mcc | ConfigChanges.Mnc | ConfigChanges.Locale | ConfigChanges.Touchscreen | ConfigChanges.Keyboard | ConfigChanges.KeyboardHidden | ConfigChanges.Navigation | ConfigChanges.Orientation | ConfigChanges.ScreenLayout | ConfigChanges.UiMode | ConfigChanges.ScreenSize | ConfigChanges.SmallestScreenSize | ConfigChanges.FontScale | ConfigChanges.LayoutDirection | ConfigChanges.Density,
          ScreenOrientation = ScreenOrientation.FullUser,
          ResizeableActivity = false)]
public class UnityActivity : Activity,
IUnityPlayerLifecycleEvents,
INativeUnityBridge,
                             IUnityPermissionRequestSupport
{
    private UnityPlayerForActivityOrService player;

    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        player = new UnityPlayerForActivityOrService(this, this);

        //UnityPlayer.UnitySendMessage()

        this.SetContentView(player.FrameLayout);

        UnityUaalMaui.Unity.UnityBridge.RegisterNativeBridge(this);
        Bridge.Instance.OnUnityContent("atest", "asdvaxd");
    }


    // Configuration changes are used by Video playback logic in Unity
    public override void OnConfigurationChanged(Configuration newConfig)
    {
        player.ConfigurationChanged(newConfig);
        base.OnConfigurationChanged(newConfig);
    }

    // Notify Unity of the focus change.
    public override void OnWindowFocusChanged(bool hasFocus)
    {
        player.WindowFocusChanged(hasFocus);
        base.OnWindowFocusChanged(hasFocus);
    }

    protected override void OnNewIntent(Intent intent)
    {
        base.OnNewIntent(intent);
        player.NewIntent(intent);
    }

    protected override void OnStop()
    {
        base.OnStop();
        player.Pause();
    }

    protected override void OnPause()
    {
        base.OnPause();
        player.Pause();
    }

    protected override void OnStart()
    {
        base.OnStart();
        player.Resume();
    }

    protected override void OnResume()
    {
        base.OnResume();
        player.Resume();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        UnityUaalMaui.Unity.UnityBridge.RegisterNativeBridge(null);
    }

    // TODO: Input events etc?

    public void OnUnityPlayerQuitted()
    {
    }

    public void OnUnityPlayerUnloaded()
    {
    }

    public void SendContent(string eventName, string eventContent)
    {
        var content = eventName + "|" + (eventContent ?? string.Empty);

        UnityPlayer.UnitySendMessage("Bridge", "ReceiveContent", content);
    }

    public void RequestPermissions(PermissionRequest request)
    {
        int requestCode = player.AddPermissionRequest(request);

        // request.getPermissionNames => Not accesible?

        //this.RequestPermissions(request. request.getPermissionNames(), requestCode);
    }

    public override bool DispatchKeyEvent(KeyEvent e)
    {
        if (e.Action == KeyEventActions.Multiple)
            return player.InjectEvent(e);

        return base.DispatchKeyEvent(e);
    }

    public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
    {
        player.PermissionResponse(this, requestCode, permissions, grantResults?.Select(gr => (int)gr)?.ToArray() ?? Array.Empty<int>());
    }

    // Pass any events not handled by (unfocused) views straight to UnityPlayer
    public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
    {
        return player.FrameLayout.OnKeyUp(keyCode, e);
    }

    public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
    {
        return player.FrameLayout.OnKeyDown(keyCode, e);
    }

    public override bool OnTouchEvent(MotionEvent e)
    {
        return player.FrameLayout.OnTouchEvent(e);
    }

    public override bool OnGenericMotionEvent(MotionEvent e)
    {
        return player.FrameLayout.OnGenericMotionEvent(e);
    }
}

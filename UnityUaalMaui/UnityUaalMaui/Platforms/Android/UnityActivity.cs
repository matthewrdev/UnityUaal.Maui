
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Com.Unity3d.Player;
using UnityUaalMaui.Unity;

namespace UnityUaalMaui;

[Activity(Label = "UnityActivity", MainLauncher = false)]
public class UnityActivity : Activity, IUnityPlayerLifecycleEvents, INativeUnityBridge
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
}

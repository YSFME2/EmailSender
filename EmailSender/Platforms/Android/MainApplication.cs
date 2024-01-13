using Android.App;
using Android.Runtime;
namespace ResumeSender
{
    [Application]
    [assembly: UsesPermission(Android.Manifest.Permission.ReadExternalStorage, MaxSdkVersion = 32)]
    [assembly: UsesPermission(Android.Manifest.Permission.ReadMediaAudio)]
    [assembly: UsesPermission(Android.Manifest.Permission.ReadMediaImages)]
    [assembly: UsesPermission(Android.Manifest.Permission.ReadMediaVideo)]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}

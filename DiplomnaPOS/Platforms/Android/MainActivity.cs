using Android.App;
using Android.Content.PM;
using Android.Content;
using AndroidX.Core.Content;

namespace DiplomnaPOS
{
    //[MetaData(NfcAdapter.ActionTechDiscovered, Resource = "@xml/nfc_tech_filter")]
    //[IntentFilter(new[] { NfcAdapter.ActionTechDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
    //[IntentFilter(new[] { NfcAdapter.ActionNdefDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]
    //[IntentFilter(new[] { NfcAdapter.ActionTagDiscovered }, Categories = new[] { Intent.CategoryDefault }, DataMimeType = "text/plain")]

    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
    }
}

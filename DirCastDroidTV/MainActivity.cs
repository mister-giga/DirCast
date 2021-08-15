using System;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using AndroidX.AppCompat.Widget;
using AndroidX.AppCompat.App;
using Google.Android.Material.FloatingActionButton;
using Google.Android.Material.Snackbar;
using Android.Widget;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Extractor;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2;
using Com.Google.Android.Exoplayer2.Util;
using Com.Google.Android.Exoplayer2.Source.Dash;
using Com.Google.Android.Exoplayer2.Source.Hls;
using Uri = Android.Net.Uri;
using AndroidX.Fragment.App;
using Fragment = AndroidX.Fragment.App.Fragment;
using DirCastDroidTV.Activities;
using AndroidX.Leanback.App;
using DirCastDroidTV.Fragments;
using System.Collections.Generic;
using DirCastDroidTV.Utils;
using System.Linq;
using Android.Content;
using DirCastDroidTV.Data;

namespace DirCastDroidTV
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true, ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]
    [IntentFilter(new[] { "android.intent.action.MAIN" }, Categories = new[] { "android.intent.category.LEANBACK_LAUNCHER" })]
    public class MainActivity : DCActivityBase
    {
        Stack<Fragment> navigation = new Stack<Fragment>();
        

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            EditText editText;

            if (savedInstanceState == null)
            {
                var url = UserSettings.WebServerUrl ?? "";
                if (!url.StartsWith("http://"))
                    url = "http://";
                new AndroidX.AppCompat.App.AlertDialog.Builder(this, Resource.Style.Theme_AppCompat)
                    .SetTitle("WebServer URL")
                    .SetCancelable(false)
                    .SetView(editText = new EditText(this) 
                    {
                        InputType = Android.Text.InputTypes.TextVariationUri,
                        Text = url
                    })
                    .SetPositiveButton("YES", OnYesClicked)
                    .Create()
                    .Show();               
            }

            void OnYesClicked(object sender, DialogClickEventArgs e)
            {
                UserSettings.WebServerUrl = editText.Text;
                Go(new DCBrowseFragment());
            }
        }

        

        public void Go(Fragment fragment, bool isFront = true)
        {
            if (isFront)
                navigation.Push(fragment);

            SupportFragmentManager
                .BeginTransaction()
                .Replace(Resource.Id.fragment_container_view, fragment, null)
                .Commit();
        }

        public override void OnBackPressed()
        {
            if(navigation.TryPeek(out var fragment))
            {
                if (fragment is IGoBackable goBackable && goBackable.GoBack())
                    return;

                if(navigation.Count > 1)
                {
                    if (navigation.Pop() is IDestroyable destroyable)
                        destroyable.Destroy();
                    Go(navigation.Peek(), false);
                    return;
                }
            }

            base.OnBackPressed();
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}

    public static class Ext
    {
        public static void Go(this Fragment fragment, Fragment to)
        {
            ((MainActivity)fragment.Activity).Go(to);
        }
    }
}

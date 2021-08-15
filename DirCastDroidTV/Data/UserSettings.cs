using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Xamarin.Essentials.Preferences;

namespace DirCastDroidTV.Data
{
    public  static class UserSettings
    {
        public static string WebServerUrl
        {
            get => Get(nameof(WebServerUrl), "");
            set => Set(nameof(WebServerUrl), value ?? "");
        }
    }
}
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DirCastCommon.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DirCastDroidTV.Data
{
    //192.168.1.66:555
    public static class ApiClient
    {
        public static async Task<DirInfo> GetDirInfo(string path)
        {
            var res = await new HttpClient().GetStringAsync($"{UserSettings.WebServerUrl}/api/dirs/explore?path=" + path);
            var data = JsonConvert.DeserializeObject<DirInfo>(res);
            return data;
        }

        public static string GetMediaUrl(DirFileInfo dirFileInfo) => $"{UserSettings.WebServerUrl}/api/dirs/view?path={dirFileInfo.Path}&name={dirFileInfo.Name}";
    }
}
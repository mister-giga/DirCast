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

namespace DirCastDroidTV
{
    public static class IEnumerableExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> src, Action<T> action)
        {
            if(src != null)
            {
                foreach (var item in src)
                {
                    action(item);
                }
            }
        }
    }
}
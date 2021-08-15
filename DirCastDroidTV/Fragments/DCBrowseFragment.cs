using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Nfc;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content;
using AndroidX.Leanback.App;
using AndroidX.Leanback.Widget;
using Com.Google.Android.Exoplayer2.Util;
using DirCastCommon.Models;
using DirCastDroidTV.Data;
using DirCastDroidTV.Utils;
using Java.Nio.Channels;
using Java.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static Android.Content.ClipData;

namespace DirCastDroidTV.Fragments
{
    public class DCBrowseFragment : BrowseSupportFragment, IGoBackable
    {
        Stack<(string path, int position)> paths = new(new[] { (".", 0) });

        public async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);


            Title = "DirCast";
            HeadersState = HeadersDisabled;

            await Load();


            ItemViewClicked -= DCBrowseFragment_ItemViewClicked;
            ItemViewClicked += DCBrowseFragment_ItemViewClicked;
        }

        private async void DCBrowseFragment_ItemViewClicked(object sender, ItemViewClickedEventArgs e)
        {
            if(e.Item is DCObjectHolder obj)
            {
                if (obj.IsDirectory)
                {
                    if (obj.IsEmptyDirectory)
                    {
                        Toast.MakeText(Context, $"This directory is empty", ToastLength.Short).Show();
                    }
                    else
                    {
                        paths.Push((obj.DirInfo.Path, SelectedPosition));
                        await Load();
                    }
                }
                else if (obj.IsMovie)
                {
                    this.Go(new DCPlayerFragment() { FileInfo = obj.DirFileInfo });
                }
                else
                {
                    Toast.MakeText(Context, $"Viewing {obj.DirFileInfo.GetExtension()} files is not yet supported", ToastLength.Short).Show();
                }
            }
        }

        async Task Load(int position = 0)
        {
            var path = paths.Peek();
            var data = await ApiClient.GetDirInfo(path.path);

            var rowsAdapter = new ArrayObjectAdapter(new ListRowPresenter());

            var cardPresenter = new CardPresenter();

            AddRow(data.GetName(), null, data.Files);

            foreach (var subDir in data.SubDirs)
                AddRow(subDir.GetName(), subDir.SubDirs, subDir.Files);

            Adapter = rowsAdapter;

            SetSelectedPosition(position, true);

            void AddRow(string title, IEnumerable<DirInfo> dirs, IEnumerable<DirFileInfo> files)
            {
                var header = new HeaderItem(title);

                var listRowAdapter = new ArrayObjectAdapter(cardPresenter);

                dirs?.ForEach(dir => listRowAdapter.Add(new DCObjectHolder { DirInfo = dir }));
                files?.ForEach(file => listRowAdapter.Add(new DCObjectHolder { DirFileInfo = file }));

                if (listRowAdapter.Size() == 0)
                    listRowAdapter.Add(new DCObjectHolder { });

                rowsAdapter.Add(new ListRow(header, listRowAdapter));
            }
        }

        public bool GoBack()
        {
            if(paths.Count > 1)
            {
                var item = paths.Pop();
                _ = Load(item.position);
                return true;
            }
            return false;
        }
    }

    class DCObjectHolder : Java.Lang.Object
    {
        public DirInfo DirInfo { get; init; }
        public DirFileInfo DirFileInfo { get; init; }

        public override string ToString()
        {
            if (DirInfo != null)
                return DirInfo.GetName();
            if (DirFileInfo != null)
                return DirFileInfo.Name;
            return "Directory is empty";
        }

        public string GetInfo()
        {
            if (DirInfo != null)
                return "Directory";
            if(DirFileInfo != null)
            {
                return System.IO.Path.GetExtension(DirFileInfo.Name);
            }
            return "";
        }

        public bool IsEmptyDirectory => DirInfo == null && DirFileInfo == null;
        public bool IsDirectory => IsEmptyDirectory || DirInfo != null;
        public bool IsMovie => HasExtension(".mp4");

        private bool HasExtension(params string[] extensions) => extensions.Any(extension => DirFileInfo?.GetExtension()?.Equals(extension, StringComparison.InvariantCultureIgnoreCase) == true);
    }

    class CardPresenter : Presenter
    {
        private Drawable iconFolder;
        private Drawable iconUnknown;
        private Drawable iconMovie;

        public override void OnBindViewHolder(ViewHolder viewHolder, Java.Lang.Object item)
        {
            var cardView = viewHolder.View as ImageCardView;

            if(item is DCObjectHolder obj)
            {
                cardView.TitleText = obj.ToString();
                cardView.ContentText = obj.GetInfo();

                Drawable drawable;
                if (obj.IsDirectory)
                    drawable = iconFolder;
                else if (obj.IsMovie)
                    drawable = iconMovie;
                else
                    drawable = iconUnknown;

                cardView.SetMainImage(drawable, true);
            }

            cardView.SetMainImageDimensions(313, 176);
        }

        public override ViewHolder OnCreateViewHolder(ViewGroup parent)
        {
            iconFolder = ContextCompat.GetDrawable(parent.Context, Resource.Drawable.ic_folder);
            iconUnknown = ContextCompat.GetDrawable(parent.Context, Resource.Drawable.ic_unknown);
            iconMovie = ContextCompat.GetDrawable(parent.Context, Resource.Drawable.ic_movie);

            var cardView = new ImageCardView(parent.Context)
            {
                Focusable = true,
                FocusableInTouchMode = true,
            };

            cardView.MainImageView.SetScaleType(ImageView.ScaleType.CenterInside);

            return new ViewHolder(cardView);
        }

        public override void OnUnbindViewHolder(ViewHolder viewHolder)
        {
        }
    }
}
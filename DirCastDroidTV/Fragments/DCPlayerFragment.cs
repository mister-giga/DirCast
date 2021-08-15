using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Google.Android.Exoplayer2.Source;
using Com.Google.Android.Exoplayer2.Trackselection;
using Com.Google.Android.Exoplayer2.UI;
using Com.Google.Android.Exoplayer2.Upstream;
using Com.Google.Android.Exoplayer2;
using DirCastCommon.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uri = Android.Net.Uri;
using Com.Google.Android.Exoplayer2.Util;
using Android.Util;
using DirCastDroidTV.Utils;
using DirCastDroidTV.Data;

namespace DirCastDroidTV.Fragments
{
    class DCPlayerFragment : AndroidX.Fragment.App.Fragment, IDestroyable, PlayerControlView.IVisibilityListener
    {
        private PlayerView playerView;
        SimpleExoPlayer player;

        public DirFileInfo FileInfo { get; init; }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.content_main, container, false);

            return view;
        }


        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);

           
        }

        public override void OnStart()
        {
            base.OnStart();

            playerView = (PlayerView)View.FindViewById(Resource.Id.player_view);
            playerView.SetControllerVisibilityListener(this);
            //playerView.SetErrorMessageProvider(new PlayerErrorMessageProvider());
            playerView.Focusable = true;
            playerView.FocusableInTouchMode = true;
            playerView.RequestFocus();


            DefaultTrackSelector trackSelector = new DefaultTrackSelector(Context);
            trackSelector.SetParameters(trackSelector.BuildUponParameters().SetMaxVideoSizeSd());
            DefaultRenderersFactory renderersFactory = new DefaultRenderersFactory(Context);
            var hls = Uri.Parse(ApiClient.GetMediaUrl(FileInfo));
            IMediaSource mediaSource = BuildMediaSource(hls);

            player = new SimpleExoPlayer.Builder(Context, renderersFactory).SetTrackSelector(trackSelector).Build();
            playerView.Player = player;
            player.PlayWhenReady = true;
            player.Prepare(mediaSource, false, false);


            IMediaSource BuildMediaSource(Uri uri)
            {
                DefaultDataSourceFactory dataSourceFactory = new DefaultDataSourceFactory(Context, "ExoplayerTest");
                return new ProgressiveMediaSource.Factory(dataSourceFactory).CreateMediaSource(uri);
            }
        }

        public void Destroy()
        {
            playerView.Player = null;
            player?.Stop(true);
            player?.Dispose();
        }

        public void OnVisibilityChange(int visibility)
        {

        }
    }
}
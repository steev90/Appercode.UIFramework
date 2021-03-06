using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Appercode.UI.Device;

namespace Appercode.UI.Controls.NativeControl.Wrapers
{
    public class WrapedVideoView : VideoView, ITapableView, IJavaFinalizable, ISurfaceHolderCallback, MediaPlayer.IOnPreparedListener, MediaPlayer.IOnCompletionListener, View.IOnClickListener
    {
        private NativeMediaPlayer player;

        private ISurfaceHolder holder;

        private double bufferingProgress = 0.0;

        public WrapedVideoView(Context context)
            : base(context)
        {
            this.SetOnClickListener(this);
        }

        public event EventHandler NativeTap;
        public event EventHandler JavaFinalized;
        public event EventHandler VideoSizeChanged;
        public new event EventHandler Completion;
        public new event EventHandler Prepared;
        public event EventHandler BufferingProgressUpdate;

        public override bool IsPlaying
        {
            get
            {
                return this.player.IsPlaying;
            }
        }

        public int VideoHeight
        {
            get
            {
                return (int)ScreenProperties.ConvertPixelsToDPI(this.player.VideoHeight);
            }
        }

        public int VideoWidth
        {
            get
            {
                return (int)ScreenProperties.ConvertPixelsToDPI(this.player.VideoWidth);
            }
        }

        public override int Duration
        {
            get
            {
                return this.player.Duration;
            }
        }

        public int Position
        {
            get
            {
                return this.player.CurrentPosition;
            }
        }

        public double BufferingProgress
        {
            get
            {
                return this.bufferingProgress;
            }
        }

        public void InitMediaPlayer()
        {
            this.holder = this.Holder;
            this.holder.SetType(SurfaceType.PushBuffers);
            this.holder.AddCallback(this);

            this.player = new NativeMediaPlayer();

            this.player.VideoSizeChanged -= player_VideoSizeChanged;
            this.player.VideoSizeChanged += player_VideoSizeChanged;

            this.player.BufferingUpdate -= player_BufferingUpdate;
            this.player.BufferingUpdate += player_BufferingUpdate;

            this.player.SetOnCompletionListener(this);
            this.player.SetOnPreparedListener(this);
        }

        public void SetDataSource(string path)
        {
            if(!string.IsNullOrWhiteSpace(path))
            {
                this.player.SetDataSource(path);
            }
        }

        public void SetDataSource(Java.IO.FileDescriptor fd, long offset, long length)
        {
            if (fd != null)
            {
                this.player.Reset();
                this.player.SetDataSource(fd, offset, length);
            }
        }

        public void Prepare()
        {
            this.player.Prepare();
        }

        public void Play()
        {
            this.player.Start();
        }

        public override void Pause()
        {
            if (this.player.IsPlaying)
            {
                this.player.Pause();
            }
        }

        public void Stop()
        {
            this.player.SeekTo(this.player.Duration);
        }

        public override void SeekTo(int msec)
        {
            this.player.SeekTo(msec);
        }

        public void WrapedNativeRaiseTap()
        {
            if (this.NativeTap != null)
            {
                this.NativeTap(this, null);
            }
        }

        protected override void JavaFinalize()
        {
            if (this.JavaFinalized != null)
            {
                this.JavaFinalized(null, null);
            }
            base.JavaFinalize();
        }

        public void OnPrepared(MediaPlayer mp)
        {
            if (this.Prepared != null)
            {
                this.Prepared(this, new EventArgs());
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, Android.Graphics.Format format, int width, int height)
        {
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            this.player.SetDisplay(holder);
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            
        }

        private void player_BufferingUpdate(object sender, MediaPlayer.BufferingUpdateEventArgs e)
        {
            this.bufferingProgress = e.Percent;

            if (this.BufferingProgressUpdate != null)
            {
                this.BufferingProgressUpdate(this, new EventArgs());
            }
        }

        private void player_VideoSizeChanged(object sender, MediaPlayer.VideoSizeChangedEventArgs e)
        {
            if (this.VideoSizeChanged != null)
            {
                this.VideoSizeChanged(this, e);
            }
        }

        public void OnCompletion(MediaPlayer mp)
        {
            if (this.Completion != null)
            {
                this.Completion(this, new EventArgs());
            }
        }

        public void OnClick(View v)
        {
            WrapedNativeRaiseTap();
        }
    }
}
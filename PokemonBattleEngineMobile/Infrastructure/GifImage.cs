using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using Xamarin.Forms;

namespace Kermalis.PokemonBattleEngineMobile.Infrastructure
{
    public class GifImage : SKCanvasView
    {
        private readonly object lockObj = new object();
        private string resource;
        private SKBitmap[] bitmaps;
        private int[] bitmapTimes;
        private int totalDuration;

        private readonly Stopwatch stopwatch = new Stopwatch();
        private bool isAnimating;

        private int currentFrame;

        public void SetGifResource(string resource)
        {
            if (this.resource != resource)
            {
                this.resource = resource;
                lock (lockObj)
                {
                    isAnimating = false;
                    stopwatch.Stop();
                    using (Stream stream = Utils.GetResourceStream(resource))
                    using (var skStream = new SKManagedStream(stream))
                    using (var codec = SKCodec.Create(skStream))
                    {
                        WidthRequest = codec.Info.Width;
                        HeightRequest = codec.Info.Height;
                        // So here's the story about how great Xamarin is:
                        // If you do nothing, then the old bitmap and old size are used for a few frames, because the size requests set above are only requests, and therefore not going to actually tell the view to update.
                        // If you call InvalidateMeasure() (any of them), InvalidateSurface(), or ((IVisualElementController)this).NativeSizeChanged(), it is the same as doing nothing.
                        // If you call ForceLayout() on the first parent that is a Layout, the width and height are stuck to the previous gif used, therefore ignoring the requests set above (even when you change the device orientation).
                        // So, you are forced to use Layout() as seen below, because it directly sets the X,Y,Width,Height properties which are otherwise private set in VisualElement.
                        // More retardation: if you use double.MaxValue, double.MinValue, double.PositiveInfinity, double.NegativeInfinity, or -1.0, then the new bitmap is shown at some random location (not the screen's (0,0) or the parent's (0,0) or anything that would make sense, no, just random, and not for a few frames like the other solutions, literally one frame), but it is the correct size.
                        // So, I have to use some meaningless large number that no screen will be for it to work, but it is sometimes pretty slow.
                        // In the end, due to the fact that there is no way to do "FORCE SIZE AND POSITION RECALCULATION RIGHT NOW WITHOUT WAITING A FRAME" in this unfriendly framework, we have to settle for this nonsense that took me 2 days to work out, when they could instead include 1 function and it would take 5 seconds for a programmer to find.
                        Layout(new Rectangle(99999, 99999, codec.Info.Width, codec.Info.Height));

                        int frameCount = codec.FrameCount;
                        bitmaps = new SKBitmap[frameCount];
                        bitmapTimes = new int[frameCount];
                        totalDuration = 0;
                        for (int frame = 0; frame < frameCount; frame++)
                        {
                            int dur = codec.FrameInfo[frame].Duration;
                            totalDuration += dur;
                            bitmapTimes[frame] = dur + (frame == 0 ? 0 : bitmapTimes[frame - 1]);

                            var imageInfo = new SKImageInfo(codec.Info.Width, codec.Info.Height);
                            var bmap = new SKBitmap(imageInfo);
                            bitmaps[frame] = bmap;
                            codec.GetPixels(imageInfo, bmap.GetPixels(), new SKCodecOptions(frame));
                        }
                        // Single-frame gifs (like the Substitute gif) need this extra if so they don't divide by 0.
                        // TODO: codec has a RepititionCount property which would be 0 for Substitute and -1 for everything else that loops
                        if (frameCount == 1)
                        {
                            bitmapTimes[0] = totalDuration = 1;
                        }
                    }
                }
                SetAnimating();
            }
        }
        private void SetAnimating()
        {
            lock (lockObj)
            {
                if (bitmaps != null && IsVisible)
                {
                    if (!isAnimating)
                    {
                        currentFrame = 0;
                        isAnimating = true;
                        InvalidateSurface();
                        stopwatch.Start();
                        Device.StartTimer(TimeSpan.FromMilliseconds(1000 / 60.0), OnTimerTick);
                    }
                }
                else
                {
                    if (isAnimating)
                    {
                        isAnimating = false;
                        stopwatch.Stop();
                    }
                }
            }
        }

        protected override void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName == nameof(IsVisible))
            {
                SetAnimating();
            }
        }

        private bool OnTimerTick()
        {
            lock (lockObj)
            {
                int msec = (int)(stopwatch.ElapsedMilliseconds % totalDuration);
                int frame;
                for (frame = 0; frame < bitmapTimes.Length; frame++)
                {
                    if (msec < bitmapTimes[frame])
                    {
                        break;
                    }
                }
                if (currentFrame != frame)
                {
                    currentFrame = frame;
                    InvalidateSurface();
                }
                return isAnimating;
            }
        }
        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;
            canvas.Clear(SKColors.Transparent);
            if (isAnimating)
            {
                canvas.DrawBitmap(bitmaps[currentFrame], e.Info.Rect);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Touchless.Vision.Camera;
using Touchless.Vision.Contracts;

namespace SimpleWebCam
{
    public class WebCam
    {
        public Camera Camera => CameraService.DefaultCamera;
        public List<Bitmap> BitMaps { get; private set; } = new List<Bitmap>();

        private CameraFrameSource _frameSource;
        private int n = 0;
        public async Task<List<Bitmap>> GetNBitMaps(int n)
        {
            this._frameSource = new CameraFrameSource(this.Camera);
            _frameSource.Camera.CaptureWidth = 640;
            _frameSource.Camera.CaptureHeight = 480;
            _frameSource.Camera.Fps = 50;
            this._frameSource.NewFrame += _frameSource_NewFrame;

            this._frameSource.StartFrameCapture();

            while (n > BitMaps.Count)
                System.Threading.Thread.Sleep(100);

            this._frameSource.NewFrame -= _frameSource_NewFrame;

            Camera.Dispose();

            return BitMaps;
        }

        private void _frameSource_NewFrame(IFrameSource frameSource, Frame frame, double fps)
        {
            BitMaps.Add(frame.Image);
        }
    }
}

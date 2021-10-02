using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace LudumDare49
{
    public class Camera
    {
        public Matrix4 Projection { get; private set; }
        
        public Matrix4 View { get; private set; }

        private Vector3 _viewPos;
        public Vector3 ViewPos
        {
            get
            {
                return _viewPos;
            }
            set
            {
                _viewPos = value;
                View = Matrix4.LookAt(value, Vector3.UnitY, Vector3.UnitY);
            }
        }

        public Camera(Window window)
        {
            SetProjection(window.ClientSize.X, window.ClientSize.Y);
            window.Resize += OnResize;
            ViewPos = new Vector3(0, 5f, -5f);
        }

        private void OnResize(ResizeEventArgs args)
        {
            SetProjection(args.Width, args.Height);
        }

        private void SetProjection(float width, float height)
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(60),
                width / height, 0.001f, 1000f);
        }
    }
}
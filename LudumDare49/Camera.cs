using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace LudumDare49
{
    public static class Camera
    {
        public static Matrix4 Projection { get; private set; }
        
        public static Matrix4 View { get; }

        static Camera()
        {
            SetProjection(Window.Instance.ClientSize.X, Window.Instance.ClientSize.Y);
            Window.Instance.Resize += OnResize;
            View = Matrix4.LookAt(new Vector3(0, 5, -5f), Vector3.Zero, Vector3.UnitY);
        }

        private static void OnResize(ResizeEventArgs args)
        {
            SetProjection(args.Width, args.Height);
        }

        private static void SetProjection(float width, float height)
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45),
                width / height, 0.001f, 1000f);
        }
    }
}
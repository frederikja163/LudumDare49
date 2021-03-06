using OpenTK.Mathematics;

namespace LudumDare49
{
    public sealed class Transform
    {
        private Vector3 _position;
        public Vector3 Position
        {
            get => _position;
            set
            {
                _position = value;
                Update();
            }
        }

        private Vector3 _scale = Vector3.One;
        public Vector3 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                Update();
            }
        }

        public Vector3 EulerAngles
        {
            get => _rotation.ToEulerAngles();
            set
            {
                Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, value.Y) *
                           Quaternion.FromAxisAngle(Vector3.UnitZ, value.Z) *
                           Quaternion.FromAxisAngle(Vector3.UnitX, value.X);
            }
        }

        private Quaternion _rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get => _rotation;
            set
            {
                _rotation = value;
                Update();
            }
        }
        
        public Matrix4 Matrix { get; private set; }
        
        public Vector3 Forward => Vector3.Transform(Vector3.UnitZ, Rotation).Normalized();
        public Vector3 Right => Vector3.Transform(Vector3.UnitX, Rotation).Normalized();
        public Vector3 Up => Vector3.Transform(Vector3.UnitY, Rotation).Normalized();
        
        public Transform()
        {
            Update();
        }

        private void Update()
        {
            Matrix = Matrix4.CreateFromQuaternion(_rotation) *
                     Matrix4.CreateScale(_scale) *
                     Matrix4.CreateTranslation(_position);
        }
    }
}
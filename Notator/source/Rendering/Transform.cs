using System.Numerics;

namespace Notator.source.Rendering
{
    public class Transform
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public float Scale { get; set; } = 1.0f;

        public Transform() { }

        public Matrix4x4 ModelMatrix =>
            Matrix4x4.Identity *
            Matrix4x4.CreateFromQuaternion(Rotation) *
            Matrix4x4.CreateScale(Scale) *
            Matrix4x4.CreateTranslation(Position);
    }
}
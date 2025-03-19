using Silk.NET.OpenGL;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace Notator.source.Rendering
{
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private GL OpenGL { init; get; }
        private BufferTargetARB Type { init; get; }
        private uint ID { init; get; }
        public uint Length { set; get; }

        public BufferObject(GL openGL, ReadOnlySpan<TDataType> data, BufferTargetARB type)
        {
            OpenGL = openGL;
            Type = type;
            ID = OpenGL.GenBuffer();

            SetBuffer(data);
        }

        public void Bind()
        {
            OpenGL.BindBuffer(Type, ID);
        }

        public void Unbind()
        {
            OpenGL.BindBuffer(Type, 0);
        }

        public void Dispose()
        {
            OpenGL.DeleteBuffer(ID);
        }

        internal void SetBuffer(ReadOnlySpan<TDataType> data)
        {
            Bind();
            nuint size = (nuint)(data.Length * Marshal.SizeOf<TDataType>());
            OpenGL.BufferData(Type, size, data, BufferUsageARB.StaticDraw);
            Length = (uint)data.Length;
            Unbind();
        }
    }
}
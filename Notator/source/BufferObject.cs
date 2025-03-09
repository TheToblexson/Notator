using Silk.NET.OpenGL;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;

namespace Notator
{
    public class BufferObject<TDataType> : IDisposable
        where TDataType : unmanaged
    {
        private GL OpenGL { init; get; }
        private BufferTargetARB Type { init; get; }
        private uint ID { init; get; } 

        public BufferObject(GL openGL, ReadOnlySpan<TDataType> data, BufferTargetARB type)
        {
            OpenGL = openGL;
            Type = type;
            ID = OpenGL.GenBuffer();

            Bind();

            nuint size = (nuint)(data.Length * Marshal.SizeOf<TDataType>());
            OpenGL.BufferData(Type, size, data, BufferUsageARB.StaticDraw);
        }

        public void Bind()
        {
            OpenGL.BindBuffer(Type, ID);
        }

        public void Dispose()
        {
            OpenGL.DeleteBuffer(ID);
        }
    }
}
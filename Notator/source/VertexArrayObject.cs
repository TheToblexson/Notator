using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace Notator
{
    public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private GL OpenGL { init; get; }
        private uint ID { init; get; }

        public VertexArrayObject(GL openGL, BufferObject<TVertexType> vertexBuffer, BufferObject<TIndexType> indexBuffer)
        {
            OpenGL = openGL;
            ID = OpenGL.GenVertexArray();

            Bind();
            vertexBuffer.Bind();       
            indexBuffer.Bind();
        }

        public void VertexAttribute(uint location, int count, VertexAttribPointerType type, uint vertexSize, int offset)
        {
            int sizeOfType = Marshal.SizeOf<TVertexType>();
            OpenGL.VertexAttribPointer(location, count, type, false, (uint)(vertexSize * sizeOfType), offset * sizeOfType);
            OpenGL.EnableVertexAttribArray(location);
        }

        public void Bind()
        {
            OpenGL.BindVertexArray(ID);
        }

        public void Dispose()
        {
            OpenGL.DeleteVertexArray(ID);
        }
    }
}
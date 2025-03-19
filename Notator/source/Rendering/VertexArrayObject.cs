using Silk.NET.OpenGL;
using System.Runtime.InteropServices;

namespace Notator.source.Rendering
{
    public class VertexArrayObject<TVertexType, TIndexType> : IDisposable
        where TVertexType : unmanaged
        where TIndexType : unmanaged
    {
        private GL OpenGL { init; get; }
        public BufferObject<TVertexType> VertexBuffer { get; set; }
        public BufferObject<TIndexType> IndexBuffer { get; set; }
        private uint ID { init; get; }

        public VertexArrayObject(GL openGL, BufferObject<TVertexType> vertexBuffer, BufferObject<TIndexType> indexBuffer)
        {
            OpenGL = openGL;
            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
            ID = OpenGL.GenVertexArray();
        }

        public void VertexAttribute(uint location, int count, VertexAttribPointerType type, uint vertexSize, int offset)
        {
            BindAll();
            int sizeOfType = Marshal.SizeOf<TVertexType>();
            OpenGL.VertexAttribPointer(location, count, type, false, (uint)(vertexSize * sizeOfType), offset * sizeOfType);
            OpenGL.EnableVertexAttribArray(location);
            UnbindAll();
        }

        public void Bind()
        {
            OpenGL.BindVertexArray(ID);
        }

        public void Unbind()
        {
            OpenGL.BindVertexArray(0);
        }

        public void BindAll()
        {
            Bind();
            VertexBuffer.Bind();
            IndexBuffer.Bind();
        }

        public void UnbindAll()
        {
            Unbind();
            VertexBuffer.Unbind();
            IndexBuffer.Unbind();
        }

        public void Dispose()
        {
            OpenGL.DeleteVertexArray(ID);
        }
    }
}
using Silk.NET.OpenGL;
using System.Drawing;

namespace Notator.Rendering
{
    public class IndexBuffer
    {
        #region Private Properties

        /// <summary>
        /// A reference to the OpenGL instance.
        /// </summary>
        private GL OpenGL { get; init; }

        /// <summary>
        /// The id of the index array.
        /// </summary>
        private uint Id { get; init; }

        /// <summary>
        /// If the index buffer is bound.
        /// </summary>
        private bool IsBound { get; set; } = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates and provides access to a index buffer.
        /// </summary>
        /// <param name="openGL">The OpenGL instance.</param>
        /// <param name="size">The desired size of the buffer (in bytes).</param>
        public unsafe IndexBuffer(GL openGL, uint size)
        {
            OpenGL = openGL;

            // Create the index buffer
            Id = OpenGL.GenBuffer();
            // Bind the index buffer
            Bind();
            // Create dynamic index buffer
            OpenGL.BufferData(BufferTargetARB.ElementArrayBuffer, size, null, BufferUsageARB.DynamicDraw);
            Unbind();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Bind the index buffer.
        /// </summary>
        public void Bind()
        {
            if (!IsBound)
            {
                OpenGL.BindBuffer(BufferTargetARB.ElementArrayBuffer, Id);
                IsBound = true;
            }
        }

        /// <summary>
        /// Unbind the index buffer.
        /// </summary>
        public void Unbind()
        {
            if (IsBound)
            {
                OpenGL.BindBuffer(BufferTargetARB.ElementArrayBuffer, 0);
                IsBound = false;
            }
        }

        /// <summary>
        /// Delete the index buffer from OpenGL.
        /// </summary>
        internal void Delete()
        {
            Unbind();
            OpenGL.DeleteBuffer(Id);
        }

        internal void BufferSubData(uint[] indices)
        {
            Bind();
            OpenGL.BufferSubData<uint>(BufferTargetARB.ElementArrayBuffer, 0, indices);
            Unbind();
        }

        #endregion
    }
}
using Silk.NET.OpenGL;

namespace Notator.Rendering
{
    public class VertexBuffer
    {
        #region Private Properties

        /// <summary>
        /// A reference to the OpenGL instance.
        /// </summary>
        private GL OpenGL { get; init; }

        /// <summary>
        /// The id of the vertex buffer.
        /// </summary>
        private uint Id { get; init; }

        /// <summary>
        /// If the vertex buffer is bound.
        /// </summary>
        private bool IsBound { get; set; } = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates and provides access to a vertex buffer.
        /// </summary>
        /// <param name="openGL">The OpenGL instance.</param>
        /// <param name="size">The desired size of the buffer (in bytes).</param>
        public unsafe VertexBuffer(GL openGL, uint size)
        {
            OpenGL = openGL;

            // Create the vertex buffer
            Id = OpenGL.GenBuffer();
            // Bind the vertex buffer
            Bind();
            // Create dynamic vertex buffer
            OpenGL.BufferData(BufferTargetARB.ArrayBuffer, size, null, BufferUsageARB.DynamicDraw);
            Unbind();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Bind the vertex buffer.
        /// </summary>
        public void Bind()
        {
            if (!IsBound)
            {
                OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, Id);
                IsBound = true;
            }
        }

        /// <summary>
        /// Unbind the vertex buffer.
        /// </summary>
        public void Unbind()
        {
            if (IsBound)
            {
                OpenGL.BindBuffer(BufferTargetARB.ArrayBuffer, 0);
                IsBound = false;
            }
        }

        /// <summary>
        /// Delete the vertex buffer from OpenGL.
        /// </summary>
        public void Delete()
        {
            Unbind();
            OpenGL.DeleteBuffer(Id);
        }

        public void BufferSubData(float[] vertices)
        {
            Bind();
            OpenGL.BufferSubData<float>(BufferTargetARB.ArrayBuffer, 0, vertices);
            Unbind();
        }

        #endregion
    }
}
using Silk.NET.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notator.Rendering
{
    public class VertexArray
    {
        #region Private Properties

        /// <summary>
        /// A reference to the OpenGL instance.
        /// </summary>
        private GL OpenGL { get; init; }

        /// <summary>
        /// The id of the vertex array.
        /// </summary>
        private uint Id { get; init; }

        /// <summary>
        /// If the vertex array is bound.
        /// </summary>
        private bool IsBound { get; set; } = false;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates and provides access to a vertex array.
        /// </summary>
        /// <param name="openGL">The OpenGL instance.</param>
        public VertexArray(GL openGL)
        {
            OpenGL = openGL;

            // Create the vertex array
            Id = OpenGL.GenVertexArray();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Bind the vertex array.
        /// </summary>
        public void Bind()
        {
            if (!IsBound)
            {
                OpenGL.BindVertexArray(Id);
                IsBound = true;
            }
        }

        /// <summary>
        /// Unbind the vertex array.
        /// </summary>
        public void Unbind()
        {
            if (IsBound)
            {
                OpenGL.BindVertexArray(0);
                IsBound = false;
            }
        }

        /// <summary>
        /// Delete the vertex array from OpenGL.
        /// </summary>
        public void Delete()
        {
            Unbind();
            OpenGL.DeleteVertexArray(Id);
        }

        #endregion
    }
}

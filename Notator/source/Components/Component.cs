using Notator.Rendering.Shapes;
using Silk.NET.Maths;
using System.Drawing;

namespace Notator.source.Components
{
    public abstract class Component
    {
        #region Fields

        #endregion

        #region Properties

        /// <summary>
        /// The internal name of the component
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The position of the bottom left corner of the component.
        /// </summary>
        protected Vector3D<float> Position { get; }

        /// <summary>
        /// The size of the component
        /// </summary>
        protected Vector2D<float> Size { get; }

        /// <summary>
        /// The color of the component.
        /// </summary>
        protected RenderColor Color { get; }

        /// <summary>
        /// The render quad for the component.
        /// </summary>
        public RenderQuad Quad { get; }

        #endregion

        #region Constructors

        /// <summary>
        /// Create a UI component.
        /// </summary>
        /// <param name="name">The internal name of the component.</param>
        /// <param name="posX">The x coordinate of the bottom right corner.</param>
        /// <param name="posY">The y coordinate of the bottom right corner.</param>
        /// <param name="posZ">The z coordinate of the bottom right corner.</param>
        /// <param name="width">The width of the panel.</param>
        /// <param name="height">The height of the panel.</param>
        /// <param name="color">The color of the panel.</param>
        public Component(string name, float posX, float posY, float posZ, float width, float height, RenderColor color)
        {
            Name = name;
            Position = new Vector3D<float>(posX, posY, posZ);
            Size = new Vector2D<float>(width, height);
            Color = color;
            Quad = new RenderQuad(posX, posY, posZ, width, height, color);
        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}

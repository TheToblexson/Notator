using Notator.Rendering.Shapes;
using System.Drawing;

namespace Notator.source.Components
{
    public class Panel : Component
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Create a basic panel component.
        /// </summary>
        /// <param name="name">The internal name of the panel.</param>
        /// <param name="posX">The x coordinate of the bottom left corner.</param>
        /// <param name="posY">The y coordinate of the bottom left corner.</param>
        /// <param name="posZ">The z coordinate of the bottom left corner.</param>
        /// <param name="width">The width of the panel.</param>
        /// <param name="height">The height of the panel.</param>
        /// <param name="color">The color of the panel.</param>
        public Panel(string name, float posX, float posY, float posZ, float width, float height, RenderColor color) : base(name, posX, posY, posZ, width, height, color)
        { }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        #endregion
    }
}

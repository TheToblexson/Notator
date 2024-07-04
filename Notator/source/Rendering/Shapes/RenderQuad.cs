using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notator.Rendering.Shapes
{
    public class RenderQuad : RenderShape
    {
        #region Private Properties

        #endregion

        #region Public Properties

        #endregion

        #region Constructors

        /// <summary>
        /// Create a colored quad for rendering.
        /// </summary>
        /// <param name="posX">The x position of the bottom left corner.</param>
        /// <param name="posY">The y position of the bottom left corner.</param>
        /// <param name="posZ">The z position of the bottom left corner.</param>
        /// <param name="width">The width of the quad.</param>
        /// <param name="height">The height of the quad.</param>
        /// <param name="color">The color of the quad.</param>
        public RenderQuad(float posX, float posY, float posZ, float width, float height, RenderColor color)
            : base([new(posX,         posY,          posZ, color.R, color.G, color.B, color.A, 0f, 0f, -1f), // bottom left
                    new(posX,         posY + height, posZ, color.R, color.G, color.B, color.A, 0f, 0f, -1f), // top left
                    new(posX + width, posY + height, posZ, color.R, color.G, color.B, color.A, 0f, 0f, -1f), // top right
                    new(posX + width, posY,          posZ, color.R, color.G, color.B, color.A, 0f, 0f, -1f)],// bottom right
                    [0u, 1u, 3u, 1u, 2u, 3u])
        { }

        /// <summary>
        /// Create a textured quad for rendering.
        /// </summary>
        /// <param name="posX">The x position of the bottom left corner.</param>
        /// <param name="posY">The y position of the bottom left corner.</param>
        /// <param name="posZ">The z position of the bottom left corner.</param>
        /// <param name="width">The width of the quad.</param>
        /// <param name="height">The height of the quad.</param>
        /// <param name="color">The index of the texture to use.</param>
        public RenderQuad(float posX, float posY, float posZ, float width, float height, uint textureIndex)
            : base([new(posX,         posY,          posZ, 0f, 0f, 0f, 0f, 0f, 0f, textureIndex), // bottom left
                    new(posX,         posY + height, posZ, 0f, 0f, 0f, 0f, 1f, 0f, textureIndex), // top left
                    new(posX + width, posY + height, posZ, 0f, 0f, 0f, 0f, 1f, 1f, textureIndex), // top right
                    new(posX + width, posY,          posZ, 0f, 0f, 0f, 0f, 0f, 1f, textureIndex)],// bottom right
                    [0u, 1u, 3u, 1u, 2u, 3u])
        { }

        #endregion

        #region Private Methods

        #endregion

        #region Public Methods

        #endregion
    }
}

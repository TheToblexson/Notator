using Silk.NET.Maths;
using Silk.NET.SDL;
using System.Drawing;
using System.Numerics;

namespace Notator.source.Rendering.Shapes
{
    public class RenderVertex(Vector3D<float> position, VectorColour<float> colour, Vector2D<float> textureCoordinates, int textureID)
    {
        #region Fields

        #endregion

        #region Properties

        private Vector3D<float> Position { get; set; } = position;
        public VectorColour<float> Colour { get; set; } = colour;
        private Vector2D<float> TextureCoordinates { get; set; } = textureCoordinates;
        private int TextureID { get; set; } = textureID;

        #endregion
        #region Constructors

        public RenderVertex(float x, float y, float z, float r, float g, float b, float a, float u, float v, int textureID)
            : this(new(x, y, z), new(r, g, b, a), new(u, v), textureID) { }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public float[] ToArray()
        {
            return [Position.X, Position.Y, Position.Z,
                    Colour.R, Colour.G, Colour.B, Colour.A,
                    TextureCoordinates.X, TextureCoordinates.Y,
                    TextureID];
        }

        #endregion
    }
}

using Silk.NET.Maths;
using System.Drawing;
using System.Numerics;
using System.Transactions;

namespace Notator.source.Rendering.Shapes
{
    public class RenderRectangle : RenderShape
    {
        #region Fields

        private static readonly Vector2D<float>[] _uvs = [new(0.0f, 1.0f),
                                                          new(0.0f, 0.0f),
                                                          new(1.0f, 0.0f),
                                                          new(1.0f, 1.0f)];

        private static readonly uint[] _indices = [0u, 1u, 2u, 2u, 3u, 0u];

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public RenderRectangle(Vector3D<float> position, Vector2D<float> size, VectorColour<float> colour, int textureID)
            : base(generateVertices(position,size,colour,textureID), _indices) {}

        private static RenderVertex[] generateVertices(Vector3D<float> position, Vector2D<float> size, VectorColour<float> colour, int textureID)
        {
            RenderVertex[] vertices = new RenderVertex[4];
            vertices[0] = new(position, colour, _uvs[0], textureID);
            vertices[1] = new(new(position.X, position.Y + size.Y, position.Z), colour, _uvs[1], textureID);
            vertices[2] = new(new(position.X + size.X, position.Y + size.Y, position.Z), colour, _uvs[2], textureID);
            vertices[3] = new(new(position.X + size.X, position.Y, position.Z), colour, _uvs[3], textureID);
            return vertices;
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

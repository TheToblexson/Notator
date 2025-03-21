
using Silk.NET.Maths;

namespace Notator.source.Rendering.Shapes
{
    public class RenderShape
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        public float[] VerticesArray { get; private set; }
        public RenderVertex[] Vertices { get; private set; }
        public uint VertexCount { get; private set; }
        public uint[] Indices { get; private set; }

        #region Constructors
        public RenderShape(RenderVertex[] vertices, uint[] indices)
        {
            Vertices = vertices;
            List<float> verts = new();
            foreach (var vertex in vertices)
                verts.AddRange(vertex.ToArray());
            VerticesArray = verts.ToArray();
            VertexCount = (uint)vertices.Length;
            Indices = indices;
        }
        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods
        public uint[] GetOffsetIndices(uint offset)
        {
            uint[] offsetIndices = new uint[Indices.Length];
            for (int i = 0; i < Indices.Length; i++)
            {
                offsetIndices[i] = Indices[i] + offset;
            }
            return offsetIndices;
        }

        public void SetColour(VectorColour<float> colour)
        {
            List<float> vList = new();
            foreach (var vertex in Vertices)
            {
                vertex.Colour = colour;
                vList.AddRange(vertex.ToArray());
            }
            VerticesArray = vList.ToArray();
        }

        //Assumes that all vertices are the same colour, they can be different if I need them to be in the future
        public VectorColour<float> GetColour()
        {
            return Vertices[0].Colour;
        }
        #endregion
    }
}

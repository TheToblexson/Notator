using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Notator.Rendering.Shapes
{
    /// <summary>
    /// A shape for rendering.
    /// </summary>
    /// <param name="indices">The shape's indices.</param>
    /// <param name="vertices">The shape's vertices.</param>
    public class RenderShape
    {
        /// <summary>
        /// The shape's indices.
        /// </summary>
        public List<uint> Indices { get; private set; }

        /// <summary>
        /// The shape's vertices.
        /// </summary>
        public List<float> Vertices { get; }

        public uint VertexCount { get; }

        /// <summary>
        /// Create a shape for rendering.
        /// </summary>
        /// <param name="indices"></param>
        /// <param name="vertices"></param>
        public RenderShape(List<RenderVertex> vertices, List<uint> indices)
        {
            Vertices = [];
            foreach (var vertex in vertices)
            {
                Vertices.AddRange(vertex.Values);
            }
            Indices = indices;
            VertexCount = (uint)vertices.Count;
        }

        /// <summary>
        /// Offset all the indices by the given amount.
        /// </summary>
        /// <param name="offset">The amount to offset the indices by.</param>
        public void OffsetIndices(uint offset)
        {
            for(int i = 0; i < Indices.Count; i++)
            {
                Indices[i] += offset;
            }
        }
    }
}

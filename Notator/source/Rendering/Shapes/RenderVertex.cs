using Silk.NET.Maths;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Notator.Rendering.Shapes
{
    /// <summary>
    /// A vertex for renderer.
    /// </summary>
    public struct RenderVertex
    {
        /// <summary>
        /// Create a vertex for rendering.
        /// </summary>
        /// <param name="posX">The x position of the vertex.</param>
        /// <param name="posY">The y position of the vertex.</param>
        /// <param name="posZ">The z position of the vertex.</param>
        /// <param name="red">The red component of the vertex color.</param>
        /// <param name="green">The green component of the vertex color.</param>
        /// <param name="blue">The blue component of the vertex color.</param>
        /// <param name="alpha">The alpha component of the vertex color.</param>
        /// <param name="texX">The x position of the texture.</param>
        /// <param name="texY">The y position of the texture.</param>
        /// <param name="texIndex">The index of the texture.</param>
        public RenderVertex(float posX, float posY, float posZ, float red, float green, float blue, float alpha, float texX, float texY, float texIndex)
        {
            Values = [posX, posY, posZ, red, green, blue, alpha, texX, texY, texIndex];
        }

        public float[] Values { get; set; } = new float[Count];

        public readonly Vector3D<float> Position
        {
            get => new(Values[0], Values[1], Values[2]);
            set
            {
                Values[0] = value.X;
                Values[1] = value.Y;
                Values[2] = value.Z;
            }
        }
        public readonly RenderColor Color
        {
            get => new(Values[3], Values[4], Values[5], Values[6]);
            set
            {
                Values[3] = value.R;
                Values[4] = value.G;
                Values[5] = value.B;
                Values[6] = value.A;
            }
        }
        public readonly Vector2D<float> TextureCoordinates
        {
            get => new(Values[7], Values[8]);
            set
            {
                Values[7] = value.X;
                Values[8] = value.Y;
            }
        }
        public readonly float TextureIndex
        {
            get => Values[9];
            set => Values[9] = value;
        }
        public static uint Count = 10;
        public static uint Size => Count * sizeof(float);
    }
}

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Notator.Rendering.Shapes
{
    /// <summary>
    /// A normalised (0.0 to 1.0) rgba color.
    /// </summary>
    /// <param name="red">The red component of the color.</param>
    /// <param name="green">The green component of the color.</param>
    /// <param name="blue">The blue component of the color.</param>
    /// <param name="alpha">The alpha component of the color.</param>
    public struct RenderColor(float red, float green, float blue, float alpha)
    {
        public RenderColor(Color color) : this(color.R / 255, color.G / 255, color.B / 255, color.A / 255)
        { }

        /// <summary>
        /// The normalised red value.
        /// </summary>
        public float R { get; set; } = red;

        /// <summary>
        /// The normalised green value.
        /// </summary>
        public float G { get; set; } = green;

        /// <summary>
        /// The normalised blue value.
        /// </summary>
        public float B { get; set; } = blue;

        /// <summary>
        /// The normalised alpha value.
        /// </summary>
        public float A { get; set; } = alpha;
    }
}

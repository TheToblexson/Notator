using Silk.NET.Maths;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Notator.source.Rendering.Shapes
{
    public struct VectorColour<T>(T r, T g, T b, T a) : IEquatable<VectorColour<T>>, IFormattable where T : unmanaged, IFormattable, IEquatable<T>, IComparable<T>
    {
        #region Fields

        #endregion

        #region Properties
        public T R = r;
        public T G = g;
        public T B = b;
        public T A = a;

        #endregion
        #region Constructors
        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        public readonly bool Equals(VectorColour<T> other)
        {
            return this == other;
        }

        public override readonly bool Equals(object? obj)
        {
            if (obj is VectorColour<T> other)
                return Equals(other);
            return false;
        }

        public override readonly string ToString()
        {
            return ToString("G", CultureInfo.CurrentCulture);
        }

        public readonly string ToString(string? format)
        {
            return ToString(format, CultureInfo.CurrentCulture);
        }

        public override readonly int GetHashCode()
        {
            return HashCode.Combine(R, G, B, A);
        }

        public readonly string ToString(string? format, IFormatProvider? formatProvider)
        {
            StringBuilder stringBuilder = new StringBuilder();
            string numberGroupSeparator = NumberFormatInfo.GetInstance(formatProvider).NumberGroupSeparator;
            stringBuilder.Append('<');
            stringBuilder.Append(R.ToString(format, formatProvider));
            stringBuilder.Append(numberGroupSeparator);
            stringBuilder.Append(' ');
            stringBuilder.Append(G.ToString(format, formatProvider));
            stringBuilder.Append(numberGroupSeparator);
            stringBuilder.Append(' ');
            stringBuilder.Append(B.ToString(format, formatProvider));
            stringBuilder.Append(numberGroupSeparator);
            stringBuilder.Append(' ');
            stringBuilder.Append(A.ToString(format, formatProvider));
            stringBuilder.Append('>');
            return stringBuilder.ToString();
        }
        #endregion

        #region Operator Overrides
        public static implicit operator VectorColour<T>(Vector4D<T> vector)
        {
            return new(vector.X, vector.Y, vector.Z, vector.W);
        }

        public static implicit operator Vector4D<T>(VectorColour<T> colour)
        {
            return new(colour.R, colour.G, colour.B, colour.A);
        }

        public static explicit operator Vector3D<T>(VectorColour<T> colour)
        {
            return new(colour.R, colour.G, colour.B);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool operator ==(VectorColour<T> left, VectorColour<T> right)
        {
            return Scalar.Equal(left.R, right.R) && 
                   Scalar.Equal(left.G, right.G) && 
                   Scalar.Equal(left.B, right.B) &&
                   Scalar.Equal(left.A, right.A);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
        public static bool operator !=(VectorColour<T> left, VectorColour<T> right)
        {
            return !(left == right);
        }
        #endregion
    }
}

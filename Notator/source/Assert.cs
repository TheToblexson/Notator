using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Notator.source
{
    public class Assert
    {
        #region Fields

        #endregion

        #region Properties

        #endregion

        #region Constructors

        public Assert()
        {

        }

        #endregion

        #region Private Methods

        #endregion

        #region Protected Methods

        #endregion

        #region Public Methods

        [Conditional("DEBUG")]
        public static void NotNull([NotNull] object? nullableReference, string name)
        {
            if (nullableReference == null)
                throw new Exception($"AssertNotNull failed: {name} is Null");
        }

        #endregion
    }
}

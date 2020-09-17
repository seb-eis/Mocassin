using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Extensions
{
    /// <summary>
    ///     Provides extension methods for the SharpDX <see cref="Matrix" /> structure
    /// </summary>
    public static class DxMatrixExtensions
    {
        /// <summary>
        ///     Checks if a <see cref="Matrix" /> inverts the orientation of elements. Only works if the matrix is a proper
        ///     transformation matrix
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns></returns>
        public static bool FlipsOrientation(this Matrix matrix) => matrix.Determinant() < 0;
    }
}
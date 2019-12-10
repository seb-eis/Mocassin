using System.Collections.Generic;
using HelixToolkit.Wpf.SharpDX;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Extensions
{
    /// <summary>
    ///     Provides extension methods for the helix toolkit SharpDX <see cref="MeshBuilder" />
    /// </summary>
    public static class DxMeshBuilderExtensions
    {
        /// <summary>
        ///     Adds a revolved geometry to the <see cref="MeshBuilder" /> that has the shape of a two headed arrow
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="point0"></param>
        /// <param name="point1"></param>
        /// <param name="diameter"></param>
        /// <param name="headLength"></param>
        /// <param name="thetaDiv"></param>
        public static void AddTwoHeadedArrow(this MeshBuilder builder, in Vector3 point0, in Vector3 point1, double diameter, double headLength = 3,
            int thetaDiv = 18)
        {
            var direction = point1 - point0;
            var length = direction.Length();
            var radius = (float) diameter / 2f;
            var headLengthTimesDiameter = (float) (diameter * headLength);
            var revolvePoints = new List<Vector2>
            {
                new Vector2(0, 0),
                new Vector2(headLengthTimesDiameter, (float) diameter),
                new Vector2(headLengthTimesDiameter, radius),
                new Vector2(length - headLengthTimesDiameter, radius),
                new Vector2(length - headLengthTimesDiameter, (float) diameter),
                new Vector2(length, 0)
            };
            builder.AddRevolvedGeometry(revolvePoints, null, point0, direction, thetaDiv);
        }
    }
}
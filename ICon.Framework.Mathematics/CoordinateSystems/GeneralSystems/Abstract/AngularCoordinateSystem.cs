using ICon.Mathematics.ValueTypes;

namespace ICon.Mathematics.Coordinates
{
    /// <summary>
    ///     Represents an angular coordinate system of the specified type which does not have fixed base vectors
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public abstract class AngularCoordinateSystem<T1> : CoordinateSystem<T1> where T1 : struct, ICoordinates
    {
    }
}
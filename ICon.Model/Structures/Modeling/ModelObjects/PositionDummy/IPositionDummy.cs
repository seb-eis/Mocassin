using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Structures
{
    /// <summary>
    ///     Represents a purely visual unit cell position object that cannot be used for model purposes
    /// </summary>
    public interface IPositionDummy : IModelObject
    {
        /// <summary>
        ///     Get the fractional coordinate vector of the position dummy
        /// </summary>
        Fractional3D Vector { get; }
    }
}
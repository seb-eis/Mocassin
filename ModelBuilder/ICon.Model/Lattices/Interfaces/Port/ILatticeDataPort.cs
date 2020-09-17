using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Represents a read only data access port for the lattice reference data
    /// </summary>
    public interface ILatticeDataPort : IModelDataPort
    {
        /// <summary>
        ///     Get read only list of used dopings
        /// </summary>
        /// <returns></returns>
        FixedList<IDoping> GetDopings();

        /// <summary>
        ///     Get read only list of DopingCombinations (dopant, doped element, unit cell entry)
        /// </summary>
        /// <returns></returns>
        FixedList<IDopingCombination> GetDopingCombinations();

        /// <summary>
        ///     Get read only list of building blocks
        /// </summary>
        /// <returns></returns>
        FixedList<IBuildingBlock> GetBuildingBlocks();
    }
}
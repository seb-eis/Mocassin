using Mocassin.Framework.Collections;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Lattices
{
    /// <summary>
    ///     Lattice data manager that provides safe read only access to the Lattice base model data
    /// </summary>
    internal class LatticeDataManager : ModelDataManager<LatticeModelData>, ILatticeDataPort
    {
        /// <summary>
        ///     Create new lattice data manager for the provided data object
        /// </summary>
        /// <param name="modelData"></param>
        public LatticeDataManager(LatticeModelData modelData)
            : base(modelData)
        {
        }

        /// <summary>
        ///     Get read only list of used dopings
        /// </summary>
        /// <returns></returns>
        public ListReadOnlyWrapper<IDoping> GetDopings()
        {
            return ListReadOnlyWrapper<IDoping>.FromEnumerable(Data.Dopings);
        }

        /// <summary>
        ///     Get read only list of building blocks
        /// </summary>
        /// <returns></returns>
        public ListReadOnlyWrapper<IBuildingBlock> GetBuildingBlocks()
        {
            return ListReadOnlyWrapper<IBuildingBlock>.FromEnumerable(Data.BuildingBlocks);
        }

        /// <summary>
        ///     Get read only list of DopingCombinations
        /// </summary>
        /// <returns></returns>
        public ListReadOnlyWrapper<IDopingCombination> GetDopingCombinations()
        {
            return ListReadOnlyWrapper<IDopingCombination>.FromEnumerable(Data.DopingCombinations);
        }
    }
}
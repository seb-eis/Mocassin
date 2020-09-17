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
        public FixedList<IDoping> GetDopings() => FixedList<IDoping>.FromEnumerable(Data.Dopings);

        /// <summary>
        ///     Get read only list of building blocks
        /// </summary>
        /// <returns></returns>
        public FixedList<IBuildingBlock> GetBuildingBlocks() => FixedList<IBuildingBlock>.FromEnumerable(Data.BuildingBlocks);

        /// <summary>
        ///     Get read only list of DopingCombinations
        /// </summary>
        /// <returns></returns>
        public FixedList<IDopingCombination> GetDopingCombinations() => FixedList<IDopingCombination>.FromEnumerable(Data.DopingCombinations);
    }
}
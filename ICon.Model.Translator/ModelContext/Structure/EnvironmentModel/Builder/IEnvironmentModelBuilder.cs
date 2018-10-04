using System.Collections.Generic;
using System.Threading.Tasks;
using ICon.Model.Structures;

namespace ICon.Model.Translator.ModelContext
{
    /// <summary>
    /// Builder for environment models that fully describe the surroundings of a simulation position
    /// </summary>
    public interface IEnvironmentModelBuilder
    {
        /// <summary>
        /// Builds the environment model for each passed unit cell position
        /// </summary>
        /// <param name="unitCellPositions"></param>
        /// <param name="energyModelContextTask"></param>
        /// <returns></returns>
        IList<IEnvironmentModel> BuildModels(IEnumerable<IUnitCellPosition> unitCellPositions);
    }
}
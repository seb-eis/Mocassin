using System.Collections.Generic;
using Mocassin.Model.Structures;

namespace Mocassin.Model.Translator.ModelContext
{
    /// <summary>
    ///     Builder for environment models that fully describe the surroundings of a simulation position
    /// </summary>
    public interface IEnvironmentModelBuilder
    {
        /// <summary>
        ///     Builds the environment model for each passed unit cell position
        /// </summary>
        /// <param name="cellReferencePositions"></param>
        /// <returns></returns>
        IList<IEnvironmentModel> BuildModels(IEnumerable<ICellReferencePosition> cellReferencePositions);
    }
}
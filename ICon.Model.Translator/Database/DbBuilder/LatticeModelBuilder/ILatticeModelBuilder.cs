using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.DbBuilder
{
    /// <summary>
    ///     Database model builder that converts simulation model definitions from the model context into the required
    ///     lattice database model objects using a configuration information
    /// </summary>
    public interface ILatticeDbModelBuilder
    {
        /// <summary>
        ///     Builds a new simulation lattice model for the passed simulation model using the provided configuration information
        /// </summary>
        /// <param name="simulationModel"></param>
        /// <param name="latticeConfiguration"></param>
        /// <returns></returns>
        SimulationLatticeModel BuildModel(ISimulationModel simulationModel, LatticeConfiguration latticeConfiguration);
    }
}
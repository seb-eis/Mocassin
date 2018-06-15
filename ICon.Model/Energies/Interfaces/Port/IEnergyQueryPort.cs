using ICon.Framework.Collections;
using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    /// Represents a query port for query based access to the reference and extended energies data
    /// </summary>
    public interface IEnergyQueryPort : IModelQueryPort<IEnergyDataPort>, IModelQueryPort<IEnergyCachePort>
    {

    }
}

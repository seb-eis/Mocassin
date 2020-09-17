using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Represents a manager for model energies and related modeling parameters and objects
    /// </summary>
    public interface IEnergyManager : IModelManager<IEnergyInputPort, IEnergyEventPort, IEnergyQueryPort>
    {
    }
}
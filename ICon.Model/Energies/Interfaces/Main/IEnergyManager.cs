using ICon.Model.Basic;

namespace ICon.Model.Energies
{
    /// <summary>
    ///     Represents a manager for model energies and related modeling parameters and objects
    /// </summary>
    public interface IEnergyManager : IModelManager<IEnergyInputPort, IEnergyEventPort, IEnergyQueryPort>
    {
    }
}
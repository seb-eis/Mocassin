using Mocassin.Model.Basic;

namespace Mocassin.Model.Energies
{
    /// <summary>
    ///     Basic transitions notification manager that handles distribution of push based update notifications about changes
    ///     in the transition manager base data
    /// </summary>
    internal class EnergyEventManager : ModelEventManager, IEnergyEventPort
    {
    }
}
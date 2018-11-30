using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Energy injector for pair energy models. Injects the energy context data from serializable interaction
    ///     specifications and corrects affiliated data structures
    /// </summary>
    [ExportInjector]
    public class PairEnergyInjector : ReflectiveDataInjector<MmlInteraction<MmlPairEnergy>, IPairEnergyModel>
    {
        
    }
}
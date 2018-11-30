using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Energy extractor for group energy models. Converts the energy context data of group energy models to serializable
    ///     interaction specifications
    /// </summary>
    public class GroupEnergyExtractor : ReflectiveDataExtractor<IGroupEnergyModel, MmlInteraction<MmlGroupEnergy>>
    {
    }
}
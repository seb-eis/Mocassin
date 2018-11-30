using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Relay based group energy adapter that uses the default injector and extractor for group energy information
    /// </summary>
    [ExportAdapter]
    public class GroupEnergyAdapter : RelayDataAdapter<MmlInteraction<MmlGroupEnergy>, IGroupEnergyModel>
    {
        /// <inheritdoc />
        [ImportExtractor(typeof(GroupEnergyExtractor))]
        public override IDataExtractor<IGroupEnergyModel, MmlInteraction<MmlGroupEnergy>> Extractor 
        { 
            get => base.Extractor; 
            protected set => base.Extractor = value;
        }

        /// <inheritdoc />
        [ImportInjector(typeof(GroupEnergyInjector))]
        public override IDataInjector<MmlInteraction<MmlGroupEnergy>, IGroupEnergyModel> Injector
        {
            get => base.Injector;
            protected set => base.Injector = value;
        }
    }
}
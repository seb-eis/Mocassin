using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Relay based pair energy adapter that uses the default injector and extractor for pair energy information
    /// </summary>
    [ExportAdapter]
    public class PairEnergyAdapter : RelayDataAdapter<MmlInteraction<MmlPairEnergy>, IPairEnergyModel>
    {
        /// <inheritdoc />
        [ImportExtractor(typeof(PairEnergyExtractor))]
        public override IDataExtractor<IPairEnergyModel, MmlInteraction<MmlPairEnergy>> Extractor
        {
            get => base.Extractor;
            protected set => base.Extractor = value;
        }

        /// <inheritdoc />
        [ImportInjector(typeof(GroupEnergyInjector))]
        public override IDataInjector<MmlInteraction<MmlPairEnergy>, IPairEnergyModel> Injector
        {
            get => base.Injector;
            protected set => base.Injector = value;
        }
    }
}
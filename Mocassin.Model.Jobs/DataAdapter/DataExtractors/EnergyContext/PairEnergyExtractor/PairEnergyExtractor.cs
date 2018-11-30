using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Energies;
using Mocassin.Model.Mml.Descriptions;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Energy extractor for pair energy models. Converts the energy context data of pair energy models to serializable
    ///     interaction specifications
    /// </summary>
    public class PairEnergyExtractor : ReflectiveDataExtractor<IPairEnergyModel, MmlInteraction<MmlPairEnergy>>
    {
        /// <summary>
        ///     Extracts the misc data from the passed pair energy model into the Mml data object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="energyModel"></param>
        [ExtractionMethod]
        public void ExtractMiscData(MmlInteraction<MmlPairEnergy> data, IPairEnergyModel energyModel)
        {
            data.ModelContextId = energyModel.ModelId;
            data.TargetModelHash = energyModel.GetHashCode();
            data.Description = DescriptionSource.Default.CreateDescription(energyModel);
        }

        /// <summary>
        ///     Extracts the position data from the passed pair energy model into the Mml data object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="energyModel"></param>
        [ExtractionMethod]
        public void ExtractPositionData(MmlInteraction<MmlPairEnergy> data, IPairEnergyModel energyModel)
        {
            data.Positions = data.Positions ?? new List<MmlPosition>();
            data.Positions.Add(MmlPosition.FromModelObject(energyModel.PairInteraction.Position0));
            data.Positions.Add(MmlPosition.FromModelObject(energyModel.PairInteraction.Position1));
        }

        /// <summary>
        ///     Extracts the energy data from the passed pair energy model into the Mml data object
        /// </summary>
        /// <param name="data"></param>
        /// <param name="energyModel"></param>
        [ExtractionMethod]
        public void ExtractEnergyData(MmlInteraction<MmlPairEnergy> data, IPairEnergyModel energyModel)
        {
            data.Energies = data.Energies ?? new List<MmlPairEnergy>(energyModel.EnergyEntries.Count);
        }
    }
}
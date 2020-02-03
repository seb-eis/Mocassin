using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Energies;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.EnergyModel
{
    /// <summary>
    ///     Serializable data object for storing a defect energy background as a set of <see cref="DefectEnergyData" />
    ///     instances
    /// </summary>
    [XmlRoot]
    public class DefectBackgroundData : ProjectDataObject
    {
        private ObservableCollection<DefectEnergyData> defectEnergies;

        /// <summary>
        ///     Get or set the list of <see cref="DefectEnergyData" /> that describe the defect background
        /// </summary>
        [XmlArray]
        public ObservableCollection<DefectEnergyData> DefectEnergies
        {
            get => defectEnergies;
            set => SetProperty(ref defectEnergies, value);
        }

        /// <summary>
        ///     Create new <see cref="DefectBackgroundData" /> with empty defect list
        /// </summary>
        public DefectBackgroundData()
        {
            DefectEnergies = new ObservableCollection<DefectEnergyData>();
        }

        /// <summary>
        ///     Get the defect background as a list of <see cref="DefectEnergy" /> objects for the model input pipeline
        /// </summary>
        /// <returns></returns>
        public List<DefectEnergy> AsDefectList()
        {
            return DefectEnergies.Select(x => x.GetInputObject()).ToList();
        }
    }
}
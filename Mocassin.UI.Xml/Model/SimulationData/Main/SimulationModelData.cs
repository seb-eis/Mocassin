using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.SimulationModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the
    ///     <see cref="Mocassin.Model.Simulations.ISimulationManager" />
    ///     system
    /// </summary>
    [XmlRoot]
    public class SimulationModelData : ModelManagerData
    {
        private ObservableCollection<KineticSimulationData> kineticSimulations;
        private ObservableCollection<MetropolisSimulationData> metropolisSimulations;

        /// <summary>
        ///     Get or set the list of metropolis simulations
        /// </summary>
        [XmlArray]
        public ObservableCollection<MetropolisSimulationData> MetropolisSimulations
        {
            get => metropolisSimulations;
            set => SetProperty(ref metropolisSimulations, value);
        }

        /// <summary>
        ///     Get or set the list of metropolis simulations
        /// </summary>
        [XmlArray]
        public ObservableCollection<KineticSimulationData> KineticSimulations
        {
            get => kineticSimulations;
            set => SetProperty(ref kineticSimulations, value);
        }

        /// <summary>
        ///     Creates new <see cref="SimulationModelData" /> with empty component lists
        /// </summary>
        public SimulationModelData()
        {
            MetropolisSimulations = new ObservableCollection<MetropolisSimulationData>();
            KineticSimulations = new ObservableCollection<KineticSimulationData>();
        }


        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield break;
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return MetropolisSimulations.Select(x => x.GetInputObject())
                .Concat(KineticSimulations.Select(x => x.GetInputObject()));
        }
    }
}
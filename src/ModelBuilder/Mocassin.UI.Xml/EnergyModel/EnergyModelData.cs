using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.UI.Data.Base;

namespace Mocassin.UI.Data.EnergyModel
{
    /// <summary>
    ///     Serializable data object to supply all data managed by the <see cref="Mocassin.Model.Energies.IEnergyManager" />
    ///     system
    /// </summary>
    [XmlRoot]
    public class EnergyModelData : ModelManagerData
    {
        private ObservableCollection<GroupInteractionData> groupInteractions;
        private StableEnvironmentData stableEnvironment;
        private ObservableCollection<UnstableEnvironmentData> unstableEnvironments;

        /// <summary>
        ///     Get or set the stable environment info of the energy model
        /// </summary>
        [XmlElement]
        public StableEnvironmentData StableEnvironment
        {
            get => stableEnvironment;
            set => SetProperty(ref stableEnvironment, value);
        }

        /// <summary>
        ///     Get or set the list of unstable environments of the energy model
        /// </summary>
        [XmlArray]
        public ObservableCollection<UnstableEnvironmentData> UnstableEnvironments
        {
            get => unstableEnvironments;
            set => SetProperty(ref unstableEnvironments, value);
        }

        /// <summary>
        ///     Get or set the list of unstable environments of the energy model
        /// </summary>
        [XmlArray]
        public ObservableCollection<GroupInteractionData> GroupInteractions
        {
            get => groupInteractions;
            set => SetProperty(ref groupInteractions, value);
        }

        /// <summary>
        ///     Creates new <see cref="EnergyModelData" /> with empty component lists
        /// </summary>
        public EnergyModelData()
        {
            GroupInteractions = new ObservableCollection<GroupInteractionData>();
            UnstableEnvironments = new ObservableCollection<UnstableEnvironmentData>();
            StableEnvironment = new StableEnvironmentData();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelParameter> GetInputParameters()
        {
            yield return StableEnvironment.GetInputObject();
        }

        /// <inheritdoc />
        public override IEnumerable<IModelObject> GetInputObjects()
        {
            return UnstableEnvironments.Select(x => x.GetInputObject())
                                       .Concat(GroupInteractions.Select(x => x.GetInputObject()));
        }
    }
}
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Mocassin.Framework.Extensions;
using Mocassin.Model.Energies;
using Mocassin.Model.ModelProject;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.Customization
{
    /// <summary>
    ///     Serializable object that carries data for customization of <see cref="Mocassin.Model.Energies.IEnergyManager" />
    ///     interaction settings through the <see cref="IEnergySetterProvider" /> system
    /// </summary>
    [XmlRoot]
    public class EnergyModelCustomizationData : ModelCustomizationData, IDuplicable<EnergyModelCustomizationData>
    {
        private ObservableCollection<GroupEnergySetData> groupEnergyParameterSets;
        private ObservableCollection<PairEnergySetData> stablePairEnergyParameterSets;
        private ObservableCollection<PairEnergySetData> unstablePairEnergyParameterSets;

        /// <summary>
        ///     Get or set the list of <see cref="IStablePairInteraction" /> customization data sets
        /// </summary>
        [XmlArray]
        public ObservableCollection<PairEnergySetData> StablePairEnergyParameterSets
        {
            get => stablePairEnergyParameterSets;
            set => SetProperty(ref stablePairEnergyParameterSets, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="IUnstablePairInteraction" /> customization data sets
        /// </summary>
        [XmlArray]
        public ObservableCollection<PairEnergySetData> UnstablePairEnergyParameterSets
        {
            get => unstablePairEnergyParameterSets;
            set => SetProperty(ref unstablePairEnergyParameterSets, value);
        }

        /// <summary>
        ///     Get or set the list of <see cref="Mocassin.Model.Energies.IGroupInteraction" /> customization data sets
        /// </summary>
        [XmlArray]
        public ObservableCollection<GroupEnergySetData> GroupEnergyParameterSets
        {
            get => groupEnergyParameterSets;
            set => SetProperty(ref groupEnergyParameterSets, value);
        }

        /// <inheritdoc />
        public EnergyModelCustomizationData Duplicate()
        {
            var copy = new EnergyModelCustomizationData
            {
                Parent = Parent,
                Name = Name,
                stablePairEnergyParameterSets = stablePairEnergyParameterSets.Select(x => x.Duplicate()).ToObservableCollection(),
                unstablePairEnergyParameterSets = unstablePairEnergyParameterSets.Select(x => x.Duplicate()).ToObservableCollection(),
                groupEnergyParameterSets = groupEnergyParameterSets.Select(x => x.Duplicate()).ToObservableCollection()
            };
            return copy;
        }

        /// <inheritdoc />
        object IDuplicable.Duplicate() => Duplicate();

        /// <inheritdoc />
        public override void PushToModel(IModelProject modelProject)
        {
            var setterProvider = modelProject.Manager<IEnergyManager>().DataAccess.Query(x => x.GetEnergySetterProvider());

            foreach (var parameterSet in StablePairEnergyParameterSets)
            {
                var setter = setterProvider.GetStablePairEnergySetter(parameterSet.ModelIndex);
                parameterSet.PushToModel(modelProject, setter);
            }

            foreach (var parameterSet in UnstablePairEnergyParameterSets)
            {
                var setter = setterProvider.GetUnstablePairEnergySetter(parameterSet.ModelIndex);
                parameterSet.PushToModel(modelProject, setter);
            }

            foreach (var parameterSet in GroupEnergyParameterSets)
            {
                var setter = setterProvider.GetGroupEnergySetter(parameterSet.ModelIndex);
                parameterSet.PushToModel(modelProject, setter);
            }
        }

        /// <summary>
        ///     Creates a new <see cref="EnergyModelCustomizationData" /> by transforming all defined data in the passed
        ///     <see cref="IEnergySetterProvider" /> into serializable data formats referencing the
        ///     <see cref="ProjectModelData" /> parent
        /// </summary>
        /// <param name="energySetterProvider"></param>
        /// <param name="parent"></param>
        /// <returns></returns>
        public static EnergyModelCustomizationData Create(IEnergySetterProvider energySetterProvider, ProjectModelData parent)
        {
            if (energySetterProvider == null) throw new ArgumentNullException(nameof(energySetterProvider));
            if (parent == null) throw new ArgumentNullException(nameof(parent));

            var obj = new EnergyModelCustomizationData
            {
                StablePairEnergyParameterSets = energySetterProvider
                                                .GetStablePairEnergySetters()
                                                .Select(x => PairEnergySetData.Create(x, parent))
                                                .ToObservableCollection(),

                UnstablePairEnergyParameterSets = energySetterProvider
                                                  .GetUnstablePairEnergySetters()
                                                  .Select(x => PairEnergySetData.Create(x, parent))
                                                  .ToObservableCollection(),

                GroupEnergyParameterSets = energySetterProvider
                                           .GetGroupEnergySetters()
                                           .Select(x => GroupEnergySetData.Create(x, parent))
                                           .ToObservableCollection()
            };

            return obj;
        }
    }
}
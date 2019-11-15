using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Base;
using Mocassin.UI.Xml.EnergyModel;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.EnergyModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel"/> for the set of <see cref="DefectEnergyGraph"/> definitions
    /// </summary>
    public class DefectEnergyControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get or set the active defect background target
        /// </summary>
        private DefectBackgroundGraph DefectBackground { get; set; }

        /// <summary>
        ///     Get the <see cref="CollectionControlViewModel{T}"/> for the <see cref="DefectEnergyGraph"/> set
        /// </summary>
        public CollectionControlViewModel<DefectEnergyGraph> DefectEnergiesControlViewModel { get; }

        /// <summary>
        ///     Get a <see cref="RelayCommand"/> to force update the current defect collection
        /// </summary>
        public RelayCommand UpdateDefectCollectionCommand { get; }

        /// <inheritdoc />
        public DefectEnergyControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            DefectEnergiesControlViewModel = new CollectionControlViewModel<DefectEnergyGraph>();
            UpdateDefectCollectionCommand = new RelayCommand(
                () => UpdateDefectCollection(DefectBackground, ContentSource), 
                () => ContentSource != null && DefectBackground != null);
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            DefectBackground = contentSource?.ProjectModelGraph?.EnergyModelGraph?.StableEnvironment?.DefectBackground;
            UpdateDefectCollection(DefectBackground, contentSource);
            DefectEnergiesControlViewModel.SetCollection(DefectBackground?.DefectEnergies);
        }

        /// <summary>
        ///     Updates the defect list of <see cref="DefectBackgroundGraph"/> to the context of the passed <see cref="MocassinProjectGraph"/>
        /// </summary>
        /// <param name="contentSource"></param>
        /// <param name="defectBackground"></param>
        public void UpdateDefectCollection(DefectBackgroundGraph defectBackground, MocassinProjectGraph contentSource)
        {
            if (defectBackground == null || contentSource == null) return;

            var positions = contentSource.ProjectModelGraph.StructureModelGraph.UnitCellPositions;
            var occupations = contentSource.ProjectModelGraph.ParticleModelGraph.ParticleSets;

            var oldList = defectBackground.DefectEnergies;
            var newList = new ObservableCollection<DefectEnergyGraph>();
            foreach (var position in positions)
            {
                var particles = occupations.Single(x => x.Key == position.OccupationKey).Particles;
                foreach (var particle in particles)
                {
                    var obj = oldList.SingleOrDefault(x => x.Particle.Equals(particle) && x.UnitCellPosition.TargetGraph.Equals(position));

                    obj = obj ?? new DefectEnergyGraph
                    {
                        Particle = particle.Duplicate(),
                        UnitCellPosition = new ModelObjectReferenceGraph<UnitCellPosition>(position),
                    };
                    newList.Add(obj);
                }
            }
            defectBackground.DefectEnergies = newList;
        }
    }
}
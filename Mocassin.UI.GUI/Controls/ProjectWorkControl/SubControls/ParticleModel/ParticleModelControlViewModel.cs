using System.Collections.Generic;
using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ParticleModelControlView" /> that controls
    ///     <see cref="ParticleModelGraph" /> creation and manipulation
    /// </summary>
    public class ParticleModelControlViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     The <see cref="ParticleGraphs"/> backing field
        /// </summary>
        private IList<ParticleGraph> particleGraphs;

        /// <summary>
        ///     The <see cref="ParticleSetGraphs"/> backing field
        /// </summary>
        private IList<ParticleSetGraph> particleSetGraphs;

        /// <summary>
        ///     The <see cref="SelectedModelGraph"/> backing field
        /// </summary>
        private ProjectModelGraph selectedModelGraph;

        /// <summary>
        ///     Get or set the <see cref="IList{T}"/> of <see cref="ParticleGraph"/> objects
        /// </summary>
        public IList<ParticleGraph> ParticleGraphs
        {
            get => particleGraphs;
            set => SetProperty(ref particleGraphs, value);
        }

        /// <summary>
        ///     Get or set the <see cref="IList{T}"/> of <see cref="ParticleSetGraph"/> objects
        /// </summary>
        public IList<ParticleSetGraph> ParticleSetGraphs
        {
            get => particleSetGraphs;
            set => SetProperty(ref particleSetGraphs, value);
        }

        /// <summary>
        ///     Get or set the currently selected <see cref="ProjectModelGraph"/>
        /// </summary>
        public ProjectModelGraph SelectedModelGraph
        {
            get => selectedModelGraph;
            set
            {
                SetProperty(ref selectedModelGraph, value); 
                OnNewModelGraphSelected(value);
            }
        }

        /// <inheritdoc />
        public ParticleModelControlViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {

        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            SelectedModelGraph = MainProjectControl.ProjectGraphs?.FirstOrDefault()?.ProjectModelGraph;
            base.OnProjectLibraryChangedInternal(newProjectLibrary);
        }

        /// <summary>
        ///     Action that is called if the selected <see cref="ProjectModelGraph"/> changes
        /// </summary>
        private void OnNewModelGraphSelected(ProjectModelGraph modelGraph)
        {
            ParticleGraphs = modelGraph?.ParticleModelGraph?.Particles;
            ParticleSetGraphs = modelGraph?.ParticleModelGraph?.ParticleSets;
        }
    }
}
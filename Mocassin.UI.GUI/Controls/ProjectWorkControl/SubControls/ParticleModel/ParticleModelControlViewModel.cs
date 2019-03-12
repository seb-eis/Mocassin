using System.Collections.Generic;
using System.Linq;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base;
using Mocassin.UI.Xml.Main;
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
        private IEnumerable<ParticleGraph> particleGraphs;

        private IEnumerable<ParticleSetGraph> particleSetGraphs;

        public IEnumerable<ParticleGraph> ParticleGraphs
        {
            get => particleGraphs;
            set => SetProperty(ref particleGraphs, value);
        }

        public IEnumerable<ParticleSetGraph> ParticleSetGraphs
        {
            get => particleSetGraphs;
            set => SetProperty(ref particleSetGraphs, value);
        }

        /// <inheritdoc />
        public ParticleModelControlViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {

        }

        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            base.OnProjectLibraryChangedInternal(newProjectLibrary);
            var model = MainProjectControl.ProjectGraphs?
                .FirstOrDefault()?.ProjectModelGraph.ParticleModelGraph;
            ParticleGraphs = model?.Particles;
            ParticleSetGraphs = model?.ParticleSets;
        }
    }
}
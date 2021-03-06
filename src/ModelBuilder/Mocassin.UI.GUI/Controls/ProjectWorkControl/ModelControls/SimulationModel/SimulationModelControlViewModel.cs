﻿using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel.DataControl;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.SimulationModel
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="SimulationModelControlView" /> that controls
    ///     simulation
    ///     base definitions
    /// </summary>
    public class SimulationModelControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="MetropolisSimulationControlViewModel" /> that controls metropolis simulation base definitions
        /// </summary>
        public MetropolisSimulationControlViewModel MetropolisSimulationViewModel { get; }

        /// <summary>
        ///     Get the <see cref="KineticSimulationControlViewModel" /> that controls kinetic simulation base definitions
        /// </summary>
        public KineticSimulationControlViewModel KineticSimulationViewModel { get; }

        /// <inheritdoc />
        public SimulationModelControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            MetropolisSimulationViewModel = new MetropolisSimulationControlViewModel();
            KineticSimulationViewModel = new KineticSimulationControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            MetropolisSimulationViewModel.ChangeContentSource(contentSource);
            KineticSimulationViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }
    }
}
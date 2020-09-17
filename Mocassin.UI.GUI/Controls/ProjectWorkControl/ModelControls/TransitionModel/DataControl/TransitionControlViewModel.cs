using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.DataControl
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="TransitionControlView" /> that controls transition
    ///     definition
    /// </summary>
    public class TransitionControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="MetropolisTransitionGridControlViewModel " /> that controls metropolis transition definitions
        /// </summary>
        public MetropolisTransitionGridControlViewModel MetropolisTransitionGridViewModel { get; }

        /// <summary>
        ///     Get the <see cref="KineticTransitionGridControlViewModel" /> that controls kinetic transition definitions
        /// </summary>
        public KineticTransitionGridControlViewModel KineticTransitionGridViewModel { get; }

        /// <inheritdoc />
        public TransitionControlViewModel(IProjectAppControl projectControl)
            : base(projectControl)
        {
            MetropolisTransitionGridViewModel = new MetropolisTransitionGridControlViewModel();
            KineticTransitionGridViewModel = new KineticTransitionGridControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            MetropolisTransitionGridViewModel.ChangeContentSource(contentSource);
            KineticTransitionGridViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }
    }
}
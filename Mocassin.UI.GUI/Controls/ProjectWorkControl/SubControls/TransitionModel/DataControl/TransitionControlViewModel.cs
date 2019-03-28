using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.GridControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.DataControl
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="TransitionControlView" /> that controls transition
    ///     definition
    /// </summary>
    public class TransitionControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <summary>
        ///     Get the <see cref="MetropolisTransitionGridControlViewModel " /> that controls metropolis transition definitions
        /// </summary>
        public MetropolisTransitionGridControlViewModel MetropolisTransitionGridViewModel { get; }

        /// <summary>
        ///     Get the <see cref="KineticTransitionGridControlViewModel" /> that controls kinetic transition definitions
        /// </summary>
        public KineticTransitionGridControlViewModel KineticTransitionGridViewModel { get; }

        /// <inheritdoc />
        public TransitionControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            MetropolisTransitionGridViewModel = new MetropolisTransitionGridControlViewModel();
            KineticTransitionGridViewModel = new KineticTransitionGridControlViewModel();
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            if (contentSource is MocassinProjectGraph projectGraph) ChangeContentSource(projectGraph);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
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
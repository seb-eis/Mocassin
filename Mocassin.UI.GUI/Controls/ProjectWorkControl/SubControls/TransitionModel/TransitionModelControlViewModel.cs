using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel.DataControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.TransitionModel
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="TransitionModelControlView" /> that controls
    ///     transition definition
    /// </summary>
    public class TransitionModelControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get the <see cref="DataControl.AbstractTransitionControlViewModel" /> that controls transition abstraction
        /// </summary>
        public AbstractTransitionControlViewModel AbstractTransitionControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="DataControl.TransitionControlViewModel" /> that controls transition definition
        /// </summary>
        public TransitionControlViewModel TransitionControlViewModel { get; }

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public TransitionModelControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            AbstractTransitionControlViewModel = new AbstractTransitionControlViewModel(projectControl);
            TransitionControlViewModel = new TransitionControlViewModel(projectControl);
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
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
            TransitionControlViewModel.ChangeContentSource(contentSource);
            AbstractTransitionControlViewModel.ChangeContentSource(contentSource);
        }
    }
}
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel.DataControl;
using Mocassin.UI.Data.Main;
using Mocassin.UI.Data.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.TransitionModel
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for the <see cref="TransitionModelControlView" /> that controls
    ///     transition definition
    /// </summary>
    public class TransitionModelControlViewModel : ProjectGraphControlViewModel
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
        public TransitionModelControlViewModel(IProjectAppControl projectControl)
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
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            TransitionControlViewModel.ChangeContentSource(contentSource);
            AbstractTransitionControlViewModel.ChangeContentSource(contentSource);
        }
    }
}
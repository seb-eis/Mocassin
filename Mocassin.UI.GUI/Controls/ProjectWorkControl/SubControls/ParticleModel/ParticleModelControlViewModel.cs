using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.Interfaces;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel.DataControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.SubControls.ParticleModel
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for <see cref="ParticleModelControlView" /> that controls
    ///     <see cref="ParticleModelGraph" /> creation and manipulation
    /// </summary>
    public class ParticleModelControlViewModel : PrimaryControlViewModel, IContentSupplier<MocassinProjectGraph>
    {
        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> for the <see cref="ParticleGraph" /> data control
        /// </summary>
        public ParticleControlViewModel ParticleControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> for the <see cref="ParticleSetGraph" /> data
        ///     control
        /// </summary>
        public ParticleSetControlViewModel ParticleSetControlViewModel { get; }

        /// <inheritdoc />
        public MocassinProjectGraph ContentSource { get; protected set; }

        /// <inheritdoc />
        public ParticleModelControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            ParticleControlViewModel = new ParticleControlViewModel();
            ParticleSetControlViewModel = new ParticleSetControlViewModel();
        }

        /// <inheritdoc />
        protected override void OnProjectLibraryChangedInternal(IMocassinProjectLibrary newProjectLibrary)
        {
            ChangeContentSource(null);
        }

        /// <inheritdoc />
        public void ChangeContentSource(MocassinProjectGraph contentSource)
        {
            ContentSource = contentSource;
            ParticleControlViewModel.ChangeContentSource(contentSource);
            ParticleSetControlViewModel.ChangeContentSource(contentSource);
        }

        /// <inheritdoc />
        public void ChangeContentSource(object contentSource)
        {
            ChangeContentSource(contentSource as MocassinProjectGraph);
        }
    }
}
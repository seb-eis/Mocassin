using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel.DataControl;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.ParticleModel;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ParticleModel
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for <see cref="ParticleModelControlView" /> that controls
    ///     <see cref="ParticleModelData" /> creation and manipulation
    /// </summary>
    public class ParticleModelControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> for the <see cref="ParticleData" /> data control
        /// </summary>
        public ParticleControlViewModel ParticleControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="ViewModelBase" /> for the <see cref="ParticleSetData" /> data
        ///     control
        /// </summary>
        public ParticleSetControlViewModel ParticleSetControlViewModel { get; }

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
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            ParticleControlViewModel.ChangeContentSource(contentSource);
            ParticleSetControlViewModel.ChangeContentSource(contentSource);
        }
    }
}
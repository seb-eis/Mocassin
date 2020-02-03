using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel" /> for
    /// </summary>
    public class LatticeModelControlViewModel : ProjectGraphControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="BuildingBlockControlViewModel" /> that manages the building blocks
        /// </summary>
        public BuildingBlockControlViewModel BlockControlViewModel { get; }

        /// <summary>
        ///     Get the <see cref="DopingCombinationControlViewModel" /> that manages doping combinations
        /// </summary>
        public DopingCombinationControlViewModel DopingCombinationViewModel { get; }

        /// <summary>
        ///     Get the <see cref="DopingControlViewModel" /> that manages the doping bases
        /// </summary>
        public DopingControlViewModel DopingViewModel { get; }

        /// <inheritdoc />
        public LatticeModelControlViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            BlockControlViewModel = new BuildingBlockControlViewModel(projectControl);
            DopingCombinationViewModel = new DopingCombinationControlViewModel();
            DopingViewModel = new DopingControlViewModel();
        }

        /// <inheritdoc />
        public override void ChangeContentSource(MocassinProject contentSource)
        {
            ContentSource = contentSource;
            BlockControlViewModel.ChangeContentSource(contentSource);
            DopingCombinationViewModel.ChangeContentSource(contentSource);
            DopingViewModel.ChangeContentSource(contentSource);
        }
    }
}
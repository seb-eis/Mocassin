using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel.DataControl;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel
{
    /// <summary>
    ///     The <see cref="ProjectGraphControlViewModel"/> for 
    /// </summary>
	public class LatticeModelControlViewModel : ProjectGraphControlViewModel
	{
		public BuildingBlockControlViewModel BlockControlViewModel { get; }

        public DopingCombinationControlViewModel DopingCombinationViewModel { get; }

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
		public override void ChangeContentSource(MocassinProjectGraph contentSource)
		{
			ContentSource = contentSource;
			BlockControlViewModel.ChangeContentSource(contentSource);
            DopingCombinationViewModel.ChangeContentSource(contentSource);
            DopingViewModel.ChangeContentSource(contentSource);
		}
	}
}
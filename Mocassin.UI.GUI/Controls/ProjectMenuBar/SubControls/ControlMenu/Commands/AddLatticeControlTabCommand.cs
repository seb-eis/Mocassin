using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.LatticeModel;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
	/// <summary>
	///		A <see cref="AddDefaultLayoutControlTabCommand"/> to add a new <see cref="LatticeModelControlView"/> as a work tab
	/// </summary>
	public class AddLatticeControlTabCommand : AddDefaultLayoutControlTabCommand
	{
		/// <inheritdoc />
		public AddLatticeControlTabCommand(IProjectAppControl projectControl)
			: base(projectControl)
		{
		}

		/// <inheritdoc />
		protected override ContentControl GetDataControl()
		{
			return new LatticeModelControlView {DataContext = new LatticeModelControlViewModel(ProjectControl)};
		}

		/// <inheritdoc />
		protected override string GetTabName()
		{
			return "Lattice Control";
		}
	}
}
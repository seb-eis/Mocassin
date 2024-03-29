﻿using System.Windows.Controls;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.StructureModel;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu.Commands
{
    /// <summary>
    ///     The <see cref="AddDefaultLayoutControlTabCommand" /> implementation to a default structure control tab
    /// </summary>
    public class AddStructureControlTabCommand : AddDefaultLayoutControlTabCommand
    {
        /// <inheritdoc />
        public AddStructureControlTabCommand(IProjectAppControl projectControl)
            : base(projectControl)
        {
        }

        /// <inheritdoc />
        protected override ContentControl GetDataControl() => new StructureModelControlView {DataContext = new StructureModelControlViewModel(ProjectControl)};

        /// <inheritdoc />
        protected override string GetTabName() => "Structure Control";
    }
}
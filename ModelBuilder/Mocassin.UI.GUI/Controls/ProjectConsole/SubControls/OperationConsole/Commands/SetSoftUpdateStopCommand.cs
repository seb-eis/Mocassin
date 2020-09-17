using System;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Controls.ProjectConsole.SubControls.OperationConsole.Commands
{
    /// <summary>
    ///     The <see cref="Command{T}" /> implementation to set the soft update stop of a operation report console
    /// </summary>
    public class SetSoftUpdateStopCommand : Command<bool>
    {
        /// <summary>
        ///     Get the <see cref="OperationReportConsoleViewModel" /> that the command targets
        /// </summary>
        private OperationReportConsoleViewModel ReportConsoleViewModel { get; }

        /// <summary>
        ///     Creates new <see cref="SetSoftUpdateStopCommand" /> for the passed <see cref="OperationReportConsoleViewModel" />
        /// </summary>
        /// <param name="reportConsoleViewModel"></param>
        public SetSoftUpdateStopCommand(OperationReportConsoleViewModel reportConsoleViewModel)
        {
            ReportConsoleViewModel = reportConsoleViewModel ?? throw new ArgumentNullException(nameof(reportConsoleViewModel));
        }

        /// <inheritdoc />
        public override void Execute(bool parameter)
        {
            ReportConsoleViewModel.IsSoftUpdateStop = parameter;
        }
    }
}
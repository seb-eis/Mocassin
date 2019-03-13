using Mocassin.Framework.Operations;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Base.ViewModels.Collections;
using Mocassin.UI.GUI.Controls.Base.ViewModels;

namespace Mocassin.UI.GUI.Controls.ProjectBrowser.SubControls.ReportBrowser
{
    /// <summary>
    ///     The <see cref="ViewModel" /> for <see cref="ReportBrowserView" />
    /// </summary>
    public class ReportBrowserViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="ObservableCollectionViewModel{T}" /> for the set of <see cref="IOperationReport" /> objects
        /// </summary>
        public ObservableCollectionViewModel<IOperationReport> OperationReportCollectionViewModel { get; }

        /// <inheritdoc />
        public ReportBrowserViewModel(IMocassinProjectControl mainProjectControl)
            : base(mainProjectControl)
        {
        }
    }
}
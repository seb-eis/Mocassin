using Mocassin.Framework.Operations;
using Mocassin.UI.GUI.Displays.Base;

namespace Mocassin.UI.GUI.Displays.ReportDisplay
{
    /// <summary>
    ///     View model for the basic text display of multiple <see cref="IOperationReport" /> interfaces
    /// </summary>
    public class ReportTextDisplayViewModel : ObservableCollectionViewModel
    {
        /// <summary>
        ///     Appends an <see cref="IOperationReport" /> to the display collection
        /// </summary>
        /// <param name="report"></param>
        public void AppendReport(IOperationReport report)
        {
            AppenCollectionItem(report?.ToString());
        }
    }
}
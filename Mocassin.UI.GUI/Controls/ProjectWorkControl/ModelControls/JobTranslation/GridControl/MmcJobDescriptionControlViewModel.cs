using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     Extended implementation of the <see cref="BaseJobDescriptionControlViewModel" /> for
    ///     <see cref="MmcJobDescriptionGraph" /> instances
    /// </summary>
    public class MmcJobDescriptionControlViewModel : BaseJobDescriptionControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="MmcJobDescriptionGraph" /> that the view model targets
        /// </summary>
        private MmcJobDescriptionGraph JobDescription { get; }

        /// <inheritdoc />
        public MmcJobDescriptionControlViewModel(MmcJobDescriptionGraph jobDescription)
            : base(jobDescription)
        {
            JobDescription = jobDescription;
        }
    }
}
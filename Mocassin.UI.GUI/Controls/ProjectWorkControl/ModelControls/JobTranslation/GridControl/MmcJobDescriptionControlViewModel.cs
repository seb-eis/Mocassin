using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     Extended implementation of the <see cref="BaseJobDescriptionControlViewModel" /> for
    ///     <see cref="MmcJobConfigData" /> instances
    /// </summary>
    public class MmcJobDescriptionControlViewModel : BaseJobDescriptionControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="MmcJobConfigData" /> that the view model targets
        /// </summary>
        private MmcJobConfigData JobDescription { get; }

        /// <summary>
        ///     Get or set the abort tolerance value on the target <see cref="MmcJobConfigData" />
        /// </summary>
        public double? BreakTolerance
        {
            get => double.TryParse(JobDescription.BreakTolerance, out var x) ? x : (double?) null;
            set
            {
                JobDescription.BreakTolerance = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the abort sequence length on the target <see cref="MmcJobConfigData" />
        /// </summary>
        public int? BreakSampleLength
        {
            get => int.TryParse(JobDescription.BreakSampleLength, out var x) ? x : (int?) null;
            set
            {
                JobDescription.BreakSampleLength = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the abort sample interval on the target <see cref="MmcJobConfigData" />
        /// </summary>
        public int? BreakSampleInterval
        {
            get => int.TryParse(JobDescription.BreakSampleInterval, out var x) ? x : (int?) null;
            set
            {
                JobDescription.BreakSampleInterval = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the result sample mcs on the target <see cref="MmcJobConfigData" />
        /// </summary>
        public int? ResultSampleMcs
        {
            get => int.TryParse(JobDescription.ResultSampleMcs, out var x) ? x : (int?) null;
            set
            {
                JobDescription.ResultSampleMcs = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <inheritdoc />
        public MmcJobDescriptionControlViewModel(MmcJobConfigData jobDescription)
            : base(jobDescription)
        {
            JobDescription = jobDescription;
        }
    }
}
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     Extended implementation of the <see cref="BaseJobDescriptionControlViewModel"/> for <see cref="KmcJobConfigData"/> instances
    /// </summary>
    public class KmcJobDescriptionControlViewModel : BaseJobDescriptionControlViewModel 
    {
        /// <summary>
        ///     Get the <see cref="KmcJobConfigData"/> that the view model targets
        /// </summary>
        private KmcJobConfigData JobDescription { get; }

        /// <summary>
        ///     Get or set the target MCSP of the targeted <see cref="KmcJobConfigData" />
        /// </summary>
        public int? PreRunMcsp
        {
            get => int.TryParse(JobDescription.PreRunMcsp, out var x) ? x : (int?) null;
            set => JobDescription.PreRunMcsp = value?.ToString();
        }

        /// <summary>
        ///     Get or set the electric field modulus in [V/m] of the targeted <see cref="KmcJobConfigData" />
        /// </summary>
        public double? ElectricFieldModulus
        {
            get => double.TryParse(JobDescription.ElectricFieldModulus, out var x) ? x : (double?) null;
            set => JobDescription.ElectricFieldModulus = value?.ToString();
        }

        /// <summary>
        ///     Get or set the fixed normalization probability of the targeted <see cref="KmcJobConfigData" />
        /// </summary>
        public double? NormalizationProbability
        {
            get => double.TryParse(JobDescription.NormalizationProbability, out var x) ? x : (double?) null;
            set => JobDescription.NormalizationProbability = value?.ToString();
        }

        /// <summary>
        ///     Get or set the max attempt frequency [Hz] of the targeted <see cref="KmcJobConfigData" />
        /// </summary>
        public double? MaxAttemptFrequency
        {
            get => double.TryParse(JobDescription.MaxAttemptFrequency, out var x) ? x : (double?) null;
            set => JobDescription.MaxAttemptFrequency = value?.ToString();
        }

        /// <inheritdoc />
        public KmcJobDescriptionControlViewModel(KmcJobConfigData jobDescription)
            : base(jobDescription)
        {
            JobDescription = jobDescription;
        }
    }
}
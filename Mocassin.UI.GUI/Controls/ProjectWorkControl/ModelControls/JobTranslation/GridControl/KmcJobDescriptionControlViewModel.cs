using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     Extended implementation of the <see cref="BaseJobDescriptionControlViewModel"/> for <see cref="KmcJobDescriptionGraph"/> instances
    /// </summary>
    public class KmcJobDescriptionControlViewModel : BaseJobDescriptionControlViewModel 
    {
        /// <summary>
        ///     Get the <see cref="KmcJobDescriptionGraph"/> that the view model targets
        /// </summary>
        private KmcJobDescriptionGraph JobDescription { get; }

        /// <summary>
        ///     Get or set the target MCSP of the targeted <see cref="KmcJobDescriptionGraph" />
        /// </summary>
        public int? PreRunMcsp
        {
            get => int.TryParse(JobDescription.PreRunMcsp, out var x) ? x : (int?) null;
            set => JobDescription.PreRunMcsp = value?.ToString();
        }

        /// <summary>
        ///     Get or set the electric field modulus in [V/m] of the targeted <see cref="KmcJobDescriptionGraph" />
        /// </summary>
        public double? ElectricFieldModulus
        {
            get => double.TryParse(JobDescription.ElectricFieldModulus, out var x) ? x : (double?) null;
            set => JobDescription.ElectricFieldModulus = value?.ToString();
        }

        /// <summary>
        ///     Get or set the fixed normalization probability of the targeted <see cref="KmcJobDescriptionGraph" />
        /// </summary>
        public double? NormalizationProbability
        {
            get => double.TryParse(JobDescription.NormalizationProbability, out var x) ? x : (double?) null;
            set => JobDescription.NormalizationProbability = value?.ToString();
        }

        /// <summary>
        ///     Get or set the max attempt frequency [Hz] of the targeted <see cref="KmcJobDescriptionGraph" />
        /// </summary>
        public double? MaxAttemptFrequency
        {
            get => double.TryParse(JobDescription.MaxAttemptFrequency, out var x) ? x : (double?) null;
            set => JobDescription.MaxAttemptFrequency = value?.ToString();
        }

        /// <inheritdoc />
        public KmcJobDescriptionControlViewModel(KmcJobDescriptionGraph jobDescription)
            : base(jobDescription)
        {
            JobDescription = jobDescription;
        }
    }
}
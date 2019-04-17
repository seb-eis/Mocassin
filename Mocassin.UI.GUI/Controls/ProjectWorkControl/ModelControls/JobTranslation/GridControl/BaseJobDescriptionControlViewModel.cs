using System;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.Xml.Jobs;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.JobTranslation.GridControl
{
    /// <summary>
    ///     The <see cref="ViewModelBase" /> for <see cref="BaseJobDescriptionControlView" /> that controls manipulation of
    ///     <see cref="JobDescriptionGraph" /> data
    /// </summary>
    public class BaseJobDescriptionControlViewModel : ViewModelBase
    {
        /// <summary>
        ///     Get the <see cref="JobDescriptionGraph" /> that the view model targets
        /// </summary>
        private JobDescriptionGraph JobDescription { get; }

        /// <summary>
        ///     Get or set the <see cref="SimulationJobInfoFlags" /> of the targeted <see cref="JobDescriptionGraph" />
        /// </summary>
        public SimulationJobInfoFlags? JobInfoFlags
        {
            get => Enum.TryParse<SimulationJobInfoFlags>(JobDescription.JobInfoFlags, out var x) ? x : (SimulationJobInfoFlags?) null;
            set
            {
                JobDescription.JobInfoFlags = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the target MCSP of the targeted <see cref="JobDescriptionGraph" />
        /// </summary>
        public int? TargetMcsp
        {
            get => int.TryParse(JobDescription.TargetMcsp, out var x) ? x : (int?) null;
            set
            {
                JobDescription.TargetMcsp = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the time limit <see cref="TimeSpan" /> of the targeted <see cref="JobDescriptionGraph" />
        /// </summary>
        public TimeSpan? TimeLimit
        {
            get => JobDescriptionGraph.ParseTimeString(JobDescription.TimeLimit);
            set
            {
                JobDescription.TimeLimit = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the simulation temperature in [K] of the targeted <see cref="JobDescriptionGraph" />
        /// </summary>
        public double? Temperature
        {
            get => double.TryParse(JobDescription.Temperature, out var x) ? x : (double?) null;
            set
            {
                JobDescription.Temperature = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Get or set the minimal success rate in [Hz] of the targeted <see cref="JobDescriptionGraph" />
        /// </summary>
        public double? MinimalSuccessRate
        {
            get => double.TryParse(JobDescription.MinimalSuccessRate, out var x) ? x : (double?) null;
            set
            {
                JobDescription.MinimalSuccessRate = value?.ToString();
                OnPropertyChanged();
            }
        }

        /// <summary>
        ///     Creates new <see cref="BaseJobDescriptionControlViewModel" /> for the passed <see cref="JobDescriptionGraph" />
        /// </summary>
        /// <param name="jobDescription"></param>
        public BaseJobDescriptionControlViewModel(JobDescriptionGraph jobDescription)
        {
            JobDescription = jobDescription ?? throw new ArgumentNullException(nameof(jobDescription));
        }
    }
}
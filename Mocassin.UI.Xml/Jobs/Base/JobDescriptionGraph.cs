using System;
using System.Xml;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     The serializable data object for creation of <see cref="JobConfiguration" /> objects
    /// </summary>
    [XmlRoot]
    public abstract class JobDescriptionGraph : ProjectObjectGraph
    {
        /// <summary>
        ///     Get or set additional job info flags
        /// </summary>
        [XmlAttribute("JobFlags")]
        public string JobInfoFlags { get; set; }

        /// <summary>
        ///     The number of target MCSP as a string
        /// </summary>
        [XmlAttribute("TargetMcsp")]
        public string TargetMcsp { get; set; }

        /// <summary>
        ///     Get or set the time limit of the simulation as a string
        /// </summary>
        [XmlAttribute("TimeLimit")]
        public string TimeLimit { get; set; }

        /// <summary>
        ///     Get or set the temperature value in [K] as a string
        /// </summary>
        [XmlAttribute("Temperature")]
        public string Temperature { get; set; }

        /// <summary>
        ///     Get or set the minimal success rate as a string
        /// </summary>
        [XmlAttribute("MinSuccessRate")]
        public string MinimalSuccessRate { get; set; }

        /// <summary>
        ///     Creates a <see cref="JobConfiguration" /> populated with specified properties using defaults from the passed
        ///     <see cref="ISimulation" /> where required
        /// </summary>
        /// <param name="baseSimulation"></param>
        /// <returns></returns>
        public JobConfiguration ToInternal(ISimulation baseSimulation)
        {
            var obj = GetPreparedInternal(baseSimulation);

            obj.JobInfoFlags |= JobInfoFlags is null
                ? 0
                : (SimulationJobInfoFlags) Enum.Parse(typeof(SimulationJobInfoFlags), JobInfoFlags);

            obj.TargetMcsp = string.IsNullOrWhiteSpace(TargetMcsp)
                ? baseSimulation.TargetMcsp
                : int.Parse(TargetMcsp);

            obj.TimeLimit = (long) (ParseTimeString(TimeLimit)?.TotalSeconds ?? baseSimulation.SaveRunTimeLimit.TotalSeconds);
            obj.Temperature = string.IsNullOrWhiteSpace(Temperature)
                ? baseSimulation.Temperature
                : double.Parse(Temperature);

            obj.MinimalSuccessRate = string.IsNullOrWhiteSpace(MinimalSuccessRate)
                ? baseSimulation.LowerSuccessRateLimit
                : double.Parse(MinimalSuccessRate);

            return obj;
        }

        /// <summary>
        ///     Creates a <see cref="JobConfiguration" /> populated with specified properties using defaults from the passed
        ///     base <see cref="JobConfiguration" /> where required
        /// </summary>
        /// <param name="baseConfiguration"></param>
        /// <returns></returns>
        public JobConfiguration ToInternal(JobConfiguration baseConfiguration)
        {
            var obj = GetPreparedInternal(baseConfiguration);

            obj.LatticeConfiguration = baseConfiguration.LatticeConfiguration ?? new LatticeConfiguration();

            obj.JobInfoFlags |= string.IsNullOrWhiteSpace(JobInfoFlags)
                ? baseConfiguration.JobInfoFlags
                : (SimulationJobInfoFlags) Enum.Parse(typeof(SimulationJobInfoFlags), JobInfoFlags);

            obj.TargetMcsp = string.IsNullOrWhiteSpace(TargetMcsp)
                ? baseConfiguration.TargetMcsp
                : int.Parse(TargetMcsp);

            obj.TimeLimit = (long) (ParseTimeString(TimeLimit)?.TotalSeconds ?? baseConfiguration.TimeLimit);

            obj.Temperature = string.IsNullOrWhiteSpace(Temperature)
                ? baseConfiguration.Temperature
                : double.Parse(Temperature);

            obj.MinimalSuccessRate = string.IsNullOrWhiteSpace(MinimalSuccessRate)
                ? baseConfiguration.MinimalSuccessRate
                : double.Parse(MinimalSuccessRate);

            return obj;
        }

        /// <summary>
        ///     Get a prepared <see cref="JobConfiguration" /> that has all values of the implementing type set using the defaults
        ///     from the passed <see cref="ISimulation" /> where required
        /// </summary>
        /// <param name="baseSimulation"></param>
        /// <returns></returns>
        protected abstract JobConfiguration GetPreparedInternal(ISimulation baseSimulation);

        /// <summary>
        ///     Get a prepared <see cref="JobConfiguration" /> that has all values of the implementing type set using the defaults
        ///     from the passed base <see cref="JobConfiguration" /> where required
        /// </summary>
        /// <param name="baseConfiguration"></param>
        /// <returns></returns>
        protected abstract JobConfiguration GetPreparedInternal(JobConfiguration baseConfiguration);

        /// <summary>
        ///     Parses the passed time string under consideration of both ISO8601 and current culture settings. If both fail a
        ///     default value is set
        /// </summary>
        /// <param name="s"></param>
        /// <param name="defaultHours"></param>
        /// <returns></returns>
        public static TimeSpan? ParseTimeString(string s, int defaultHours = 24)
        {
            if (TimeSpan.TryParse(s, out var timeLimit)) return timeLimit;
            try
            {
                timeLimit = XmlConvert.ToTimeSpan(s);
                return timeLimit;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
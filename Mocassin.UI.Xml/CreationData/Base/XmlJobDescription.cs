using System;
using System.Xml.Serialization;
using Mocassin.Model.Simulations;
using Mocassin.Model.Translator.Jobs;

namespace Mocassin.UI.Xml.CreationData
{
    /// <summary>
    ///     The serializable data object for creation of <see cref="JobConfiguration" /> objects
    /// </summary>
    [XmlRoot]
    public abstract class XmlJobDescription
    {
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
        ///     Get or set the random number generator seed as a string
        /// </summary>
        [XmlAttribute("RngSeed")]
        public string RngSeed { get; set; }

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
            obj.TargetMcsp = TargetMcsp is null 
                ? baseSimulation.TargetMcsp 
                : int.Parse(TargetMcsp);

            obj.TimeLimit = (TimeLimit is null ? baseSimulation.SaveRunTimeLimit.Ticks : TimeSpan.Parse(TimeLimit).Ticks) /
                            TimeSpan.TicksPerSecond;

            obj.Temperature = Temperature is null 
                ? baseSimulation.Temperature 
                : double.Parse(Temperature);

            obj.MinimalSuccessRate = MinimalSuccessRate is null 
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

            obj.TargetMcsp = TargetMcsp is null 
                ? baseConfiguration.TargetMcsp 
                : int.Parse(TargetMcsp);

            obj.TimeLimit = TimeLimit is null
                ? baseConfiguration.TimeLimit
                : TimeSpan.Parse(TimeLimit).Ticks / TimeSpan.TicksPerSecond;

            obj.Temperature = Temperature is null 
                ? baseConfiguration.Temperature 
                : double.Parse(Temperature);

            obj.MinimalSuccessRate = MinimalSuccessRate is null 
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
    }
}
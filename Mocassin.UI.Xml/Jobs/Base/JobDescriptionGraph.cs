using System;
using System.Xml;
using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
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
        private string executionFlags;
        private string targetMcsp;
        private string timeLimit;
        private string temperature;
        private string minimalSuccessRate;
        private LatticeConfigurationGraph latticeConfiguration;
        private string instruction;

        /// <summary>
        ///     Get or set additional job info flags
        /// </summary>
        [XmlAttribute("JobFlags")]
        public string ExecutionFlags
        {
            get => executionFlags;
            set => SetProperty(ref executionFlags, value);
        }

        /// <summary>
        ///     The number of target MCSP as a string
        /// </summary>
        [XmlAttribute("TargetMcsp")]
        public string TargetMcsp
        {
            get => targetMcsp;
            set => SetProperty(ref targetMcsp, value);
        }

        /// <summary>
        ///     Get or set the time limit of the simulation as a string
        /// </summary>
        [XmlAttribute("TimeLimit")]
        public string TimeLimit
        {
            get => timeLimit;
            set => SetProperty(ref timeLimit, value);
        }

        /// <summary>
        ///     Get or set the temperature value in [K] as a string
        /// </summary>
        [XmlAttribute("Temperature")]
        public string Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }

        /// <summary>
        ///     Get or set the minimal success rate as a string
        /// </summary>
        [XmlAttribute("MinSuccessRate")]
        public string MinimalSuccessRate
        {
            get => minimalSuccessRate;
            set => SetProperty(ref minimalSuccessRate, value);
        }

        /// <summary>
        ///     Get or set the <see cref="LatticeConfigurationGraph"/> of the job
        /// </summary>
        [XmlElement("LatticeConfiguration")]
        public LatticeConfigurationGraph LatticeConfiguration
        {
            get => latticeConfiguration;
            set => SetProperty(ref latticeConfiguration, value);
        }

        /// <summary>
        ///     Get or set a <see cref="string"/> that describes additional instructions
        /// </summary>
        [XmlElement("Instruction")]
        public string Instruction
        {
            get => instruction;
            set => SetProperty(ref instruction, value);
        }

        /// <inheritdoc />
        protected JobDescriptionGraph()
        {
            LatticeConfiguration = new LatticeConfigurationGraph();
        }

        /// <summary>
        ///     Creates a <see cref="JobConfiguration" /> populated with specified properties using defaults from the passed
        ///     <see cref="ISimulation" /> where required
        /// </summary>
        /// <param name="baseSimulation"></param>
        /// <returns></returns>
        public JobConfiguration ToInternal(ISimulation baseSimulation)
        {
            var obj = GetPreparedInternal(baseSimulation);

            obj.ExecutionFlags |= ExecutionFlags is null
                ? SimulationExecutionFlags.None
                : (SimulationExecutionFlags) Enum.Parse(typeof(SimulationExecutionOverwriteFlags), ExecutionFlags);

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

            obj.Instruction = Instruction;
            obj.ConfigName = Name;

            return obj;
        }

        /// <summary>
        ///     Creates a <see cref="JobConfiguration" /> populated with specified properties using defaults from the passed
        ///     base <see cref="JobConfiguration" /> where required
        /// </summary>
        /// <param name="baseConfiguration"></param>
        /// <param name="modelProject"></param>
        /// <param name="configIndex"></param>
        /// <returns></returns>
        public JobConfiguration ToInternal(JobConfiguration baseConfiguration, IModelProject modelProject, int configIndex)
        {
            if (modelProject == null) throw new ArgumentNullException(nameof(modelProject));

            var obj = GetPreparedInternal(baseConfiguration);
            obj.ConfigIndex = configIndex;

            obj.LatticeConfiguration = baseConfiguration.LatticeConfiguration ?? new LatticeConfiguration();

            obj.ExecutionFlags |= string.IsNullOrWhiteSpace(ExecutionFlags)
                ? baseConfiguration.ExecutionFlags
                : (SimulationExecutionFlags) Enum.Parse(typeof(SimulationExecutionFlags), ExecutionFlags);

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

            obj.Instruction = string.IsNullOrWhiteSpace(Instruction)
                ? baseConfiguration.Instruction
                : Instruction;

            obj.LatticeConfiguration = LatticeConfiguration.ToInternal(modelProject);
            obj.ConfigName = Name;

            return obj;
        }

        /// <summary>
        ///     Copies the base data of the <see cref="JobDescriptionGraph"/> to the target
        /// </summary>
        /// <param name="other"></param>
        protected void CopyBaseDataTo(JobDescriptionGraph other)
        {
            other.ExecutionFlags = ExecutionFlags;
            other.MinimalSuccessRate = MinimalSuccessRate;
            other.TargetMcsp = TargetMcsp;
            other.Temperature = Temperature;
            other.TimeLimit = TimeLimit;
            other.LatticeConfiguration = LatticeConfiguration.Duplicate();
            other.Name = Name;
            other.Instruction = Instruction;
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
        /// <returns></returns>
        public static TimeSpan? ParseTimeString(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return null;
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
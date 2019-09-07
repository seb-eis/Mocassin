﻿using System;
using System.Xml;
using System.Xml.Serialization;
using Mocassin.Model.Basic;
using Mocassin.Model.Simulations;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.SimulationModel
{
    /// <summary>
    ///     Serializable data object base for <see cref="Mocassin.Model.Simulations.ISimulation" /> model object creation
    /// </summary>
    [XmlRoot]
    public abstract class SimulationBaseGraph : ModelObjectGraph
    {
        private string customRngSeed;
        private double temperature = 1000;
        private int targetMcsp = 1000;
        private int simulationBlockCount = 100;
        private string saveRunTimeLimit = "P1Y";
        private double lowerSuccessRateLimit;
        private int jobCount = 12;

        /// <summary>
        ///     Get or set the custom rng seed for the simulation
        /// </summary>
        [XmlAttribute("RngSeed")]
        public string CustomRngSeed
        {
            get => customRngSeed;
            set => SetProperty(ref customRngSeed, value);
        }

        /// <summary>
        ///     Get or set the base simulation temperature in [K]
        /// </summary>
        [XmlAttribute("Temperature")]
        public double Temperature
        {
            get => temperature;
            set => SetProperty(ref temperature, value);
        }

        /// <summary>
        ///     Get or set the target MCSP value
        /// </summary>
        [XmlAttribute("Mcsp")]
        public int TargetMcsp
        {
            get => targetMcsp;
            set => SetProperty(ref targetMcsp, value);
        }

        /// <summary>
        ///     Get or set the number of simulation blocks
        /// </summary>
        [XmlAttribute("SimulationBlocks")]
        public int SimulationBlockCount
        {
            get => simulationBlockCount;
            set => SetProperty(ref simulationBlockCount, value);
        }

        /// <summary>
        ///     Get or set the hard run limit as a time span
        /// </summary>
        [XmlAttribute("TimeLimit")]
        public string SaveRunTimeLimit
        {
            get => saveRunTimeLimit;
            set => SetProperty(ref saveRunTimeLimit, value);
        }

        /// <summary>
        ///     Get or set the hard lower success rate limit in [Hz]
        /// </summary>
        [XmlAttribute("MinSuccessRate")]
        public double LowerSuccessRateLimit
        {
            get => lowerSuccessRateLimit;
            set => SetProperty(ref lowerSuccessRateLimit, value);
        }

        /// <summary>
        ///     Get or set the basic job count multiplier
        /// </summary>
        [XmlAttribute("JobCount")]
        public int JobCount
        {
            get => jobCount;
            set => SetProperty(ref jobCount, value);
        }

        /// <inheritdoc />
        protected override ModelObject GetModelObjectInternal()
        {
            if (!TimeSpan.TryParse(SaveRunTimeLimit, out var timeLimit))
            {
                try
                {
                    timeLimit = XmlConvert.ToTimeSpan(SaveRunTimeLimit);
                }
                catch (Exception)
                {
                    timeLimit = TimeSpan.FromDays(365);
                }
            }

            var simulationBase = GetPreparedSpecifiedSimulation();
            simulationBase.Name = Name ?? Guid.NewGuid().ToString();
            simulationBase.CustomRngSeed = CustomRngSeed;
            simulationBase.Temperature = Temperature;
            simulationBase.TargetMcsp = TargetMcsp;
            simulationBase.WriteOutCount = SimulationBlockCount;
            simulationBase.SaveRunTimeLimit = timeLimit;
            simulationBase.LowerSuccessRateLimit = LowerSuccessRateLimit;
            simulationBase.JobCount = JobCount;
            return simulationBase;
        }

        /// <summary>
        ///     Get the specified simulation object from the implementing instance
        /// </summary>
        /// <returns></returns>
        protected abstract SimulationBase GetPreparedSpecifiedSimulation();
    }
}
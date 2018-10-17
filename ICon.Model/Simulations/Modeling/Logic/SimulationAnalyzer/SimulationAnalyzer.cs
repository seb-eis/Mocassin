using System;
using System.Collections.Generic;
using System.Text;
using Mocassin.Model.Simulations;
using System.Linq;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Simulations
{
    /// <summary>
    /// Analyzer for simulations that provides basic information about its behavior
    /// </summary>
    public class SimulationAnalyzer
    {
        protected IModelProject ModelProject { get; set; }

        public SimulationAnalyzer(IModelProject modelProject)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
        }

        /// <summary>
        /// Create the simulation position mobility information for the passed kinetic simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public IList<KineticMobilityInfo> CreateSimulationMobilityInfos(IKineticSimulation simulation)
        {
            return CreateSimulationMobilityInfos(simulation).Cast<KineticMobilityInfo>().ToList();
        }

        /// <summary>
        /// Creates the simulation position mobility information for the passed metropolis simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public IList<MetropolisMobilityInfo> CreateSimulationMobilityInfos(IMetropolisSimulation simulation)
        {
            return CreateSimulationMobilityInfos(simulation).Cast<MetropolisMobilityInfo>().ToList();
        }

        /// <summary>
        /// Creates a set of particles for each unit cell position that describes the mobile species on this position in the passed simulation
        /// </summary>
        /// <param name="simulation"></param>
        /// <returns></returns>
        protected IEnumerable<PositionMobilityInfo> CreateUnitCellMobilityInfo<T1>(T1 simulation) where T1 : ISimulation
        {
            var manager = ModelProject.GetManager<IStructureManager>();
            foreach (var cellPosition in manager.QueryPort.Query(data => data.GetUnitCellPositions()))
            {
                var mobilityInfo = PositionMobilityInfo.CreateForSimulation(simulation);
                mobilityInfo.UnitCellPosition = cellPosition;
                PopulatePositionMobilityInfo(mobilityInfo);
                yield return mobilityInfo;
            }
        }

        /// <summary>
        /// Adds the transition and particle information to the mobility info depending on the type
        /// </summary>
        /// <param name="mobilityInfo"></param>
        protected void PopulatePositionMobilityInfo<T1>(T1 mobilityInfo) where T1 : PositionMobilityInfo
        {
            if (mobilityInfo is KineticMobilityInfo kineticMobilityInfo)
            {
                kineticMobilityInfo.PositionTransitions = GetApplicableTransitions(kineticMobilityInfo);
                kineticMobilityInfo.MobileParticleSet = GetMobileParticleSet(kineticMobilityInfo);
            }

            if (mobilityInfo is MetropolisMobilityInfo metropolisMobilityInfo)
            {
                metropolisMobilityInfo.PositionTransitions = GetApplicableTransitions(metropolisMobilityInfo);
                metropolisMobilityInfo.MobileParticleSet = GetMobileParticleSet(metropolisMobilityInfo);
            }
        }

        
        protected IList<IKineticTransition> GetApplicableTransitions(KineticMobilityInfo mobilityInfo)
        {
            return null;
        }

        protected IList<IMetropolisTransition> GetApplicableTransitions(MetropolisMobilityInfo mobilityInfo)
        {
            return null;
        }

        protected IParticleSet GetMobileParticleSet(KineticMobilityInfo mobilityInfo)
        {
            return null;  
        }

        protected IParticleSet GetMobileParticleSet(MetropolisMobilityInfo mobilityInfo)
        {
            return null;
        }

        /// <summary>
        /// Get the mobile species of the passed transition rule if its active, else returns null
        /// </summary>
        /// <param name="transitionRule"></param>
        /// <returns></returns>
        protected IParticle GetMobileOfTransitionRule(ITransitionRule transitionRule)
        {
            if (transitionRule.RuleFlags.HasFlag(RuleFlags.Active))
            {
                return transitionRule.GetStartStateOccupation().First();
            }
            return null;
        }
    }
}

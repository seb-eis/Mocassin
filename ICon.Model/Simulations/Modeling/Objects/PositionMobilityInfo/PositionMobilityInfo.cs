using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Model.Transitions;
using System;
using System.Collections.Generic;
using System.Text;


namespace ICon.Model.Simulations
{
    /// <summary>
    /// Untyped position mobility info that describes which species are mobile on a position
    /// </summary>
    public abstract class PositionMobilityInfo
    {
        /// <summary>
        /// The unit cell position the mobility info is valid for
        /// </summary>
        public IUnitCellPosition UnitCellPosition { get; set; }

        /// <summary>
        /// The particle set that describes which species are mobile in this context
        /// </summary>
        public IParticleSet MobileParticleSet { get; set; }

        /// <summary>
        /// Create a position mobility info matching te passed simulation type
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="simulation"></param>
        /// <returns></returns>
        public static PositionMobilityInfo CreateForSimulation<T1>(T1 simulation) where T1 : ISimulationBase
        {
            if (simulation is IKineticSimulation kineticSimulation)
            {
                return new KineticMobilityInfo() { Simulation = kineticSimulation };
            }
            if (simulation is IMetropolisSimulation metropolisSimulation)
            {
                return new MetropolisMobilityInfo() { Simulation = metropolisSimulation };
            }
            throw new ArgumentException("Simulation type is not supported");
        }
    }

    /// <summary>
    /// Describes the mobility information for a unit cell position that results in a specific simulation
    /// </summary>
    public class PositionMobilityInfo<TSimulation, TTransition> : PositionMobilityInfo where TSimulation : ISimulationBase
    {
        /// <summary>
        /// The simulation the mobility info is valid for
        /// </summary>
        public TSimulation Simulation { get; set; }

        /// <summary>
        /// The list of transitions that can occure on this position
        /// </summary>
        public IList<TTransition> PositionTransitions { get; set; }
    }

    public class KineticMobilityInfo : PositionMobilityInfo<IKineticSimulation, IKineticTransition>
    {

    }

    public class MetropolisMobilityInfo : PositionMobilityInfo<IMetropolisSimulation, IMetropolisTransition>
    {

    }
}

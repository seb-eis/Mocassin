using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Represents the abstract basic description of a transition without geometric information
    /// </summary>
    public interface IAbstractTransition : IModelObject, IEquatable<IAbstractTransition>
    {
        /// <summary>
        /// Get the number of states of the transition
        /// </summary>
        int StateCount { get; }

        /// <summary>
        /// Get the number of connectors. Always has to be StateCount - 1
        /// </summary>
        int ConnectorCount { get; }

        /// <summary>
        /// The name of the transition description
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Flag if the abstract transition can only describe a metropolis exchange (State count is 2)
        /// </summary>
        bool IsMetropolis { get; }

        /// <summary>
        /// Get the state exchange group for each step of the transition
        /// </summary>
        IEnumerable<IStateExchangeGroup> GetStateExchangeGroups();

        /// <summary>
        /// Get the set of step connectors that describe the position connection
        /// </summary>
        IEnumerable<ConnectorType> GetConnectorSequence();
    }
}

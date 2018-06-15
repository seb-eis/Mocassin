using System;
using ICon.Framework.Operations;

using ICon.Model.ProjectServices;
using ICon.Model.Basic;

namespace ICon.Model.Lattices
{
    /// <summary>
    /// Validation service for lattice related model objects that checks new lattice related model object inputs
    /// </summary>
    public class LatticeValidationService : ValidationService<ILatticeDataPort>
    {
        /// <summary>
        /// The basic Lattice settings object that defines all data constraints
        /// </summary>
        protected BasicLatticeSettings Settings { get; set; }

        /// <summary>
        /// Create new Lattice validation service that uses the provided project service and settings object
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="settings"></param>
        public LatticeValidationService(IProjectServices projectServices, BasicLatticeSettings settings) : base(projectServices)
        {
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

    }
}

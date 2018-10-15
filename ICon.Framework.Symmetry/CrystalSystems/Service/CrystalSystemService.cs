using System;
using System.Collections.Generic;
using System.Text;
using ICon.Mathematics.Coordinates;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Symmetry.CrystalSystems
{
    public class CrystalSystemService : ICrystalSystemService
    {
        /// <summary>
        /// The crystal system provide for loading
        /// </summary>
        private ICrystalSystemProvider CrystalSystemProvider { get; set; }

        /// <summary>
        /// Access to the currently loaded crystal system
        /// </summary>
        public CrystalSystem CrystalSystem { get; protected set; }

        /// <summary>
        /// Get the vector transformer that hanldes coordinate system transformations with the tolerance of the crytsal system
        /// </summary>
        public VectorTransformer VectorTransformer { get; protected set; }

        /// <summary>
        /// Defines the tolerance range used for equality comparisons in the spherical coordinate transformations
        /// </summary>
        public double ToleranceRange { get; protected set; }

        /// <summary>
        /// Create new crystal system service from the crystal system provider and vector transformer
        /// </summary>
        /// <param name="crystalSystemProvider"></param>
        public CrystalSystemService(ICrystalSystemProvider crystalSystemProvider, double toleranceRange)
        {
            CrystalSystemProvider = crystalSystemProvider ?? throw new ArgumentNullException(nameof(crystalSystemProvider));
            ToleranceRange = toleranceRange;
            CrystalSystem = GetDefaultSystem();
        }

        /// <summary>
        /// Tries to the set parameter set, if successful the vector encoder is updated
        /// </summary>
        /// <param name="parameterSet"></param>
        /// <returns></returns>
        public bool TrySetParameters(CrystalParameterSet parameterSet)
        {
            if (CrystalSystem.TrySetParameters(parameterSet))
            {
                UpdateVectorTransformer();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Get the default crystal system (Triclinic)
        /// </summary>
        /// <returns></returns>
        public CrystalSystem GetDefaultSystem()
        {
            return CrystalSystemProvider.Create(0, "None");
        }

        /// <summary>
        /// Load a new crystal system from the provided that matches the provided space group (Returns false if already loaded)
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public bool LoadNewSystem(ISpaceGroup group)
        {
            if (group == null)
            {
                throw new ArgumentNullException(nameof(group));
            }
            return LoadIfDifferentSystem(CrystalSystemProvider.Create(group));
        }

        /// <summary>
        /// Load a new crystal system from the provided that matches the system index and variation name
        /// </summary>
        /// <param name="SystemIndex"></param>
        /// <param name="VariationName"></param>
        /// <returns></returns>
        public bool LoadNewSystem(int systemIndex, string variationName)
        {
            if (variationName == null)
            {
                throw new ArgumentNullException(nameof(variationName));
            }
            return LoadIfDifferentSystem(CrystalSystemProvider.Create(systemIndex, variationName));
        }

        /// <summary>
        /// Sets a new crystal system if it is not equal to the currently set one
        /// </summary>
        /// <param name="newSystem"></param>
        /// <returns></returns>
        protected bool LoadIfDifferentSystem(CrystalSystem newSystem)
        {
            if (newSystem.SystemID == CrystalSystem.SystemID && newSystem.Variation == CrystalSystem.Variation)
            {
                return false;
            }
            CrystalSystem = newSystem;
            return true;
        }

        /// <summary>
        /// Creates the vector transformer using the currently loaded crystal system
        /// </summary>
        /// <returns></returns>
        public VectorTransformer CreateVectorTransformer()
        {
            return new VectorTransformer(CrystalSystem.CreateCoordinateSystem(), SphericalCoordinateSystem3D.CreateIsoSystem(ToleranceRange));
        }

        /// <summary>
        /// Triggers an update of the vector transfromer
        /// </summary>
        public void UpdateVectorTransformer()
        {
            VectorTransformer = CreateVectorTransformer();
        }

        /// <summary>
        /// Get a copy of the currently active parameter set
        /// </summary>
        /// <returns></returns>
        public CrystalParameterSet GetCurrentParameterSet()
        {
            return new CrystalParameterSet()
            {
                ParamA = CrystalSystem.ParamA.Value,
                ParamB = CrystalSystem.ParamB.Value,
                ParamC = CrystalSystem.ParamC.Value,
                Alpha = CrystalSystem.Alpha.Value,
                Beta = CrystalSystem.Beta.Value,
                Gamma = CrystalSystem.Gamma.Value
            };
        }
    }
}

using System;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Symmetry.CrystalSystems
{
    public class CrystalSystemService : ICrystalSystemService
    {
        /// <summary>
        ///     The crystal system provide for loading
        /// </summary>
        private ICrystalSystemSource CrystalSystemSource { get; }

        /// <inheritdoc />
        public CrystalSystem CrystalSystem { get; protected set; }

        /// <inheritdoc />
        public IVectorTransformer VectorTransformer { get; protected set; }

        /// <summary>
        ///     Defines the tolerance range used for equality comparisons in the spherical coordinate transformations
        /// </summary>
        public double ToleranceRange { get; protected set; }

        /// <summary>
        ///     Create new crystal system service from the crystal system provider and vector transformer
        /// </summary>
        /// <param name="crystalSystemSource"></param>
        /// <param name="toleranceRange"></param>
        public CrystalSystemService(ICrystalSystemSource crystalSystemSource, double toleranceRange)
        {
            CrystalSystemSource = crystalSystemSource ?? throw new ArgumentNullException(nameof(crystalSystemSource));
            ToleranceRange = toleranceRange;
            CrystalSystem = GetDefaultSystem();
        }

        /// <inheritdoc />
        public bool TrySetParameters(CrystalParameterSet parameterSet)
        {
            if (!CrystalSystem.TrySetParameters(parameterSet))
                return false;

            UpdateVectorTransformer();
            return true;
        }

        /// <summary>
        ///     Get the default crystal system (Triclinic)
        /// </summary>
        /// <returns></returns>
        public CrystalSystem GetDefaultSystem()
        {
            return CrystalSystemSource.Create(0, "None");
        }

        /// <inheritdoc />
        public bool LoadNewSystem(ISpaceGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return LoadIfDifferentSystem(CrystalSystemSource.Create(group));
        }

        /// <inheritdoc />
        public CrystalSystem GetSystem(ISpaceGroup group)
        {
            if (group == null)
                throw new ArgumentNullException(nameof(group));

            return CrystalSystemSource.Create(group);
        }

        /// <inheritdoc />
        public bool LoadNewSystem(int systemIndex, string variationName)
        {
            if (variationName == null)
                throw new ArgumentNullException(nameof(variationName));

            return LoadIfDifferentSystem(CrystalSystemSource.Create(systemIndex, variationName));
        }

        /// <summary>
        ///     Sets a new crystal system if it is not equal to the currently set one
        /// </summary>
        /// <param name="newSystem"></param>
        /// <returns></returns>
        protected bool LoadIfDifferentSystem(CrystalSystem newSystem)
        {
            if (newSystem.SystemId == CrystalSystem.SystemId && newSystem.Variation == CrystalSystem.Variation)
                return false;

            CrystalSystem = newSystem;
            return true;
        }

        /// <summary>
        ///     Creates the vector transformer using the currently loaded crystal system
        /// </summary>
        /// <returns></returns>
        public IVectorTransformer CreateVectorTransformer()
        {
            return new VectorTransformer(CrystalSystem.CreateCoordinateSystem(),
                SphericalCoordinateSystem3D.CreateIsoSystem(ToleranceRange));
        }

        /// <summary>
        ///     Triggers an update of the vector transformer
        /// </summary>
        public void UpdateVectorTransformer()
        {
            VectorTransformer = CreateVectorTransformer();
        }

        /// <inheritdoc />
        public CrystalParameterSet GetCurrentParameterSet()
        {
            return new CrystalParameterSet
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
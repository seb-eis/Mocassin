using System;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Symmetry.SpaceGroups;

namespace Mocassin.Symmetry.CrystalSystems
{
    /// <inheritdoc />
    public class CrystalSystemService : ICrystalSystemService
    {
        /// <summary>
        ///     The <see cref="ICrystalSystemSource" /> that supplies <see cref="CrystalSystem" /> instances
        /// </summary>
        private ICrystalSystemSource CrystalSystemSource { get; }

        /// <inheritdoc />
        public CrystalSystem ActiveCrystalSystem { get; protected set; }

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
            ActiveCrystalSystem = GetDefaultSystem();
        }

        /// <inheritdoc />
        public bool TrySetParameters(CrystalParameterSet parameterSet)
        {
            if (!ActiveCrystalSystem.TrySetParameterValues(parameterSet))
                return false;

            UpdateVectorTransformer();
            return true;
        }

        /// <inheritdoc />
        public bool LoadNewSystem(ISpaceGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            return LoadIfDifferentSystem(CrystalSystemSource.GetSystem(group));
        }

        /// <inheritdoc />
        public CrystalSystem GetSystem(ISpaceGroup group)
        {
            if (group == null) throw new ArgumentNullException(nameof(group));
            return CrystalSystemSource.GetSystem(group);
        }

        /// <inheritdoc />
        public bool LoadNewSystem(CrystalSystemIdentification systemIdentification) =>
            LoadIfDifferentSystem(CrystalSystemSource.GetSystem(systemIdentification));

        /// <inheritdoc />
        public CrystalParameterSet CopyCurrentParameterSet() =>
            new CrystalParameterSet
            {
                ParamA = ActiveCrystalSystem.ParamA.Value,
                ParamB = ActiveCrystalSystem.ParamB.Value,
                ParamC = ActiveCrystalSystem.ParamC.Value,
                Alpha = ActiveCrystalSystem.Alpha.Value,
                Beta = ActiveCrystalSystem.Beta.Value,
                Gamma = ActiveCrystalSystem.Gamma.Value
            };

        /// <summary>
        ///     Get the default crystal system (Triclinic)
        /// </summary>
        /// <returns></returns>
        public CrystalSystem GetDefaultSystem() => CrystalSystemSource.GetSystem(CrystalSystemIdentification.Triclinic);

        /// <summary>
        ///     Sets a new crystal system if it is not equal to the currently set one
        /// </summary>
        /// <param name="newSystem"></param>
        /// <returns></returns>
        protected bool LoadIfDifferentSystem(CrystalSystem newSystem)
        {
            if (newSystem.SystemType == ActiveCrystalSystem.SystemType && newSystem.SystemVariation == ActiveCrystalSystem.SystemVariation)
                return false;

            ActiveCrystalSystem = newSystem;
            return true;
        }

        /// <summary>
        ///     Creates the vector transformer using the currently loaded crystal system
        /// </summary>
        /// <returns></returns>
        public IVectorTransformer CreateVectorTransformer() =>
            new VectorTransformer(ActiveCrystalSystem.CreateCoordinateSystem(),
                SphericalCoordinateSystem3D.CreateIsoSystem(ToleranceRange));

        /// <summary>
        ///     Triggers an update of the vector transformer
        /// </summary>
        public void UpdateVectorTransformer()
        {
            VectorTransformer = CreateVectorTransformer();
        }
    }
}
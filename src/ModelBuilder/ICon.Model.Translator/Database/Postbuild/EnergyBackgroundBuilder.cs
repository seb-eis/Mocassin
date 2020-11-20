using System;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.Database.Postbuild
{
    /// <summary>
    ///     Builder that creates <see cref="EnergyBackgroundEntity" /> for <see cref="SimulationJobModel" /> instances based on
    ///     energy provider functions
    /// </summary>
    public class EnergyBackgroundBuilder
    {
        /// <summary>
        ///     The lattice size in A direction
        /// </summary>
        public int SizeA { get; }

        /// <summary>
        ///     The lattice size in B direction
        /// </summary>
        public int SizeB { get; }

        /// <summary>
        ///     The lattice size in C direction
        /// </summary>
        public int SizeC { get; }

        /// <summary>
        ///     Creates a new <see cref="EnergyBackgroundBuilder"/> for a specific supercell size
        /// </summary>
        /// <param name="sizeA"></param>
        /// <param name="sizeB"></param>
        /// <param name="sizeC"></param>
        public EnergyBackgroundBuilder(int sizeA, int sizeB, int sizeC)
        {
            SizeA = sizeA;
            SizeB = sizeB;
            SizeC = sizeC;
        }

        /// <summary>
        ///     Builds an <see cref="EnergyBackgroundEntity"/> using the provided <see cref="IProjectModelContext"/> and energy provider for <see cref="Cartesian3D"/> vectors
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="energyFunc"></param>
        /// <returns></returns>
        public EnergyBackgroundEntity Build(IProjectModelContext modelContext, Func<IParticle, Cartesian3D, double> energyFunc)
        {
            var transformedEnergyFunc = TransformEnergyFunction(modelContext, energyFunc);
            return Build(modelContext, transformedEnergyFunc);
        }

        /// <summary>
        /// Builds an <see cref="EnergyBackgroundEntity"/> using the provided <see cref="IProjectModelContext"/> and energy provider for <see cref="Fractional3D"/> vectors
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="energyFunc"></param>
        /// <returns></returns>
        public EnergyBackgroundEntity Build(IProjectModelContext modelContext, Func<IParticle, Fractional3D, double> energyFunc)
        {
            var transformedEnergyFunc = TransformEnergyFunction(modelContext, energyFunc);
            return Build(modelContext, transformedEnergyFunc);
        }

        /// <summary>
        /// Builds an <see cref="EnergyBackgroundEntity"/> using the provided <see cref="IProjectModelContext"/> and energy provider for <see cref="Vector4I"/> vectors
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="energyFunc"></param>
        /// <returns></returns>
        public EnergyBackgroundEntity Build(IProjectModelContext modelContext, Func<IParticle, Vector4I, double> energyFunc)
        {
            var particles = modelContext.ModelProject.DataTracker.MapObjects<IParticle>();
            var positionCount = modelContext.ModelProject
                                            .Manager<IStructureManager>().DataAccess
                                            .Query(x => x.GetLinearizedExtendedPositionCount());
            var rawResult = CreateRawArray(positionCount, particles.Length);

            for (var a = 0; a < SizeA; a++)
            {
                for (var b = 0; b < SizeB; b++)
                {
                    for (var c = 0; c < SizeC; c++)
                    {
                        for (var p = 0; p < positionCount; p++)
                        {
                            for (var particleId = 1; particleId < particles.Length; particleId++)
                            {
                                var particle = particles[particleId];
                                var vector4 = new Vector4I(a, b, c, p);
                                rawResult[a, b, c, p, particleId] = energyFunc.Invoke(particle, vector4);
                            }
                        }
                    }
                }
            }

            return new EnergyBackgroundEntity(rawResult);
        }

        /// <summary>
        ///     Provides a new zero initialized 5D <see cref="double"/> array of correct size
        /// </summary>
        /// <param name="positionCount"></param>
        /// <param name="particleCount"></param>
        /// <returns></returns>
        private double[,,,,] CreateRawArray(int positionCount, int particleCount) => new double[SizeA, SizeB, SizeC, positionCount, particleCount];

        /// <summary>
        ///     Transforms the provided energy provider function for <see cref="Cartesian3D"/> to use <see cref="Vector4I"/> data
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="energyFunc"></param>
        /// <returns></returns>
        private static Func<IParticle, Vector4I, double> TransformEnergyFunction(IProjectModelContext modelContext, Func<IParticle, Cartesian3D, double> energyFunc)
        {
            var vectorEncoder = modelContext.ModelProject
                                            .Manager<IStructureManager>().DataAccess
                                            .Query(x => x.GetVectorEncoder());

            double ProvideEnergy(IParticle particle, Vector4I vector4)
            {
                vectorEncoder.DecodeUnchecked(vector4, out Cartesian3D cartesian3D);
                return energyFunc.Invoke(particle, cartesian3D);
            }

            return ProvideEnergy;
        }

        /// <summary>
        ///     Transforms the provided energy provider function for <see cref="Fractional3D"/> to use <see cref="Vector4I"/> data
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="energyFunc"></param>
        /// <returns></returns>
        private static Func<IParticle, Vector4I, double> TransformEnergyFunction(IProjectModelContext modelContext, Func<IParticle, Fractional3D, double> energyFunc)
        {
            var vectorEncoder = modelContext.ModelProject
                                            .Manager<IStructureManager>().DataAccess
                                            .Query(x => x.GetVectorEncoder());

            double ProvideEnergy(IParticle particle, Vector4I vector4)
            {
                vectorEncoder.DecodeUnchecked(vector4, out Fractional3D fractional3D);
                return energyFunc.Invoke(particle, fractional3D);
            }

            return ProvideEnergy;
        }
    }
}
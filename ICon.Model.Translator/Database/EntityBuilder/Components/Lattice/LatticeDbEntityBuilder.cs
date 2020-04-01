using System;
using Mocassin.Framework.Random;
using Mocassin.Model.Lattices;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.EntityBuilder
{
    /// <inheritdoc cref="Mocassin.Model.Translator.EntityBuilder.ILatticeDbEntityBuilder" />
    public class LatticeDbEntityBuilder : DbEntityBuilder, ILatticeDbEntityBuilder
    {
        /// <summary>
        ///     Get or set the used <see cref="Random" /> number source
        /// </summary>
        public Random Rng { get; set; }

        /// <summary>
        ///     Get or set the <see cref="IDopedByteLatticeSource" />
        /// </summary>
        public IDopedByteLatticeSource DopedByteLatticeSource { get; set; }

        /// <inheritdoc />
        public LatticeDbEntityBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
            InitializeBuildComponents();
        }

        /// <inheritdoc />
        public SimulationLatticeModel BuildModel(ISimulationModel simulationModel, LatticeConfiguration latticeConfiguration)
        {
            var simulationLatticeModel = BuildLatticeModelEntity(latticeConfiguration, simulationModel);
            simulationLatticeModel.LatticeInfo = GetLatticeInfoEntity(simulationLatticeModel.Lattice, simulationModel);
            return simulationLatticeModel;
        }

        /// <summary>
        ///     Builds the <see cref="SimulationLatticeModel" /> for the passed <see cref="ISimulationModel" /> and
        ///     <see cref="LatticeConfiguration" />
        /// </summary>
        /// <param name="latticeConfiguration"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        public SimulationLatticeModel BuildLatticeModelEntity(LatticeConfiguration latticeConfiguration, ISimulationModel simulationModel)
        {
            var rawLattice = DopedByteLatticeSource.CreateLattice(latticeConfiguration.GetSizeVector(),
                latticeConfiguration.DopingConcentrations, Rng);
            var interopLattice = new LatticeEntity(rawLattice);
            return new SimulationLatticeModel {Lattice = interopLattice};
        }

        /// <summary>
        ///     Builds the <see cref="CLatticeInfo" /> interop object for the passed set of <see cref="LatticeEntity" /> and
        ///     <see cref="ISimulationModel" />
        /// </summary>
        /// <param name="latticeEntity"></param>
        /// <param name="simulationModel"></param>
        /// <returns></returns>
        public InteropObject<CLatticeInfo> GetLatticeInfoEntity(LatticeEntity latticeEntity, ISimulationModel simulationModel)
        {
            var latticeDimensions = latticeEntity.GetDimensions();
            var latticeInfo = new CLatticeInfo
            {
                SizeVector = new CVector4(latticeDimensions[0], latticeDimensions[1], latticeDimensions[2], latticeDimensions[3])
            };

            CountNumberOfMobiles(latticeEntity, ref latticeInfo, simulationModel);
            return InteropObject.Create(latticeInfo);
        }

        /// <summary>
        ///     Initializes the missing build components of the entity builder
        /// </summary>
        private void InitializeBuildComponents()
        {
            DopedByteLatticeSource = ModelContext.ModelProject.Manager<ILatticeManager>().DataAccess
                .Query(x => x.GetDefaultByteLatticeSource());
            Rng = new PcgRandom32();
        }

        /// <summary>
        ///     Counts the number of mobiles and maximum number of selectable atoms the passed <see cref="LatticeEntity" />
        ///     contains and writes the to the passed <see cref="CLatticeInfo" /> reference
        /// </summary>
        /// <param name="latticeEntity"></param>
        /// <param name="latticeInfo"></param>
        /// <param name="simulationModel"></param>
        private void CountNumberOfMobiles(LatticeEntity latticeEntity, ref CLatticeInfo latticeInfo, ISimulationModel simulationModel)
        {
            var positionIndexToMobilityTypesSet = simulationModel.SimulationEncodingModel.PositionIndexToMobilityTypesSet;
            var (positionIndex, maxPositionIndex) = (0, latticeInfo.SizeVector.D - 1);
            foreach (var elementIndex in (byte[,,,]) latticeEntity.InternalArray)
            {
                var mobilityTypes = positionIndexToMobilityTypesSet[positionIndex];
                switch (mobilityTypes[elementIndex])
                {
                    case MobilityType.Immobile:
                        break;

                    case MobilityType.Mobile:
                        latticeInfo.NumberOfMobiles++;
                        break;

                    case MobilityType.Selectable:
                        latticeInfo.NumberOfMobiles++;
                        latticeInfo.NumberOfSelectAtoms++;
                        break;

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                positionIndex = positionIndex == maxPositionIndex ? 0 : positionIndex + 1;
            }
        }
    }
}
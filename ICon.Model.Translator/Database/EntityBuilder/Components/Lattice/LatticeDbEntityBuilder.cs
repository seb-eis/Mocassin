using System;
using Mocassin.Framework.Random;
using Mocassin.Model.Lattices;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Model.Translator.EntityBuilder
{
    public class LatticeDbEntityBuilder : DbEntityBuilder, ILatticeDbEntityBuilder
    {
        public Random Rng { get; set; }

        public ILatticeCreationProvider LatticeCreationProvider { get; set; }

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

        public SimulationLatticeModel BuildLatticeModelEntity(LatticeConfiguration latticeConfiguration, ISimulationModel simulationModel)
        {
            var rawLattice = LatticeCreationProvider.BuildLattice(latticeConfiguration.GetIntVector3D(),
                latticeConfiguration.DopingConcentrations, Rng);
            var interopLattice = new LatticeEntity(rawLattice);
            return new SimulationLatticeModel {Lattice = interopLattice};
        }

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

        private void InitializeBuildComponents()
        {
            LatticeCreationProvider = ModelContext.ModelProject.GetManager<ILatticeManager>().QueryPort
                .Query(x => x.GetLatticeCreationProvider());
            Rng = new PcgRandom32();
        }

        private void CountNumberOfMobiles(LatticeEntity latticeEntity, ref CLatticeInfo latticeInfo, ISimulationModel simulationModel)
        {
            var mobileDictionary = simulationModel.SimulationEncodingModel.PositionIndexToMobilityTypesSet;
            for (var atomIndex = 0; atomIndex < latticeEntity.Length;)
            {
                for (var positionIndex = 0; positionIndex < latticeInfo.SizeVector.D; positionIndex++)
                {
                    var mobilityTypes = mobileDictionary[positionIndex];
                    var elementIndex = latticeEntity.Values[atomIndex];
                    atomIndex++;

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
                }
            }
        }
    }
}
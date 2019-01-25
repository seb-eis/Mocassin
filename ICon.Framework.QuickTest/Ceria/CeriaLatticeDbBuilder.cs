using System.Linq;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.EntityBuilder;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;

namespace Mocassin.Framework.QuickTest
{
    /// <summary>
    ///     Mock lattice builder for data creation and testing of doped ceria lattices
    /// </summary>
    public class CeriaLatticeDbBuilder : DbEntityBuilder, ILatticeDbEntityBuilder
    {
        /// <inheritdoc />
        public CeriaLatticeDbBuilder(IProjectModelContext modelContext)
            : base(modelContext)
        {
        }

        /// <inheritdoc />
        public SimulationLatticeModel BuildModel(ISimulationModel simulationModel, LatticeConfiguration latticeConfiguration)
        {
            var dbModel = new SimulationLatticeModel
            {
                Lattice = GetLatticeEntity(latticeConfiguration),
                EnergyBackground = GetEnergyBackgroundEntity(latticeConfiguration)
            };

            dbModel.LatticeInfo = GetLatticeInfoEntity(dbModel.Lattice, latticeConfiguration);
            return dbModel;
        }

        private LatticeEntity GetLatticeEntity(LatticeConfiguration latticeConfiguration)
        {
            var entity = new LatticeEntity(BuildTestLattice(latticeConfiguration));

            return entity;
        }

        private EnergyBackgroundEntity GetEnergyBackgroundEntity(LatticeConfiguration latticeConfiguration)
        {
            var entity = new EnergyBackgroundEntity();

            return entity;
        }

        private InteropObject<CLatticeInfo> GetLatticeInfoEntity(LatticeEntity latticeEntity, LatticeConfiguration latticeConfiguration)
        {
            var cLatticeInfo = new CLatticeInfo();
            var sizes = latticeEntity.GetDimensions();
            cLatticeInfo.SizeVector = new CVector4(sizes[0], sizes[1], sizes[2], sizes[3]);

            var vacancyParticle = ModelContext.ModelProject
                .GetManager<IParticleManager>().QueryPort
                .Query(port => port.GetParticles())
                .Single(x => x.Name == "Vacancy");

            var oxygenParticle =ModelContext.ModelProject
                .GetManager<IParticleManager>().QueryPort
                .Query(port => port.GetParticles())
                .Single(x => x.Name == "Oxygen");

            foreach (var value in latticeEntity.Values)
            {
                if (value == vacancyParticle.Index)
                {
                    cLatticeInfo.NumberOfSelectAtoms++;
                    cLatticeInfo.NumberOfMobiles++;
                }

                if (value == oxygenParticle.Index)
                {
                    cLatticeInfo.NumberOfMobiles++;
                }
            }

            return InteropObject.Create(cLatticeInfo);
        }

        private byte[,,,] BuildTestLattice(LatticeConfiguration latticeConfiguration)
        {
            var structureManager = ModelContext.ModelProject.GetManager<IStructureManager>();
            var cellOccupation = GetUnitCellOccupation();
            var atomsPerCell = structureManager.QueryPort.Query(port => port.GetLinearizedExtendedPositionCount());
            var ucpProvider = structureManager.QueryPort.Query(port => port.GetFullUnitCellProvider());
            var lattice = new byte[latticeConfiguration.SizeA, latticeConfiguration.SizeB, latticeConfiguration.SizeC, atomsPerCell];

            for (var i = 0; i < latticeConfiguration.SizeA; i++)
            {
                for (var j = 0; j < latticeConfiguration.SizeB; j++)
                {
                    for (var k = 0; k < latticeConfiguration.SizeC; k++)
                    {
                        for (var l = 0; l < atomsPerCell; l++) lattice[i, j, k, l] = cellOccupation[l];
                    }
                }
            }

            return lattice;
        }

        private byte[] GetUnitCellOccupation()
        {
            var particles = ModelContext.ModelProject.GetManager<IParticleManager>().QueryPort.Query(port => port.GetParticles());

            var idVc = (byte) particles.Single(x => x.Name == "Vacancy").Index;
            var idY = (byte) particles.Single(x => x.Name == "Yttrium").Index;
            var idCe = (byte) particles.Single(x => x.Name == "Cerium" && x.Charge == 4.0).Index;
            var idO = (byte) particles.Single(x => x.Name == "Oxygen").Index;
            byte @void = 0;

            // Ceria sequence is: (k-t-t-k-t-t-t-t-t-a-t-a-t-t-t-a-t-a) x 2
            return new[]
            {
                idCe, @void, @void, idCe, @void, @void, @void, @void, @void, idVc, @void, idO, @void, @void, @void, idO, @void, idO,
                idCe, @void, @void, idCe, @void, @void, @void, @void, @void, idO, @void, idO, @void, @void, @void, idO, @void, idO
            };
        }
    }
}
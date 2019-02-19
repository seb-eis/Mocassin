using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mocassin.Model.Lattices;
using System.Collections.Generic;
using Mocassin.Framework.Collections;
using System.Collections.Immutable;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;
using Mocassin.Symmetry.Analysis;
using Mocassin.Mathematics.Coordinates;
using Mocassin.Mathematics.Comparers;
using System.Linq;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Test
{
    [TestClass]
    public class LatticeManagerTest : ManagementModuleTestsBasis
    {
        [TestMethod]
        public override void TestInputPortSystem()
        {
            throw new NotImplementedException();      
        }

        [TestMethod]
        public override void TestManagementCreation()
        {
            var managers = ManagerFactory.DebugFactory.CreateLatticeManagementSystem(null);

            foreach (var item in managers.LatticeManager.GetType().GetProperties())
            {
                Assert.IsNotNull(item.GetValue(managers.LatticeManager));
            }

            //Assert.IsNotNull(managers.LatticeManager.EventPort);
            //Assert.IsNotNull(managers.LatticeManager.QueryPort);
            //Assert.IsNotNull(managers.LatticeManager.InputPort);
        }

        //[TestMethod]
        //public void TestFMR()
        //{
        //    for (int run = 0; run < 40; run++)
        //    {
        //        List<IntWrapper> intWrappers = new List<IntWrapper>();
        //        for (int i = 0; i < 10; i++)
        //        {
        //            intWrappers.Add(new IntWrapper() { I = i });
        //        }

        //        var sampler = new FMRSampler<IntWrapper>();

        //        Action<IntWrapper> action = (x) => { x.I = 0; };

        //        sampler.ApplyToSamples(intWrappers, 11, action);

        //        foreach (var item in intWrappers)
        //        {
        //            Console.WriteLine("The sampled numer is:" + item.I.ToString());
        //        }

        //        Console.WriteLine("----------------");
        //    }
        //}


        [TestMethod]
        public void TestLatticeCreation()
        {
            //////////////////////Lattice creation/////////////////////////////////////////////////////////////////////////////////////////////

            //Particle particleA = new Particle() { Charge = 0, Name = "ElementA", Symbol = "A", IsDeprecated = false, Index = 0, IsEmpty = false, IsVacancy = false };
            //Particle particleB = new Particle() { Charge = 0, Name = "ElementB", Symbol = "B", IsDeprecated = false, Index = 1, IsEmpty = false, IsVacancy = false };

            //Particle particleC = new Particle() { Charge = 2, Name = "ElementC", Symbol = "C", IsDeprecated = false, Index = 2, IsEmpty = false, IsVacancy = false };
            //Particle particleD = new Particle() { Charge = -1, Name = "ElementD", Symbol = "D", IsDeprecated = false, Index = 3, IsEmpty = false, IsVacancy = false };

            //ReadOnlyList<IParticle> particles = ReadOnlyList<IParticle>.FromEnumerable(new Particle[]
            //{
            //    particleA,
            //    particleB
            //});

            //ReadOnlyList<IParticle> particlesCustom = ReadOnlyList<IParticle>.FromEnumerable(new Particle[]
            //{
            //    particleA,
            //    particleA
            //});

            //UnitCellAdapter<IParticle> unitCellWrapper1 = new UnitCellAdapter<IParticle>
            //   (
            //       particles,
            //       new UnitCellVectorEncoder
            //       (
            //           new SetList<Fractional3D>
            //           (
            //               new VectorComparer3D<Fractional3D>(new DoubleRangeComparer(0.1))
            //           )
            //           { new Fractional3D(0,0,0), new Fractional3D(0.5,0.5,0.5) },
            //           new VectorTransformer
            //           (
            //               new FractionalCoordinateSystem3D(
            //                   new Coordinates<double, double, double>(1,0,0),
            //                   new Coordinates<double, double, double>(0,1,0),
            //                   new Coordinates<double, double, double>(0,0,1),
            //                   new DoubleRangeComparer(0.1)),
            //               SphericalCoordinateSystem3D.CreateISO(0.1)
            //           )
            //       ));

            //UnitCellAdapter<IParticle> unitCellWrapper2 = new UnitCellAdapter<IParticle>
            //   (
            //       particlesCustom,
            //       new UnitCellVectorEncoder
            //       (
            //           new SetList<Fractional3D>
            //           (
            //               new VectorComparer3D<Fractional3D>(new DoubleRangeComparer(0.1))
            //           )
            //           { new Fractional3D(0,0,0), new Fractional3D(0.5,0.5,0.5) },
            //           new VectorTransformer
            //           (
            //               new FractionalCoordinateSystem3D(
            //                   new Coordinates<double, double, double>(1,0,0),
            //                   new Coordinates<double, double, double>(0,1,0),
            //                   new Coordinates<double, double, double>(0,0,1),
            //                   new DoubleRangeComparer(0.1)),
            //               SphericalCoordinateSystem3D.CreateISO(0.1)
            //           )
            //       ));

            //ReadOnlyList<IUnitCell<IParticle>> unitCells = ReadOnlyList<IUnitCell<IParticle>>.FromEnumerable(new UnitCellAdapter<IParticle>[] { unitCellWrapper1, unitCellWrapper2 });

            //ReadOnlyList <IBlockInfo> blockInfos = ReadOnlyList<IBlockInfo>.FromEnumerable(new BlockInfo[] 
            //{
            //    (new BlockInfo() {Origin = new DataIntegralVector3D(0,0,0), Extent = new DataIntegralVector3D(16,16,16), Index = 0, IsDeprecated = false }),
            //    (new BlockInfo() {Origin = new DataIntegralVector3D(0,0,0), Extent = new DataIntegralVector3D(1,1,16), Index = 1, IsDeprecated = false })
            //});

            //UnitCellPosition unitCellPosition1 = new UnitCellPosition() { Index = 0, IsDeprecated = false, OccupationIndex = 1, Status = PositionStatus.Stable, Vector = new DataVector3D(0, 0, 0) };
            //UnitCellPosition unitCellPosition2 = new UnitCellPosition() { Index = 1, IsDeprecated = false, OccupationIndex = 2, Status = PositionStatus.Stable, Vector = new DataVector3D(0.5, 0.5, 0.5) };

            //IReadOnlyDictionary<int, IUnitCellPosition> unitCellPositions = new Dictionary<int, IUnitCellPosition>()
            //{
            //    {0, unitCellPosition1 },
            //    {1, unitCellPosition2 }
            //};

            //Coordinates<int, int, int> extent = new Coordinates<int, int, int>(16, 16, 16);

            ////WorkLattice workLattice = (new WorkLatticeFactory()).Fabricate(unitCells, blockInfos, unitCellPositions, extent);

            ////////////////////////Doping///////////////////////////////////////////////////////////////////////////////////////////////////////

            //DopingCode dopingInfo = new DopingCode() { Dopant = particleC, DopedParticle = particleA, UnitCellPosition = unitCellPosition1, Index = 0, IsDeprecated = false };
            //DopingCode counterDopingInfo = new DopingCode() { Dopant = particleD, DopedParticle = particleD, UnitCellPosition = unitCellPosition2, Index = 1, IsDeprecated = false };
            //Doping doping = new Doping() { Concentration = 0.1, DopingInfo = dopingInfo, CounterDopingInfo = counterDopingInfo };

            //(new DopingExecuter()).Execute(workLattice, doping);

            //Dictionary<string, int> elementCounterNonCustom = new Dictionary<string,int>(){ { "A", 0 }, { "B", 0 }, { "C", 0 }, { "D", 0 } };
            //Dictionary<string, int> elementCounterCustom = new Dictionary<string, int>() { { "A", 0 }, { "B", 0 }, { "C", 0 }, { "D", 0 } };

            //foreach (var item in workLattice.WorkCells)
            //{
            //    foreach (var entry in item.CellEntries)
            //    {
            //        if (item.IsCustom == true)
            //        {
            //            elementCounterCustom[entry.Particle.Symbol]++;
            //        }
            //        else
            //        {
            //            elementCounterNonCustom[entry.Particle.Symbol]++;
            //        }
            //    }
            //}

            //Console.WriteLine("Elements in custom lattice:");
            //foreach (var entry in elementCounterCustom)
            //{
            //    Console.WriteLine(entry.Key + ": " + entry.Value.ToString());
            //}
            //Console.WriteLine("Elements in default doped lattice:");
            //foreach (var entry in elementCounterNonCustom)
            //{
            //    Console.WriteLine(entry.Key + ": " + entry.Value.ToString());
            //}


        }

        void AcceptList(IReadOnlyCollection<IParticle> particles)
        {
            return;
        }

    }
}

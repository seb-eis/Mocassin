using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using Mocassin.Framework.Collections;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Test
{
    [TestClass]
    public class TransitionLogicTests
    {
        [TestMethod]
        public void MetropolisMapperBaseTest()
        {
            var managers = ManagerFactory.DebugFactory.CreateManageSystemForCeria(null);
            var mapper = new MetropolisTransitionMapper();

            var encoded = managers.StructureManager.QueryPort.Query(port => port.GetEncodedExtendedPositionLists());
            var transition0 = new MetropolisTransition()
            {
                FirstUnitCellPosition = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(0)),
                SecondUnitCellPosition = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(0))
            };
            var transition1 = new MetropolisTransition()
            {
                FirstUnitCellPosition = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(1)),
                SecondUnitCellPosition = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(1))
            };
            var mappings = mapper.MapTransitions(new IMetropolisTransition[] { transition0, transition1 }, encoded).ToArray();

            Assert.IsTrue(mappings[0].Count() == encoded[0].Count * encoded[0].Count);
            Assert.IsTrue(mappings[1].Count() == encoded[1].Count * encoded[1].Count);
        }

        [TestMethod]
        public void TestKineticAppoxMapper()
        {
            var managers = ManagerFactory.DebugFactory.CreateManageSystemForCeria(null);

            var comparer = managers.ModelProject.GeometryNumeric.RangeComparer;
            var ucProvider = managers.StructureManager.QueryPort.Query(port => port.GetOccupationUnitCellProvider());
            var startPoints = managers.StructureManager.QueryPort.Query(port => port.GetExtendedPositionList(1)).Select(value => value.Vector);
            var mapper = new ApproxKineticTransitionMapper(managers.ModelProject.SymmetryAnalysisService, ucProvider);

            var interGeoemtry = new List<DataVector3D>
            {
                 new DataVector3D(.25,.25,.25), new DataVector3D(.75,.75,.75)
            };
            var longGeometry = new List<DataVector3D>
            {
                new DataVector3D(.25,.25,.25), new DataVector3D(.75,.25,.25), new DataVector3D(.75,.75,.25)
            };
            var shortGeometry = new List<DataVector3D>
            {
                new DataVector3D(.25,.25,.25), new DataVector3D(.75,.25,.25)
            };
            var interTransition = new KineticTransition() { Index = 0, PathGeometry = interGeoemtry };
            var shortTransition = new KineticTransition() { Index = 0, PathGeometry = shortGeometry };
            var longTransition = new KineticTransition() { Index = 0, PathGeometry = longGeometry };

            var shortResult = mapper.QuickMapping(shortTransition, startPoints, comparer);
            var longResult = mapper.QuickMapping(longTransition, startPoints, comparer);
            var interResult = mapper.QuickMapping(interTransition, startPoints, comparer);

            // This yields 64 instead of the expected 32 when the mapper is used without defined transition positions
            Assert.IsTrue(interResult.Count() == 64);
            Assert.IsTrue(shortResult.Count() == 48);
            Assert.IsTrue(longResult.Count() == 192);
        }

        [TestMethod]
        public void TestKineticQuickMapper()
        {
            var managers = ManagerFactory.DebugFactory.CreateManageSystemForCeria(null);
            var structureManager = managers.ModelProject.GetManager<IStructureManager>();
            var encoder = structureManager.QueryPort.Query(port => port.GetVectorEncoder());
            var ucp = structureManager.QueryPort.Query(port => port.GetFullUnitCellProvider());

            var mapper = new KineticTransitionMapper(managers.ModelProject.SpaceGroupService, encoder, ucp);

            var interGeoemtry = new List<DataVector3D>
            {
                 new DataVector3D(0.25,0.25,0.25), new DataVector3D(0.75,0.75,0.75), new DataVector3D(1.25,1.25,1.25)
            };
            var longGeometry = new List<DataVector3D>
            {
                new DataVector3D(.25,.25,.25), new DataVector3D(.75,.25,.25), new DataVector3D(.75,.75,.25)
            };
            var shortGeometry = new List<DataVector3D>
            {
                new DataVector3D(.25,.25,.25), new DataVector3D(.75,.25,.25)
            };
            var shortTransition = new KineticTransition() { Index = 0, PathGeometry = shortGeometry };
            var longTransition = new KineticTransition() { Index = 1, PathGeometry = longGeometry };
            var interTransition = new KineticTransition() { Index = 2, PathGeometry = interGeoemtry };

            // 8 Starts, 6 Directions (=>48)
            var shortResult = mapper.GetMappings(shortTransition);

            // 8 Starts, 6 Directions , 6 Directions (2 Invalid) (=>192)
            var longResult = mapper.GetMappings(longTransition);

            // 8 Starts, 8 Directions (4 Invalid) , 1 Direction (=>32)
            var interResult = mapper.GetMappings(interGeoemtry.Select(value => value.AsFractional()), interTransition);

            Assert.IsTrue(interResult.Count() == 32);
            Assert.IsTrue(shortResult.Count() == 48);
            Assert.IsTrue(longResult.Count() == 192);
        }

        [TestMethod]
        public void TestQuickRuleGenerator()
        {
            int empty = 0;
            int vacancy = 1;
            int oxygen = 2;
            int hydrogen = 3;
            int vehicle = 4;
            var particles = new IParticle[]
            {
                new Particle() { Index = empty, Name = "Empty", IsEmpty = true},
                new Particle() { Index = vacancy, Name = "Vacancy"},
                new Particle() { Index = oxygen, Name = "Oxygen"},
                new Particle() { Index = hydrogen, Name = "Hydrogen"},
                new Particle() { Index = vehicle, Name = "Vehicle"},
            };

            var oxygenStateGroup = new StatePairGroup(new(int, int)[] { (oxygen, vacancy) }, PositionStatus.Stable);
            var hydrogenStateGroup = new StatePairGroup(new(int, int)[] { (hydrogen, vacancy) }, PositionStatus.Stable);
            var oxygenTransGroup = new StatePairGroup(new(int, int)[] { (oxygen, empty) }, PositionStatus.Unstable);
            var hydrogenTransGroup = new StatePairGroup(new(int, int)[] { (hydrogen, empty) }, PositionStatus.Unstable);
            var vehicleTransGroup = new StatePairGroup(new(int, int)[] { (vehicle, empty) }, PositionStatus.Unstable);

            var generator = new TransitionRuleGenerator<KineticRule>(particles);

            // Migration pattern (Should yield 2 rules, without activated filtering the inverse case is also present)
            var migrationPattern = new ConnectorType[]
            {
                ConnectorType.Dynamic, ConnectorType.Dynamic
            };
            var migrationGroups = new StatePairGroup[]
            {
                oxygenStateGroup, oxygenTransGroup, oxygenStateGroup
            };
            var migRules = generator.MakeUniqueRules(migrationGroups, migrationPattern);
            Assert.IsTrue(migRules.Count() == 2);

            // Chained pattern (Should yield 1 rule, the inverted case cannot be produced)
            var chainedPattern = new ConnectorType[]
            {
                ConnectorType.Dynamic, ConnectorType.Dynamic, ConnectorType.Dynamic, ConnectorType.Dynamic
            };
            var chainedGroups = new StatePairGroup[]
            {
                oxygenStateGroup, oxygenTransGroup, oxygenStateGroup, oxygenTransGroup, oxygenStateGroup
            };
            var chainedRules = generator.MakeUniqueRules(chainedGroups, chainedPattern);
            Assert.IsTrue(chainedRules.Count() == 1);

            //Vehicle pattern (Default D-D-S-D-D, should yield 4 as each single step is treated like an unchained migration with forw/backw jump)
            var vehiclePattern0 = new ConnectorType[]
            {
                ConnectorType.Dynamic, ConnectorType.Dynamic, ConnectorType.Static, ConnectorType.Dynamic, ConnectorType.Dynamic
            };
            var vehicleGroups0 = new StatePairGroup[]
            {
                oxygenStateGroup, oxygenTransGroup, oxygenStateGroup, hydrogenStateGroup, hydrogenTransGroup, hydrogenStateGroup
            };
            var vehicleRules0 = generator.MakeUniqueRules(vehicleGroups0, vehiclePattern0);
            Assert.IsTrue(vehicleRules0.Count() == 4);

            //Vehicle pattern (special D-S-S-D, should yield 4, same reason as with default vehicle pattern)
            var vehiclePattern1 = new ConnectorType[]
            {
                ConnectorType.Dynamic, ConnectorType.Static, ConnectorType.Static, ConnectorType.Dynamic
            };
            var vehicleGroups1 = new StatePairGroup[]
            {
                oxygenStateGroup, oxygenStateGroup, vehicleTransGroup, hydrogenStateGroup, hydrogenStateGroup
            };
            var vehicleRules1 = generator.MakeUniqueRules(vehicleGroups1, vehiclePattern1);
            Assert.IsTrue(vehicleRules1.Count() == 4);
        }
    }
}

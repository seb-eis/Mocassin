using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ICon.Symmetry.SpaceGroups;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Structures;
using ICon.Model.Transitions;

namespace ICon.Model.Test
{
    [TestClass]
    public class TransitionManagerTest : ManagementModuleTestsBasis
    {
        [TestMethod]
        public override void TestInputPortSystem()
        {
            throw new NotImplementedException();
        }

        [TestMethod]
        public override void TestManagementCreation()
        {
            var managers = ManagerFactory.DebugFactory.CreateTransitionManagementSystem();
            foreach (var item in managers.TransitionManager.GetType().GetProperties())
            {
                Assert.IsNotNull(item.GetValue(managers.TransitionManager));
            }
        }

        [TestMethod]
        public void CeriaTransitionMappingTest()
        {
            var managers = ManagerFactory.DebugFactory.CreateManageSystemForCeria();

            var firstPair = new PropertyStatePair()
            {
                DonorParticle = managers.ParticleManager.QueryPort.Query(port => port.GetParticle(2)),
                AcceptorParticle = managers.ParticleManager.QueryPort.Query(port => port.GetParticle(1)),
                IsVacancyPair = true
            };
            var propertyGroup = new PropertyGroup()
            {
                VacancyGroup = true,
                PropertyStatePairs = new List<IPropertyStatePair>
                {
                    firstPair, firstPair
                }
            };
            var abstractTransition = new AbstractTransition()
            {
                Index = 0,
                Name = "OxygenMigration",
                PropertyGroups = new List<IPropertyGroup> { propertyGroup, propertyGroup },
                Connectors = new List<ConnectorType> { ConnectorType.Dynamic }
            };
            var abstractTransition2 = new AbstractTransition()
            {
                Index = 1,
                Name = "FunnyMigration",
                PropertyGroups = new List<IPropertyGroup> { propertyGroup, propertyGroup, propertyGroup },
                Connectors = new List<ConnectorType> { ConnectorType.Dynamic, ConnectorType.Dynamic }
            };
            var metropolisTransition = new MetropolisTransition()
            {
                CellPosition0 = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(1)),
                CellPosition1 = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(1))
            };
            var kineticTransition = new KineticTransition()
            {
                AbstractTransition = abstractTransition,
                PathGeometry = new List<DataVector3D> { new DataVector3D(.25,.25,.25), new DataVector3D(.75, .25, .25) }
            };
            var kineticTransition2 = new KineticTransition()
            {
                AbstractTransition = abstractTransition2,
                PathGeometry = new List<DataVector3D> { new DataVector3D(.25, .25, .25), new DataVector3D(.75, .25, .25), new DataVector3D(.75, .75, .25) }
            };
            var kineticTransition3 = new KineticTransition()
            {
                AbstractTransition = abstractTransition,
                PathGeometry = new List<DataVector3D> { new DataVector3D(.25, .25, .25), new DataVector3D(.75, .75, .75) }
            };

            // Input abstracts and pairs
            var result = managers.TransitionManager.InputPort.InputModelObject(firstPair).Result;
            var result2 = managers.TransitionManager.InputPort.InputModelObject(propertyGroup).Result;
            var result3 = managers.TransitionManager.InputPort.InputModelObject(abstractTransition).Result;
            var result6 = managers.TransitionManager.InputPort.InputModelObject(abstractTransition2).Result;

            // Input transitions
            var result4 = managers.TransitionManager.InputPort.InputModelObject(metropolisTransition).Result;
            var result5 = managers.TransitionManager.InputPort.InputModelObject(kineticTransition).Result;
            var result7 = managers.TransitionManager.InputPort.InputModelObject(kineticTransition2).Result;
            var result8 = managers.TransitionManager.InputPort.InputModelObject(kineticTransition3).Result;

            // Test mappers
            var kinMappings = managers.TransitionManager.QueryPort.Query(port => port.GetAllKineticMappingLists());
            var metMappings = managers.TransitionManager.QueryPort.Query(port => port.GetAllMetropolisMappingLists());

            // Assert that kinetic counts are (48,192,32) and metropolis counts are (64)
            Assert.IsTrue(kinMappings.Select(value => value.Count).SequenceEqual(new int[] { 48, 192, 32 }));
            Assert.IsTrue(metMappings.Select(value => value.Count).SequenceEqual(new int[] { 64 }));
        
        }

        [TestMethod]
        public void CeriaTransitionRuleGenerationTest()
        {
            var managers = ManagerFactory.DebugFactory.CreateManageSystemForCeria();

            var firstPair = new PropertyStatePair()
            {
                DonorParticle = managers.ParticleManager.QueryPort.Query(port => port.GetParticle(2)),
                AcceptorParticle = managers.ParticleManager.QueryPort.Query(port => port.GetParticle(1)),
                IsVacancyPair = true
            };
            var propertyGroup = new PropertyGroup()
            {
                VacancyGroup = true,
                PropertyStatePairs = new List<IPropertyStatePair> { firstPair }
            };
            var abstractTransition = new AbstractTransition()
            {
                Name = "OxygenMigration",
                PropertyGroups = new List<IPropertyGroup> { propertyGroup, propertyGroup },
                Connectors = new List<ConnectorType> { ConnectorType.Dynamic }
            };
            var metropolisTransition = new MetropolisTransition()
            {
                CellPosition0 = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(1)),
                CellPosition1 = managers.StructureManager.QueryPort.Query(port => port.GetUnitCellPosition(1))
            };
            var kineticTransition = new KineticTransition()
            {
                AbstractTransition = abstractTransition,
                PathGeometry = new List<DataVector3D> { new DataVector3D(.25, .25, .25), new DataVector3D(.75, .25, .25) }
            };

            // Input abstracts and pairs
            var result = managers.TransitionManager.InputPort.InputModelObject(firstPair).Result;
            var result2 = managers.TransitionManager.InputPort.InputModelObject(propertyGroup).Result;
            var result3 = managers.TransitionManager.InputPort.InputModelObject(abstractTransition).Result;

            // Input transitions
            var result4 = managers.TransitionManager.InputPort.InputModelObject(metropolisTransition).Result;
            var result5 = managers.TransitionManager.InputPort.InputModelObject(kineticTransition).Result;

            // Test rule generator
            var kinRules = managers.TransitionManager.QueryPort.Query(port => port.GetAllKineticRuleLists());
            var metRules = managers.TransitionManager.QueryPort.Query(port => port.GetAllMetropolisRuleLists());

            // Assert that there are 1 kinetic and 1 metropolis rule
            Assert.IsTrue(kinRules.Select(value => value.Count).SequenceEqual(new int[] { 1 }));
            Assert.IsTrue(metRules.Select(value => value.Count).SequenceEqual(new int[] { 1 }));
        }
    }
}

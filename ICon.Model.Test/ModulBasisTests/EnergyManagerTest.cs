using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Particles;
using ICon.Model.Basic;
using ICon.Model.Energies;

namespace ICon.Model.Test
{
    [TestClass]
    public class EnergyManagerTest : ManagementModuleTestsBasis
    {
        [TestMethod]
        public override void TestInputPortSystem()
        {
            var managers = ManagerFactory.DebugFactory.CreateManageSystemForCeria();

            var validEnvInfo = new StableEnvironmentInfo()
            {
                MaxInteractionRange = 5.0,
                IgnoredPairInteractions = new List<SymParticlePair>
                {
                    new SymParticlePair() { Particle0 = new Particle(){ Index = 1}, Particle1 = new Particle(){ Index = 1} },
                    new SymParticlePair() { Particle0 = new Particle(){ Index = 1}, Particle1 = new Particle(){ Index = 1} },
                }
            };

            var invalidEnvInfo = new StableEnvironmentInfo()
            {
                MaxInteractionRange = 100.0,
                IgnoredPairInteractions = new List<SymParticlePair>
                {
                    new SymParticlePair() { Particle0 = new Particle(){ Index = 1}, Particle1 = new Particle(){ Index = 1} },
                    new SymParticlePair() { Particle0 = new Particle(){ Index = 1}, Particle1 = new Particle(){ Index = 1} },
                }
            };

            var firstResult = managers.EnergyManager.InputPort.SetModelParameter(invalidEnvInfo).Result;
            var secondResult = managers.EnergyManager.InputPort.SetModelParameter(validEnvInfo).Result;

            Assert.IsFalse(firstResult.IsGood);
            Assert.IsTrue(secondResult.IsGood);
        }

        [TestMethod]
        public override void TestManagementCreation()
        {
            var managers = ManagerFactory.DebugFactory.CreateEnergyManagementSystem();
            foreach (var item in managers.EnergyManager.GetType().GetProperties())
            {
                Assert.IsNotNull(item.GetValue(managers.EnergyManager));
            }
        }
    }
}

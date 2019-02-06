using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Basic;
using Mocassin.Model.Energies;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Test
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
            };

            var invalidEnvInfo = new StableEnvironmentInfo()
            {
                MaxInteractionRange = 100.0,
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

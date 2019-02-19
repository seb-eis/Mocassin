using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Mocassin.Mathematics.Bitmasks;
using Mocassin.Model.Basic;
using Mocassin.Model.Particles;

namespace Mocassin.Model.Test
{
    [TestClass]
    public class ParticleManagerTest : ManagementModuleTestsBasis
    {
        [TestMethod]
        public override void TestManagementCreation()
        {
            var managers = ManagerFactory.DebugFactory.CreateParticleManagementSystem(null);
            foreach (var item in managers.ParticleManager.GetType().GetProperties())
            {
                Assert.IsNotNull(item.GetValue(managers.ParticleManager));
            }
            Assert.IsTrue(managers.ParticleManager.QueryPort.Query(data => data.GetParticle(0)).CompareTo(Particle.CreateEmpty()) == 0);
            Assert.IsTrue(managers.ParticleManager.QueryPort.Query(data => data.GetParticleSet(0).GetEncoded().Mask) == ParticleSet.CreateEmpty().GetEncoded().Mask);
        }

        [TestMethod]
        public override void TestInputPortSystem()
        {
            var managers = ManagerFactory.DebugFactory.CreateParticleManagementSystem(null);
            // Test default inputs
            for (int i = 1; i < 64; i++)
            {
                var task = managers.ParticleManager.InputPort.InputModelObject(new Particle() { Name = $"Hydrogen{i}", Symbol = $"H{i}", Charge = i, IsVacancy = false });
                Assert.IsTrue(task.Result.IsGood);
            }
            for (int i = 1; i < 64; i++)
            {
                var particle = managers.ParticleManager.QueryPort.Query(data => data.GetParticle(i));
                Assert.IsTrue(new Particle() { Name = $"Hydrogen{i}", Symbol = $"H{i}", Charge = i, IsVacancy = false, Index = i }.CompareTo(particle) == 0);
            }
            for (int i = 1; i < 64; i++)
            {
                var particle = managers.ParticleManager.QueryPort.Query(port => port.GetParticle(i));
                var task = managers.ParticleManager.InputPort.InputModelObject(new ParticleSet() { Particles = new List<IParticle> { particle } });
                Assert.IsTrue(task.Result.IsGood);
            }
            for (int i = 1; i < 64; i++)
            {
                var mask = managers.ParticleManager.QueryPort.Query(data => data.GetParticleSet(i).GetEncoded().Mask);
                Assert.IsTrue(new Bitmask64(1UL + (1UL << i)).Mask == mask);
            }
            for (int i = 1; i < 64; i++)
            {
                var task = managers.ParticleManager.InputPort.ReplaceModelObject(new Particle() { Index = i}, new Particle() { Name = $"Replace{i}", Symbol = $"H{i}", Charge = i, IsVacancy = false });
                Assert.IsTrue(task.Result.IsGood);
            }

            // Tests that should fail
            var failTask1 = managers.ParticleManager.InputPort.InputModelObject(new Particle() { Name = "Fail", Symbol = "Fail" });
            Assert.IsFalse(failTask1.Result.IsGood);

            // Test depraction of model objects
            for (int i = 1; i < 64; i++)
            {
                var task = managers.ParticleManager.InputPort.RemoveModelObject(new Particle() { Index = i });
                Assert.IsTrue(task.Result.IsGood);
            }
            for (int i = 1; i < 64; i++)
            {
                var task = managers.ParticleManager.InputPort.RemoveModelObject(new ParticleSet() { Index = i });
                Assert.IsTrue(task.Result.IsGood);
            }

            // Remove all deprecated objects and check if lists have returned to default size
            var cleanResult = managers.ParticleManager.InputPort.CleanupManager().Result;
            Assert.IsTrue(managers.ParticleManager.QueryPort.Query(port => port.GetParticles().Count == 1));
            Assert.IsTrue(managers.ParticleManager.QueryPort.Query(port => port.GetParticleSets().Count == 1));
        }
    }
}

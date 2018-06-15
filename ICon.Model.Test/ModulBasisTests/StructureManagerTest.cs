using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ICon.Symmetry.SpaceGroups;

using ICon.Mathematics.ValueTypes;
using ICon.Mathematics.Comparers;
using ICon.Symmetry.CrystalSystems;
using ICon.Model.Basic;
using ICon.Model.Structures;

namespace ICon.Model.Test
{
    [TestClass]
    public class StructureManagerTest : ManagementModuleTestsBasis
    {
        [TestMethod]
        public override void TestInputPortSystem()
        {
            var manager = ManagerFactory.DebugFactory.CreateStructureManagementSystem();

            var dummy = new PositionDummy() { Vector = new DataVector3D(0.5, 0.5, 0.5) };

            var result0 = manager.StructureManager.InputPort.InputModelObject(dummy).Result;
            var result1 = manager.StructureManager.InputPort.InputModelObject(dummy).Result;
            Assert.IsTrue(result0.IsGood);
            Assert.IsFalse(result1.IsGood);
        }

        [TestMethod]
        public void FullSpaceGroupDatabaseLoadTest()
        {
            var managers = ManagerFactory.DebugFactory.CreateStructureManagementSystem();
            var groups = managers.ProjectServices.SpaceGroupService.GetFullGroupList();
            groups.RemoveAt(0);
            foreach (var item in groups)
            {
                var task = managers.StructureManager.InputPort.SetModelParameter(new SpaceGroupInfo() { GroupEntry = new SpaceGroupEntry(item) });
                Assert.IsTrue(task.Result.IsGood);
            }
        }

        [TestMethod]
        public void TestVectorEncoderUpdate()
        {
            var managers = ManagerFactory.DebugFactory.CreateManageSystemForCeria();
            var comparer = new VectorComparer3D<Cartesian3D>(DoubleComparer.CreateRanged(1.0e-6));
            for (int i = 1; i < 10; i++)
            {
                var (refA, refB, refC) = (new Cartesian3D(i, 0, 0), new Cartesian3D(0, i, 0), new Cartesian3D(0, 0, i));
                managers.StructureManager.InputPort.SetModelParameter(new CellParameters() { ParameterSet = new CrystalParameterSet(i, i, i, 0, 0, 0) }).Wait();
                var (A, B, C) = managers.ProjectServices.CrystalSystemService.VectorTransformer.FractionalSystem.GetBaseVectors();
                Assert.IsTrue(comparer.Equals(A, refA));
                Assert.IsTrue(comparer.Equals(B, refB));
                Assert.IsTrue(comparer.Equals(C, refC));
            }
        }

        [TestMethod]
        public override void TestManagementCreation()
        {
            var managers = ManagerFactory.DebugFactory.CreateStructureManagementSystem();
            foreach (var item in managers.StructureManager.GetType().GetProperties())
            {
                Assert.IsNotNull(item.GetValue(managers.StructureManager));
            }
        }
    }
}

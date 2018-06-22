using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using ICon.Model.Basic;
using ICon.Model.Lattices;
using System.Collections.Generic;
using ICon.Model.Particles;
using ICon.Framework.Collections;
using System.Collections.Immutable;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Structures;
using ICon.Symmetry.Analysis;
using ICon.Mathematics.Coordinates;
using ICon.Mathematics.Comparers;
using System.Linq;

namespace ICon.Model.Test
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
            var managers = ManagerFactory.DebugFactory.CreateLatticeManagementSystem();

            foreach (var item in managers.LatticeManager.GetType().GetProperties())
            {
                Assert.IsNotNull(item.GetValue(managers.LatticeManager));
            }

            Assert.IsNotNull(managers.LatticeManager.EventPort);
            Assert.IsNotNull(managers.LatticeManager.QueryPort);
            Assert.IsNotNull(managers.LatticeManager.InputPort);
        }

    }
}

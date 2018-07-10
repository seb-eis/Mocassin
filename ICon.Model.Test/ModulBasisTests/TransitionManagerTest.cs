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
    }
}

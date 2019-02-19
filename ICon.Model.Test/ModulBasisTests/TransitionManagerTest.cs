using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mocassin.Symmetry.SpaceGroups;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Test
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
            var managers = ManagerFactory.DebugFactory.CreateTransitionManagementSystem(null);
            foreach (var item in managers.TransitionManager.GetType().GetProperties())
            {
                Assert.IsNotNull(item.GetValue(managers.TransitionManager));
            }
        }
    }
}

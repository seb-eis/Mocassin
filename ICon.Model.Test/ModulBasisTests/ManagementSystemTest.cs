using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic;
using ICon.Model.Energies;
using ICon.Model.Lattices;
using ICon.Model.ProjectServices;

namespace ICon.Model.Test
{
    [TestClass]
    public class ManagemntSystemTest
    {
        [TestMethod]
        public void ManagementSystemSetupTest()
        {
            void CheckNoPropertyNull(object obj)
            {
                Assert.IsNotNull(obj);
                foreach (var propType in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
                {
                    Assert.IsNotNull(propType.GetValue(obj));
                }
            }

            ManagerPackage manager = ManagerFactory.DebugFactory.CreateFullManagementSystem();
            CheckNoPropertyNull(manager);
        }
    }
}

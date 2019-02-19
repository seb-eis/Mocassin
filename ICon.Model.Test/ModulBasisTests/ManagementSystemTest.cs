using System;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.Energies;
using Mocassin.Model.Lattices;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Basic;

namespace Mocassin.Model.Test
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

            ManagerPackage manager = ManagerFactory.DebugFactory.CreateFullManagementSystem(null);
            CheckNoPropertyNull(manager);
        }
    }
}

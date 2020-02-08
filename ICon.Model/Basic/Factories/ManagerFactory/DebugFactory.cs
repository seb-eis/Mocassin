using Mocassin.Model.DataManagement;
using Mocassin.Model.Energies;
using Mocassin.Model.Lattices;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Particles;
using Mocassin.Model.Simulations;
using Mocassin.Model.Structures;
using Mocassin.Model.Transitions;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Defines a static helper class for generating <see cref="ManagerPackage" /> instances
    /// </summary>
    public static class ManagerFactory
    {
        /// <summary>
        ///     Debug factory that directly creates all dependent managers
        /// </summary>
        public static class DebugFactory
        {
            /// <summary>
            ///     Create a project service system for testing
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateProjectServicesSystem(ProjectSettings settings)
            {
                return new ManagerPackage
                {
                    ModelProject = ModelProject.ModelProject.Create(settings ?? ProjectSettings.CreateDefault())
                };
            }

            /// <summary>
            ///     Creates and links default project services and particle manager for testing
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateParticleManagementSystem(ProjectSettings settings)
            {
                var package = CreateProjectServicesSystem(settings);
                package.ParticleManager = (IParticleManager) package.ModelProject.CreateAndRegister(new ParticleManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates structure management system for testing
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateStructureManagementSystem(ProjectSettings settings)
            {
                var package = CreateParticleManagementSystem(settings);
                package.StructureManager = (IStructureManager) package.ModelProject.CreateAndRegister(new StructureManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates lattice management system for testing
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateLatticeManagementSystem(ProjectSettings settings)
            {
                var package = CreateProjectServicesSystem(settings);
                package.LatticeManager = (ILatticeManager) package.ModelProject.CreateAndRegister(new LatticeManagerFactory());

                return package;
            }

            /// <summary>
            ///     Creates a management system that is capable of the modeling process until transition inputs
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateTransitionManagementSystem(ProjectSettings settings)
            {
                var package = CreateStructureManagementSystem(settings);
                package.TransitionManager = (ITransitionManager) package.ModelProject.CreateAndRegister(new TransitionManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates a management system that is capable of the modeling process until energy inputs
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateEnergyManagementSystem(ProjectSettings settings)
            {
                var package = CreateTransitionManagementSystem(settings);
                package.EnergyManager = (IEnergyManager) package.ModelProject.CreateAndRegister(new EnergyManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates a management system that is capable of the modeling process until simulation inputs
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateSimulationManagementPackage(ProjectSettings settings)
            {
                var package = CreateEnergyManagementSystem(settings);
                package.SimulationManager = (ISimulationManager) package.ModelProject.CreateAndRegister(new SimulationManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates the currently most developed management system
            /// </summary>
            /// <param name="settings"></param>
            /// <returns></returns>
            public static ManagerPackage CreateFullManagementSystem(ProjectSettings settings)
            {
                return CreateSimulationManagementPackage(settings);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using ICon.Mathematics.ValueTypes;
using ICon.Model.Basic.Debug;
using ICon.Model.DataManagement;
using ICon.Model.Energies;
using ICon.Model.Lattices;
using ICon.Model.Particles;
using ICon.Model.ProjectServices;
using ICon.Model.Simulations;
using ICon.Model.Structures;
using ICon.Model.Transitions;
using ICon.Symmetry.CrystalSystems;
using ICon.Symmetry.SpaceGroups;

namespace ICon.Model.Basic
{
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
            /// <returns></returns>
            public static ManagerPackage CreateProjectServicesSystem()
            {
                return new ManagerPackage
                {
                    ProjectServices = ProjectServices.ProjectServices.Create(ProjectSettingsData.CreateDefault())
                };
            }

            /// <summary>
            ///     Creates and links default project services and particle manager for testing
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateParticleManagementSystem()
            {
                var package = CreateProjectServicesSystem();
                package.ParticleManager = (IParticleManager) package.ProjectServices.CreateAndRegister(new ParticleManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates structure management system for testing
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateStructureManagementSystem()
            {
                var package = CreateParticleManagementSystem();
                package.StructureManager = (IStructureManager) package.ProjectServices.CreateAndRegister(new StructureManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates lattice management system for testing
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateLatticeManagementSystem()
            {
                var package = CreateProjectServicesSystem();
                package.LatticeManager = (ILatticeManager) package.ProjectServices.CreateAndRegister(new LatticeManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates a management system that is capable of the modeling process until transition inputs
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateTransitionManagementSystem()
            {
                var package = CreateStructureManagementSystem();
                package.TransitionManager = (ITransitionManager) package.ProjectServices.CreateAndRegister(new TransitionManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates a management system that is capable of the modeling process until energy inputs
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateEnergyManagementSystem()
            {
                var package = CreateTransitionManagementSystem();
                package.EnergyManager = (IEnergyManager) package.ProjectServices.CreateAndRegister(new EnergyManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates a management system that is capable of the modeling process until simulation inputs
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateSimulationManagementPackage()
            {
                var package = CreateEnergyManagementSystem();
                package.SimulationManager = (ISimulationManager) package.ProjectServices.CreateAndRegister(new SimulationManagerFactory());
                return package;
            }

            /// <summary>
            ///     Creates the currently most developed management system
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateFullManagementSystem()
            {
                return CreateSimulationManagementPackage();
            }

            /// <summary>
            ///     Creates a new management system for testing purposes that contains particle and structure information of ceria
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateManageSystemForCeria()
            {
                var package = CreateFullManagementSystem();
                var inputSystem = MakeCeriaInputSystem();
                inputSystem.AutoInputData(package.ProjectServices);
                package.InputReportJson = inputSystem.GetReportJson();
                return package;
            }

            /// <summary>
            ///     Makes an auto data input system that carries the ceria related data fro debug testing
            /// </summary>
            /// <returns></returns>
            public static ProjectDataInputSystem MakeCeriaInputSystem()
            {
                var particles = new[]
                {
                    new Particle {Index = 0},
                    new Particle {Name = "Vacancy", Symbol = "Vc", Charge = 0.0, IsVacancy = true, Index = 1},
                    new Particle {Name = "Oxygen", Symbol = "O", Charge = -2.0, IsVacancy = false, Index = 2},
                    new Particle {Name = "Cer", Symbol = "Ce", Charge = 4.0, IsVacancy = false, Index = 3},
                    new Particle {Name = "Yttrium", Symbol = "Y", Charge = 3.0, IsVacancy = false, Index = 4},
                    new Particle {Name = "Cer", Symbol = "Ce", Charge = 3.0, IsVacancy = false, Index = 5},
                    new Particle {Name = "Zirconium", Symbol = "Zr", Charge = 4.0, IsVacancy = false, Index = 6},
                    new Particle {Name = "Zirconium", Symbol = "Zr", Charge = 3.0, IsVacancy = false, Index = 7}
                };
                var particleSets = new[]
                {
                    new ParticleSet {Particles = new List<IParticle> {particles[1], particles[2]}, Index = 1},
                    new ParticleSet
                        {Particles = new List<IParticle> {particles[3], particles[4], particles[5], particles[6], particles[7]}, Index = 2},
                    new ParticleSet {Particles = new List<IParticle> {particles[0], particles[2]}, Index = 3}
                };
                var unitCellPositions = new[]
                {
                    new UnitCellPosition
                    {
                        Vector = new DataVector3D(0, 0, 0), OccupationSet = particleSets[1], Status = PositionStatus.Stable, Index = 0
                    },
                    new UnitCellPosition
                    {
                        Vector = new DataVector3D(0.25, 0.25, 0.25), OccupationSet = particleSets[0], Status = PositionStatus.Stable,
                        Index = 1
                    },
                    new UnitCellPosition
                    {
                        Vector = new DataVector3D(0.50, 0.25, 0.25), OccupationSet = particleSets[2], Status = PositionStatus.Unstable,
                        Index = 2
                    }
                };
                var propertyPairs = new[]
                {
                    new StateExchangePair {DonorParticle = particles[2], AcceptorParticle = particles[0], IsVacancyPair = false, Index = 0},
                    new StateExchangePair {DonorParticle = particles[2], AcceptorParticle = particles[1], IsVacancyPair = true, Index = 1},
                    new StateExchangePair {DonorParticle = particles[3], AcceptorParticle = particles[4], IsVacancyPair = false, Index = 2},
                    new StateExchangePair {DonorParticle = particles[3], AcceptorParticle = particles[5], IsVacancyPair = false, Index = 3},
                    new StateExchangePair {DonorParticle = particles[3], AcceptorParticle = particles[7], IsVacancyPair = false, Index = 4},
                    new StateExchangePair {DonorParticle = particles[6], AcceptorParticle = particles[7], IsVacancyPair = false, Index = 5},
                    new StateExchangePair {DonorParticle = particles[6], AcceptorParticle = particles[5], IsVacancyPair = false, Index = 6},
                    new StateExchangePair {DonorParticle = particles[3], AcceptorParticle = particles[6], IsVacancyPair = false, Index = 7},
                    new StateExchangePair {DonorParticle = particles[5], AcceptorParticle = particles[7], IsVacancyPair = false, Index = 8}
                };
                var propertyGroups = new[]
                {
                    new StateExchangeGroup
                        {VacancyGroup = false, Index = 0, StateExchangePairs = new List<IStateExchangePair> {propertyPairs[0]}},
                    new StateExchangeGroup
                        {VacancyGroup = true, Index = 1, StateExchangePairs = new List<IStateExchangePair> {propertyPairs[1]}},
                    new StateExchangeGroup
                        {VacancyGroup = false, Index = 2, StateExchangePairs = new List<IStateExchangePair> {propertyPairs[2]}},
                    new StateExchangeGroup
                    {
                        VacancyGroup = false, Index = 3, StateExchangePairs = new List<IStateExchangePair>
                        {
                            propertyPairs[3], propertyPairs[4], propertyPairs[5], propertyPairs[6], propertyPairs[7], propertyPairs[8]
                        }
                    }
                };
                var abstractTransitions = new[]
                {
                    new AbstractTransition
                    {
                        Index = 0, Name = "OxygenMigration",
                        StateExchangeGroups = new List<IStateExchangeGroup> {propertyGroups[1], propertyGroups[0], propertyGroups[1]},
                        Connectors = new List<ConnectorType> {ConnectorType.Dynamic, ConnectorType.Dynamic}
                    },
                    new AbstractTransition
                    {
                        Index = 1, Name = "KationExchange",
                        StateExchangeGroups = new List<IStateExchangeGroup> {propertyGroups[2], propertyGroups[2]},
                        Connectors = new List<ConnectorType> {ConnectorType.Dynamic}
                    },
                    new AbstractTransition
                    {
                        Index = 2, Name = "OxygenExchange",
                        StateExchangeGroups = new List<IStateExchangeGroup> {propertyGroups[1], propertyGroups[1]},
                        Connectors = new List<ConnectorType> {ConnectorType.Dynamic}
                    },
                    new AbstractTransition
                    {
                        Index = 3, Name = "FullKationMmc",
                        StateExchangeGroups = new List<IStateExchangeGroup> {propertyGroups[3], propertyGroups[3]},
                        Connectors = new List<ConnectorType> {ConnectorType.Dynamic}
                    }
                };
                var metropolisTransitions = new[]
                {
                    new MetropolisTransition
                    {
                        Index = 0, AbstractTransition = abstractTransitions[1],
                        FirstUnitCellPosition = unitCellPositions[0], SecondUnitCellPosition = unitCellPositions[0]
                    },
                    new MetropolisTransition
                    {
                        Index = 1, AbstractTransition = abstractTransitions[2],
                        FirstUnitCellPosition = unitCellPositions[1], SecondUnitCellPosition = unitCellPositions[1]
                    },
                    new MetropolisTransition
                    {
                        Index = 2, AbstractTransition = abstractTransitions[3],
                        FirstUnitCellPosition = unitCellPositions[0], SecondUnitCellPosition = unitCellPositions[0]
                    }
                };
                var kineticTransitions = new[]
                {
                    new KineticTransition
                    {
                        Index = 0, AbstractTransition = abstractTransitions[0], PathGeometry = new List<DataVector3D>
                        {
                            new DataVector3D(.25, .25, .25), new DataVector3D(.5, .25, .25), new DataVector3D(.75, .25, .25)
                        }
                    }
                };
                var groupInteractions = new[]
                {
                    new GroupInteraction
                    {
                        CenterUnitCellPosition = unitCellPositions[1],
                        GeometryVectors = new List<DataVector3D>
                        {
                            new DataVector3D(0.75, 0.75, 0.75), new DataVector3D(-0.25, -0.25, -0.25)
                        }
                    },
                    new GroupInteraction
                    {
                        CenterUnitCellPosition = unitCellPositions[0],
                        GeometryVectors = new List<DataVector3D>
                        {
                            new DataVector3D(0.25, 0.25, 0.25), new DataVector3D(-0.25, -0.25, -0.25)
                        }
                    }
                };
                var metropolisSimulations = new[]
                {
                    new MetropolisSimulation
                    {
                        BaseFlags = SimulationBaseFlags.UseCheckpointSystem,
                        WriteOutCount = 100,
                        Temperature = 1000,
                        JobCount = 10,
                        LowerSuccessRateLimit = 10,
                        TargetMcsp = 200,
                        Name = "Metropolis",
                        RelativeBreakTolerance = 0.0001,
                        BreakSampleIntervalMcs = 100,
                        BreakSampleLength = 10000,
                        ResultSampleMcs = 10000,
                        Transitions = metropolisTransitions.Cast<IMetropolisTransition>().ToList()
                    }
                };
                var kineticSimulations = new[]
                {
                    new KineticSimulation
                    {
                        WriteOutCount = 100,
                        Name = "Kinetic",
                        Temperature = 1000,
                        JobCount = 10,
                        LowerSuccessRateLimit = 10,
                        TargetMcsp = 200,
                        NormalizationProbability = 1.0,
                        KineticFlags = KineticSimulationFlags.UseDynamicTrackers | KineticSimulationFlags.UseStaticTrackers,
                        CustomRngSeed = Guid.NewGuid().ToString(),
                        ElectricFieldMagnitude = 10000000.0,
                        ElectricFieldVector = new DataVector3D(1, 0, 0),
                        Transitions = kineticTransitions.Cast<IKineticTransition>().ToList()
                    }
                };

                var inputter = new ProjectDataInputSystem();
                inputter.AddMany(particles.Skip(1));
                inputter.AddMany(particleSets);
                inputter.Add(new StructureInfo {Name = "Ceria"});
                inputter.Add(new SpaceGroupInfo {GroupEntry = new SpaceGroupEntry(225, "Fm-3m", "None")});
                inputter.Add(new CellParameters {ParameterSet = new CrystalParameterSet(5.411, 5.411, 5.411, 0, 0, 0)});
                inputter.AddMany(unitCellPositions);
                inputter.AddMany(propertyPairs);
                inputter.AddMany(propertyGroups);
                inputter.AddMany(abstractTransitions);
                inputter.AddMany(metropolisTransitions);
                inputter.AddMany(kineticTransitions);
                inputter.Add(new StableEnvironmentInfo
                    {MaxInteractionRange = 6.5, IgnoredPairInteractions = new List<SymmetricParticlePair>()});
                inputter.Add(new UnstableEnvironment
                {
                    Index = 0,
                    MaxInteractionRange = 1.5,
                    UnitCellPosition = unitCellPositions[2],
                    IgnoredPositions = new List<IUnitCellPosition>()
                });
                inputter.AddMany(groupInteractions);
                inputter.AddMany(metropolisSimulations);
                inputter.AddMany(kineticSimulations);
                return inputter;
            }
        }
    }
}
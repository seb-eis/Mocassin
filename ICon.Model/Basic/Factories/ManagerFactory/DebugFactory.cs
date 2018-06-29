﻿using System;
using System.Collections.Generic;
using System.Text;

using ICon.Mathematics.ValueTypes;
using ICon.Model.ProjectServices;
using ICon.Model.Particles;
using ICon.Model.Structures;
using ICon.Model.Transitions;
using ICon.Model.Energies;
using ICon.Model.Lattices;
using ICon.Model.DataManagement;
using ICon.Model.Basic.Debug;

namespace ICon.Model.Basic
{
    public static partial class ManagerFactory
    {
        /// <summary>
        /// Debug factory that directly creates all dependent managers
        /// </summary>
        public static class DebugFactory
        {
            /// <summary>
            /// Create a project service system for testing
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateProjectServicesSystem()
            {
                return new ManagerPackage()
                {
                    ProjectServices = ProjectServicesManager.Create(ProjectSettingsData.CreateDefault())
                };
            }

            /// <summary>
            /// Creates and links default project services and particle manager for testing
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateParticleManagementSystem()
            {
                var package = CreateProjectServicesSystem();
                package.ParticleManager = (IParticleManager)package.ProjectServices.CreateAndRegister(new ParticleManagerFactory());
                return package;
            }

            /// <summary>
            /// Creates structure management system for testing
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateStructureManagementSystem()
            {
                var package = CreateParticleManagementSystem();
                package.StructureManager = (IStructureManager)package.ProjectServices.CreateAndRegister(new StructureManagerFactory());
                return package;
            }

            /// <summary>
            /// Creates lattice management system for testing
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateLatticeManagementSystem()
            {
                var package = CreateEnergyManagementSystem();
                package.LatticeManager = (ILatticeManager)package.ProjectServices.CreateAndRegister(new LatticeManagerFactory());
                return package;
            }

            /// <summary>
            /// Creates a mangement system that is capable of the modelling process until transition inputs
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateTransitionManagementSystem()
            {
                var package = CreateStructureManagementSystem();
                package.TransitionManager = (Transitions.ITransitionManager)package.ProjectServices.CreateAndRegister(new TransitionManagerFactory());
                return package;
            }

            /// <summary>
            /// Creates a mangement system that is capable of the modelling process until energy inputs
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateEnergyManagementSystem()
            {
                var package = CreateTransitionManagementSystem();
                package.EnergyManager = (Energies.IEnergyManager)package.ProjectServices.CreateAndRegister(new EnergyManagerFactory());
                return package;
            }

            /// <summary>
            /// Creates the currently most develepped management system
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateFullManagementSystem()
            {
                return CreateLatticeManagementSystem();
            }

            /// <summary>
            /// Cretes a new management system for testing purposes that contains particle and structure information of ceria
            /// </summary>
            /// <returns></returns>
            public static ManagerPackage CreateManageSystemForCeria()
            {
                var package = CreateFullManagementSystem();
                var inputter = MakeCeriaDataInputter();
                inputter.AutoInputData(package.ProjectServices);
                return package;
            }

            /// <summary>
            /// Makes an auto data inputter that carries the ceria related data fro debug testing
            /// </summary>
            /// <returns></returns>
            public static ManagerDataInputter MakeCeriaDataInputter()
            {
                var particles = new Particle[]
                {
                    new Particle() { Index = 0 },
                    new Particle() { Name = "Vacancy", Symbol = "Vc", Charge = 0.0, IsVacancy = true, Index = 1 },
                    new Particle() { Name = "Oxygen", Symbol = "O", Charge = -2.0, IsVacancy = false, Index = 2 },
                    new Particle() { Name = "Cer", Symbol = "Ce", Charge = 4.0, IsVacancy = false, Index = 3 },
                    new Particle() { Name = "Yttrium", Symbol = "Y", Charge = 3.0, IsVacancy = false, Index = 4 },
                    new Particle() { Name = "Polaron", Symbol = "Ce", Charge = 3.0, IsVacancy = false, Index = 5 }
                };
                var particleSets = new ParticleSet[]
                {
                    new ParticleSet () { Particles = new List<IParticle> { particles[1], particles[2] }, Index = 1 },
                    new ParticleSet () { Particles = new List<IParticle> { particles[3], particles[4], particles[5] }, Index = 2 },
                    new ParticleSet () { Particles = new List<IParticle> { particles[0], particles[2] }, Index = 3 },
                };
                var unitCellPositions = new UnitCellPosition[]
                {
                    new UnitCellPosition()
                    {
                        Vector = new DataVector3D(0, 0, 0), OccupationSet = particleSets[1] , Status = PositionStatus.Stable, Index = 0
                    },
                    new UnitCellPosition()
                    {
                        Vector = new DataVector3D(0.25, 0.25, 0.25), OccupationSet = particleSets[0], Status = PositionStatus.Stable, Index = 1
                    },
                    new UnitCellPosition()
                    {
                        Vector = new DataVector3D(0.50, 0.25, 0.25), OccupationSet = particleSets[2], Status = PositionStatus.Unstable, Index = 2
                    }
                };
                var propertyPairs = new PropertyStatePair[]
                {
                    new PropertyStatePair() { DonorParticle = particles[2], AcceptorParticle = particles[0], IsVacancyPair = false, Index = 0 },
                    new PropertyStatePair() { DonorParticle = particles[2], AcceptorParticle = particles[1], IsVacancyPair = true, Index = 1 },
                    new PropertyStatePair() { DonorParticle = particles[3], AcceptorParticle = particles[4], IsVacancyPair = false, Index = 2},
                    new PropertyStatePair() { DonorParticle = particles[5], AcceptorParticle = particles[3], IsVacancyPair = false, Index = 3}
                };
                var propertyGroups = new PropertyGroup[]
                {
                    new PropertyGroup() { VacancyGroup = false, Index = 0, ChargeTransfer = -2, PropertyStatePairs = new List<IPropertyStatePair>{ propertyPairs[0]} },
                    new PropertyGroup() { VacancyGroup = true, Index = 1, ChargeTransfer = -2, PropertyStatePairs = new List<IPropertyStatePair>{ propertyPairs[1]} },
                    new PropertyGroup() { VacancyGroup = false, Index = 2, ChargeTransfer = -1, PropertyStatePairs = new List<IPropertyStatePair>{ propertyPairs[2]} },
                    new PropertyGroup() { VacancyGroup = false, Index = 3, ChargeTransfer = -1, PropertyStatePairs = new List<IPropertyStatePair>{ propertyPairs[3]} }
                };
                var metropolisTransitions = new MetropolisTransition[]
                {
                    new MetropolisTransition() { Index = 0, CellPosition0 = unitCellPositions[0], CellPosition1 = unitCellPositions[0]},
                    new MetropolisTransition() { Index = 0, CellPosition0 = unitCellPositions[1], CellPosition1 = unitCellPositions[1]}
                };
                var abstractTransitions = new AbstractTransition[]
                {
                    new AbstractTransition()
                    {
                        Index = 0, Name = "OxygenMigration",
                        PropertyGroups = new List<IPropertyGroup>{ propertyGroups[1], propertyGroups[0], propertyGroups[1] },
                        Connectors = new List<ConnectorType>{ ConnectorType.Dynamic, ConnectorType.Dynamic }
                    }
                };
                var kineticTransitions = new KineticTransition[]
                {
                    new KineticTransition()
                    {
                        Index = 0, AbstractTransition = abstractTransitions[0], PathGeometry = new List<DataVector3D>
                        {
                            new DataVector3D(.25,.25,.25), new DataVector3D(.5,.25,.25), new DataVector3D(.75,.25,.25)
                        }
                    }
                };
                var buildingBlocks = new BuildingBlock[]
                {
                    new BuildingBlock()
                    {
                        Index = 0,
                        CellEntries = new List<IParticle>()
                        {
                            particles[3], particles[0], particles[0], particles[3],
                            particles[0], particles[0], particles[0], particles[0],
                            particles[0], particles[2], particles[0], particles[2],
                            particles[0], particles[0], particles[0], particles[2], particles[0], particles[2],
                            particles[3], particles[0], particles[0], particles[3],
                            particles[0], particles[0], particles[0], particles[0],
                            particles[0], particles[2], particles[0], particles[2],
                            particles[0], particles[0], particles[0], particles[2], particles[0], particles[2]
                        }
                    },

                    new BuildingBlock()
                    {
                        Index = 1,
                        CellEntries = new List<IParticle>()
                        {
                            particles[4], particles[0], particles[0], particles[4],
                            particles[0], particles[0], particles[0], particles[0],
                            particles[0], particles[2], particles[0], particles[2],
                            particles[0], particles[0], particles[0], particles[2], particles[0], particles[1],
                            particles[4], particles[0], particles[0], particles[4],
                            particles[0], particles[0], particles[0], particles[0],
                            particles[0], particles[1], particles[0], particles[2],
                            particles[0], particles[0], particles[0], particles[2], particles[0], particles[2]
                        }
                    }
                };
                var blockInfos = new BlockInfo[]
                {
                    new BlockInfo()
                    {
                        Index = 0, BlockAssembly = new List<IBuildingBlock>() {buildingBlocks[0]},
                        Origin = new DataIntVector3D(0,0,0),
                        Extent = new DataIntVector3D(10,10,10),
                        Size = new DataIntVector3D(1,1,1)
                    },
                    new BlockInfo()
                    {
                        Index = 1,
                        BlockAssembly = new List<IBuildingBlock>()
                        {
                            buildingBlocks[0], buildingBlocks[1],
                            buildingBlocks[0], buildingBlocks[1],
                            buildingBlocks[0], buildingBlocks[1],
                            buildingBlocks[0], buildingBlocks[1],
                        },
                        Origin = new DataIntVector3D(0,0,0),
                        Extent = new DataIntVector3D(2,2,10),
                        Size = new DataIntVector3D(2,2,2)
                    }
                };
                var dopingCombinations = new DopingCombination[]
                {
                    new DopingCombination()
                    {
                         Index = 0, BuildingBlock = buildingBlocks[0], Dopant = particles[5], DopedParticle = particles[3], UnitCellPosition = unitCellPositions[0]
                    },
                    new DopingCombination()
                    {
                         Index = 1, BuildingBlock = buildingBlocks[0], Dopant = particles[1], DopedParticle = particles[2], UnitCellPosition = unitCellPositions[1]
                    }
                };
                var dopings = new Doping[]
                {
                    new Doping()
                    {
                        Index = 0, Concentration = 1.0, DopingInfo = dopingCombinations[0], CounterDopingInfo = dopingCombinations[1]
                    }
                };

                var inputter = new ManagerDataInputter()
                {
                    particles[1], particles[2], particles[3], particles[4], particles[5],

                    particleSets[0], particleSets[1], particleSets[2],

                    new StructureInfo() { Name = "Ceria" },

                    new SpaceGroupInfo() { GroupEntry = new Symmetry.SpaceGroups.SpaceGroupEntry(225, "Fm-3m", "None") },

                    new CellParameters() { ParameterSet = new Symmetry.CrystalSystems.CrystalParameterSet(5.411, 5.411, 5.411, 0, 0, 0) },

                    unitCellPositions[0], unitCellPositions[1], unitCellPositions[2],

                    propertyPairs[0], propertyPairs[1], propertyPairs[2], propertyPairs[3],

                    propertyGroups[0], propertyGroups[1], propertyGroups[2], propertyGroups[3],

                    abstractTransitions[0],

                    metropolisTransitions[0], metropolisTransitions[1],

                    kineticTransitions[0],

                    new StableEnvironmentInfo() { MaxInteractionRange = 6.5, IgnoredPairInteractions = new List<SymParticlePair>()},

                    new UnstableEnvironment() { Index = 0, MaxInteractionRange = 1.5, UnitCellPosition = unitCellPositions[2], IgnoredPositions = new List<IUnitCellPosition>()},

                    new LatticeInfo() { Extent = new DataIntVector3D(10, 10 ,10)},

                    buildingBlocks[0], buildingBlocks[1],

                    blockInfos[0], blockInfos[1],

                    dopingCombinations[0], dopingCombinations[1],

                    dopings[0]
                };
                return inputter;
            }
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Events;
using Mocassin.Framework.SQLiteCore;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.EntityBuilder;
using Mocassin.Model.Translator.Jobs;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.UI.Xml.Customization;
using Mocassin.UI.Xml.Jobs;
using Mocassin.UI.Xml.Main;
using Mocassin.UI.Xml.Model;

namespace Mocassin.UI.Xml.ProjectBuilding
{
    /// <summary>
    ///     The enumeration to signal about the current status of the <see cref="ISimulationLibrary" /> building
    ///     process
    /// </summary>
    public enum LibraryBuildStatus
    {
        Unknown,
        Error,
        BuildProcessCompleted,
        BuildProcessStarted,
        PreparingModelProject,
        ModelProjectPreparationError,
        BuildingModelContext,
        ModelContextBuildError,
        PreparingModelCustomization,
        ModelCustomizationPreparationError,
        PreparingLibrary,
        LibraryPreparationError,
        BuildingLibrary,
        LibraryBuildingError,
        AddingLibraryMetaData,
        MetaDataAddError,
        SavingLibraryContents,
        LibraryContentSavingError
    }

    /// <summary>
    ///     Build system that translates <see cref="MocassinProjectBuildGraph" /> instances into
    ///     <see cref="ISimulationLibrary" /> interfaces
    /// </summary>
    public class MocassinSimulationLibraryBuilder
    {
        private readonly object lockObject = new object();
        private int BuildCounter { get; set; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for distribution of <see cref="LibraryBuildStatus" /> notifications
        /// </summary>
        private ReactiveEvent<LibraryBuildStatus> BuildStatusEvent { get; }

        /// <summary>
        ///     Get the <see cref="ReactiveEvent{TSubject}" /> for distribution of job build notifications
        /// </summary>
        private ReactiveEvent<(int Done, int Total)> JobBuildCounterEvent { get; }

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that informs about changes in the <see cref="LibraryBuildStatus" /> and
        ///     distributes caught <see cref="Exception" />
        /// </summary>
        public IObservable<LibraryBuildStatus> LibraryBuildStatusNotifications => BuildStatusEvent.AsObservable();

        /// <summary>
        ///     Get the <see cref="IObservable{T}" /> that informs about changes in the number of successfully build job entities
        /// </summary>
        public IObservable<(int Done, int Total)> JobBuildCounterNotifications => JobBuildCounterEvent.AsObservable();

        /// <summary>
        ///     Creates a new <see cref="MocassinSimulationLibraryBuilder" />
        /// </summary>
        public MocassinSimulationLibraryBuilder()
        {
            BuildStatusEvent = new ReactiveEvent<LibraryBuildStatus>();
            JobBuildCounterEvent = new ReactiveEvent<(int Done, int Total)>();
        }


        /// <summary>
        ///     Executes a full translation cycle of a <see cref="MocassinProjectBuildGraph" /> into a
        ///     <see cref="ISimulationLibrary" /> using the passed file path <see cref="string" /> and
        ///     <see cref="IModelProject" /> interface
        /// </summary>
        /// <param name="projectBuildGraph"></param>
        /// <param name="filePath"></param>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public ISimulationLibrary BuildLibrary(MocassinProjectBuildGraph projectBuildGraph, string filePath,
            IModelProject modelProject)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.BuildProcessStarted);

            if (!TryPrepareLibraryContext(filePath, out var libraryContext))
                return null;
            if (!TryPrepareModelProject(projectBuildGraph.ProjectModelGraph, modelProject))
                return null;
            if (!TryPrepareModelCustomization(modelProject, projectBuildGraph.ProjectCustomizationGraph))
                return null;
            if (!TryBuildModelContext(modelProject, out var modelContext))
                return null;
            if (!TryBuildLibraryContent(modelContext, projectBuildGraph.ProjectJobTranslationGraph, out var jobPackageModels))
                return null;
            if (!TryAddBuildMetaData(jobPackageModels, projectBuildGraph))
                return null;
            if (!TrySaveLibraryContents(libraryContext, jobPackageModels))
                return null;

            BuildStatusEvent.OnNext(LibraryBuildStatus.BuildProcessCompleted);
            return libraryContext;
        }

        /// <summary>
        ///     Tries to prepare a <see cref="IModelProject" /> using the <see cref="ProjectModelGraph" /> data object
        /// </summary>
        /// <param name="projectModel"></param>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        private bool TryPrepareModelProject(ProjectModelGraph projectModel, IModelProject modelProject)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.PreparingModelProject);

            try
            {
                modelProject.ResetProject();
                var reports = modelProject.InputPipeline.PushToProject(projectModel.GetInputSequence());
                if (reports.Any(x => !x.IsGood)) throw new InvalidOperationException("Translation of invalid model.");
                return true;
            }
            catch (Exception e)
            {
                BuildStatusEvent.OnNext(LibraryBuildStatus.ModelProjectPreparationError);
                BuildStatusEvent.OnError(e);
                modelProject.ResetProject();
                return false;
            }
        }

        /// <summary>
        ///     Tries to prepare the customization of a prepared <see cref="IModelProject" /> using the provided
        ///     <see cref="ModelCustomizationEntity" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="modelCustomization"></param>
        /// <returns></returns>
        private bool TryPrepareModelCustomization(IModelProject modelProject, ModelCustomizationEntity modelCustomization)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.PreparingModelCustomization);

            try
            {
                modelCustomization.PushToModel(modelProject);
                return true;
            }
            catch (Exception e)
            {
                BuildStatusEvent.OnNext(LibraryBuildStatus.ModelCustomizationPreparationError);
                BuildStatusEvent.OnError(e);
                modelProject.ResetProject();
                return false;
            }
        }

        /// <summary>
        ///     Tries to build a symmetry extended <see cref="IProjectModelContext" /> for the provided and prepared
        ///     <see cref="IModelProject" />
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="modelContext"></param>
        /// <returns></returns>
        private bool TryBuildModelContext(IModelProject modelProject, out IProjectModelContext modelContext)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.BuildingModelContext);
            try
            {
                var builder = new ProjectModelContextBuilder(modelProject);
                modelContext = builder.BuildContextAsync().Result;
                return true;
            }
            catch (Exception e)
            {
                BuildStatusEvent.OnNext(LibraryBuildStatus.ModelContextBuildError);
                BuildStatusEvent.OnError(e);
                modelContext = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to prepare a new <see cref="SimulationDbContext" /> using th provided file path <see cref="string" />
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="dbContext"></param>
        /// <returns></returns>
        private bool TryPrepareLibraryContext(string filePath, out SimulationDbContext dbContext)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.PreparingLibrary);

            try
            {
                dbContext = SqLiteContext.OpenDatabase<SimulationDbContext>(filePath, true);
                return true;
            }
            catch (Exception e)
            {
                BuildStatusEvent.OnNext(LibraryBuildStatus.LibraryPreparationError);
                BuildStatusEvent.OnError(e);
                dbContext = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to build the <see cref="IList{T}" /> of <see cref="SimulationJobPackageModel" /> defined by a
        ///     <see cref="ProjectJobTranslationGraph" /> using the provided <see cref="IProjectModelContext" />
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="jobTranslation"></param>
        /// <param name="jobPackageModels"></param>
        /// <returns></returns>
        private bool TryBuildLibraryContent(IProjectModelContext modelContext, ProjectJobTranslationGraph jobTranslation,
            out IList<SimulationJobPackageModel> jobPackageModels)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.BuildingLibrary);

            try
            {
                BuildCounter = 0;
                var totalJobCount = jobTranslation.GetTotalJobCount(modelContext.ModelProject);
                jobPackageModels = jobTranslation.ToInternals(modelContext.ModelProject)
                    .Select(jobs => GetPreparedJobBuilder(modelContext, jobs, totalJobCount).BuildJobPackageModel(jobs))
                    .ToList();

                return true;
            }
            catch (Exception e)
            {
                BuildStatusEvent.OnNext(LibraryBuildStatus.LibraryBuildingError);
                BuildStatusEvent.OnError(e);
                jobPackageModels = null;
                return false;
            }
        }

        /// <summary>
        ///     Tries to save an <see cref="IEnumerable{T}" /> sequence of <see cref="SimulationJobPackageModel" /> instances to
        ///     the passed <see cref="SimulationDbContext" />
        /// </summary>
        /// <param name="dbContext"></param>
        /// <param name="jobPackageModels"></param>
        /// <returns></returns>
        private bool TrySaveLibraryContents(SimulationDbContext dbContext,
            IEnumerable<SimulationJobPackageModel> jobPackageModels)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.SavingLibraryContents);

            try
            {
                dbContext.AddRange(jobPackageModels);
                dbContext.SaveChanges();
                return true;
            }
            catch (Exception e)
            {
                BuildStatusEvent.OnNext(LibraryBuildStatus.LibraryContentSavingError);
                BuildStatusEvent.OnError(e);
                return false;
            }
        }

        /// <summary>
        ///     Tries to add all meta information of the <see cref="MocassinProjectBuildGraph" /> to the prepared set of
        ///     <see cref="SimulationJobPackageModel" /> instances
        /// </summary>
        /// <param name="jobPackageModels"></param>
        /// <param name="buildGraph"></param>
        /// <returns></returns>
        private bool TryAddBuildMetaData(IEnumerable<SimulationJobPackageModel> jobPackageModels, MocassinProjectBuildGraph buildGraph)
        {
            BuildStatusEvent.OnNext(LibraryBuildStatus.AddingLibraryMetaData);

            try
            {
                var buildXml = buildGraph.ToXml();
                var version = Assembly.GetExecutingAssembly().GetName().Version.ToString();
                foreach (var packageModel in jobPackageModels)
                {
                    packageModel.ProjectXml = buildXml;
                    packageModel.ProjectGuid = buildGraph.ProjectModelGraph.Key;
                    packageModel.ModelSystemVersion = version;
                }

                return true;
            }
            catch (Exception e)
            {
                BuildStatusEvent.OnNext(LibraryBuildStatus.MetaDataAddError);
                BuildStatusEvent.OnError(e);
                return false;
            }
        }

        /// <summary>
        ///     Prepares a <see cref="IJobDbEntityBuilder" /> that can process the passed combination of
        ///     <see cref="IProjectModelContext" /> and <see cref="IJobCollection" />
        /// </summary>
        /// <param name="modelContext"></param>
        /// <param name="jobCollection"></param>
        /// <param name="totalJobCount"></param>
        /// <returns></returns>
        private IJobDbEntityBuilder GetPreparedJobBuilder(IProjectModelContext modelContext, IJobCollection jobCollection,
            int totalJobCount)
        {
            var builder = new JobDbEntityBuilder(modelContext);
            builder.WhenJobIsBuild.Subscribe(x =>
            {
                lock (lockObject)
                {
                    BuildCounter++;
                }

                JobBuildCounterEvent.OnNext((BuildCounter, totalJobCount));
            });

            foreach (var optimizer in jobCollection.GetPostBuildOptimizers()) builder.AddPostBuildOptimizer(optimizer);

            return builder;
        }
    }
}
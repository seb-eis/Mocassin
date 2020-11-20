using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using Mocassin.Mathematics.ValueTypes;
using Mocassin.Model.DataManagement;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator;
using Mocassin.Model.Translator.Database.Entities.Other.Meta;
using Mocassin.Model.Translator.ModelContext;
using Mocassin.UI.Data.Base;
using Mocassin.UI.Data.Main;

namespace Mocassin.UI.Data.Helper
{
    /// <summary>
    ///     Provides helper methods for dealing with entities from <see cref="ISimulationLibrary" /> or translating them back
    ///     to data objects
    /// </summary>
    public static class MslHelper
    {
        /// <summary>
        ///     Restores the <see cref="IModelProject" /> using the project XML on the provided
        ///     <see cref="SimulationJobPackageModel" /> and model provider function
        /// </summary>
        /// <param name="jobPackage"></param>
        /// <param name="modelProjectProviderFunc"></param>
        /// <returns></returns>
        public static IModelProject RestoreModelProject(SimulationJobPackageModel jobPackage, Func<IModelProject> modelProjectProviderFunc)
        {
            var project = modelProjectProviderFunc.Invoke();
            var dataObj = ProjectDataObject.CreateFromXml<SimulationDbBuildTemplate>(jobPackage.ProjectXml);
            var modelPushResult = project.InputPipeline.PushToProject(dataObj.ProjectModelData.GetInputSequence());
            if (modelPushResult.Any(x => !x.IsGood)) throw new InvalidOperationException("The project Xml is damaged or invalid.");
            dataObj.ProjectCustomizationTemplate.PushToModel(project);
            return project;
        }

        /// <summary>
        ///     Restores the <see cref="IModelProject" /> using the project XML on the provided
        ///     <see cref="SimulationJobPackageModel" /> and a default build <see cref="IModelProject" />
        /// </summary>
        /// <param name="jobPackage"></param>
        /// <returns></returns>
        public static IModelProject RestoreModelProject(SimulationJobPackageModel jobPackage) => RestoreModelProject(jobPackage, ModelProjectFactory.CreateDefault);

        /// <summary>
        ///     Restores the <see cref="IProjectModelContext" /> using the project XML on the provided
        ///     <see cref="SimulationJobPackageModel" /> and model provider function
        /// </summary>
        /// <param name="jobPackage"></param>
        /// <param name="modelProjectProviderFunc"></param>
        /// <returns></returns>
        public static IProjectModelContext RestoreModelContext(SimulationJobPackageModel jobPackage, Func<IModelProject> modelProjectProviderFunc)
        {
            var project = RestoreModelProject(jobPackage, modelProjectProviderFunc);
            var contextBuilder = new ProjectModelContextBuilder(project);
            return contextBuilder.Build();
        }

        /// <summary>
        ///     Restores the <see cref="IProjectModelContext" /> using the project XML on the provided
        ///     <see cref="SimulationJobPackageModel" /> and a default build <see cref="IModelProject" />
        /// </summary>
        /// <param name="jobPackage"></param>
        /// <returns></returns>
        public static IProjectModelContext RestoreModelContext(SimulationJobPackageModel jobPackage) => RestoreModelContext(jobPackage, ModelProjectFactory.CreateDefault);

        /// <summary>
        ///     Parses the <see cref="Vector4I"/> that describes the supercell size A,B,C,P from a <see cref="IJobMetaData"/> with an optional flag to ignore the 4th dimension
        /// </summary>
        /// <param name="jobMetaData"></param>
        /// <param name="ignoreDimension4"></param>
        /// <returns></returns>
        public static Vector4I ParseLatticeSizeInfo(IJobMetaData jobMetaData, bool ignoreDimension4 = false)
        {
            var split = jobMetaData.LatticeInfo.Split(',');
            return split.Length switch
            {
                3 when ignoreDimension4 => new Vector4I(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), 0),
                4 when !ignoreDimension4 => new Vector4I(int.Parse(split[0]), int.Parse(split[1]), int.Parse(split[2]), int.Parse(split[3])),
                _ => throw new FormatException($"The string {jobMetaData.LatticeInfo} could not be parsed.")
            };
        }
    }
}
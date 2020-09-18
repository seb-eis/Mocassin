﻿using System.Xml.Serialization;
using Mocassin.Model.ModelProject;
using Mocassin.Model.Translator.Optimization;
using Mocassin.UI.Xml.Base;

namespace Mocassin.UI.Xml.Jobs
{
    /// <summary>
    ///     Serializable base class that enables manual definition of <see cref="IPostBuildOptimizer" /> interfaces for
    ///     <see cref="JobPackageData" /> objects
    /// </summary>
    [XmlRoot]
    public abstract class ManualOptimizerData : ProjectDataObject
    {
        /// <summary>
        ///     Uses the passed <see cref="IModelProject" /> to build an <see cref="IPostBuildOptimizer" /> from the set data
        /// </summary>
        /// <param name="modelProject"></param>
        /// <returns></returns>
        public abstract IPostBuildOptimizer ToInternal(IModelProject modelProject);
    }
}
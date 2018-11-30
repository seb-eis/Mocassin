using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Processing;
using Mocassin.Framework.Reflection;
using Mocassin.Model.Mml.Exceptions;

namespace Mocassin.Model.Mml.Descriptions
{
    /// <summary>
    ///     Reflective description source that supports description handling trough automated object pipeline generation
    /// </summary>
    public abstract class PipelineDescriptionSource : DescriptionSource
    {
        /// <summary>
        ///     The conversion dictionary that stores delegates for all supported types
        /// </summary>
        private readonly BreakPipeline<string> _conversionPipeline;

        /// <summary>
        ///     Creates new reflective description source and automatically builds the conversion dictionary
        /// </summary>
        protected PipelineDescriptionSource()
        {
            _conversionPipeline = CreateConversionPipeline();
        }

        /// <inheritdoc />
        public override string CreateDescription(object obj)
        {
            return _conversionPipeline.Process(obj);
        }

        /// <inheritdoc />
        public override bool IsSupported(object obj)
        {
            return _conversionPipeline.CanProcess(obj);
        }

        /// <summary>
        ///     Analyzes the implementing class for marked methods and builds the affiliated conversion pipeline
        /// </summary>
        /// <returns></returns>
        protected BreakPipeline<string> CreateConversionPipeline()
        {
            var processors = CreateConversionProcessors();
            var onCannotProcess = GetOnCannotProcessHandler();
            return new BreakPipeline<string>(onCannotProcess, processors);
        }

        /// <summary>
        ///     Finds all marked conversion methods and creates the set of conversion processors
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidAttributeException">If there is more than one conversion method for a single type defined</exception>
        protected List<IObjectProcessor<string>> CreateConversionProcessors()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var creator = new ObjectProcessorCreator();
            return creator.CreateProcessors<string>(this,
                    methodInfo => methodInfo.GetCustomAttribute<DescriptionCreatorMethodAttribute>() != null, flags)
                .ToList();
        }

        /// <summary>
        ///     Get an object processor that is called if the object cannot be converted to a string description
        /// </summary>
        /// <returns></returns>
        protected virtual IObjectProcessor<string> GetOnCannotProcessHandler()
        {
            return ObjectProcessorFactory.Create((object input) => "[Description Error]");
        }
    }
}
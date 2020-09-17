using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using Mocassin.Framework.Operations;
using Mocassin.Framework.Processing;
using Mocassin.Framework.Reflection;
using Mocassin.Model.ModelProject;

namespace Mocassin.Model.Basic
{
    /// <summary>
    ///     Abstract base class for all model data event handler that handle event based updating through processing pipelines
    /// </summary>
    internal abstract class ModelEventHandler<T1, T2, T3>
        where T1 : IModelEventPort
        where T2 : ModelData
        where T3 : ModelEventManager
    {
        /// <summary>
        ///     Flag that indicates if the event handler is connected to a port
        /// </summary>
        public bool IsConnected => EventSubscription != null;

        /// <summary>
        ///     Disposable that contains the unsubscription information for the connected event
        /// </summary>
        protected IDisposable EventSubscription { get; set; }

        /// <summary>
        ///     Pipeline that processes the event arguments and invokes affiliated reactions. Returns resolver report about the
        ///     handling
        /// </summary>
        public BreakPipeline<IConflictReport> HandlerPipeline { get; set; }

        /// <summary>
        ///     Contains the last report generated due to a pipeline invocation
        /// </summary>
        public IConflictReport LastReport { get; set; }

        /// <summary>
        ///     Access to the manager collections project services
        /// </summary>
        public IModelProject ModelProject { get; protected set; }

        /// <summary>
        ///     Accessor provider for the full data access
        /// </summary>
        protected DataAccessorSource<T2> DataAccessorSource { get; set; }

        /// <summary>
        ///     Access to the event manager used for
        /// </summary>
        protected T3 EventManager { get; set; }

        /// <summary>
        ///     Creates new update event handler that uses the provided projects services data accessor provider and event manager
        /// </summary>
        /// <param name="modelProject"></param>
        /// <param name="dataAccessorSource"></param>
        /// <param name="eventManager"></param>
        protected ModelEventHandler(IModelProject modelProject, DataAccessorSource<T2> dataAccessorSource, T3 eventManager)
        {
            ModelProject = modelProject ?? throw new ArgumentNullException(nameof(modelProject));
            DataAccessorSource = dataAccessorSource ?? throw new ArgumentNullException(nameof(dataAccessorSource));
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            HandlerPipeline = CreatePipeline();
        }

        /// <summary>
        ///     Connects to a model event port and subscribes the updating functions the the appropriate events. Returns a
        ///     disposable to terminate the connection
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        [EventPortConnector]
        public IDisposable Connect(T1 eventPort)
        {
            if (IsConnected)
                return null;

            EventSubscription = SubscribeToEvent(eventPort);
            return Disposable.Create(Disconnect);
        }

        /// <summary>
        ///     Disconnects from the event port by disposing and null-out the subscription collection
        /// </summary>
        public void Disconnect()
        {
            EventSubscription.Dispose();
            EventSubscription = null;
        }

        /// <summary>
        ///     Abstract method that defines which event the handler pipeline connects to and what should be done
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        public abstract IDisposable SubscribeToEvent(T1 eventPort);

        /// <summary>
        ///     Defines the basic process event reaction that invokes the pipeline and collects the resolver report
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual void ProcessEvent(EventArgs args)
        {
            LastReport = HandlerPipeline.Process(args);
        }

        /// <summary>
        ///     Builds the handler pipeline by searching the handler implementation for handling methods
        /// </summary>
        /// <returns></returns>
        protected BreakPipeline<IConflictReport> CreatePipeline() =>
            new BreakPipeline<IConflictReport>(GetCannotProcessProcessor(), GetObjectProcessors().ToList());

        /// <summary>
        ///     Get the object processor list by searching the handler for marked methods and creating a set of object processors
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IObjectProcessor<IConflictReport>> GetObjectProcessors()
        {
            bool MethodSearch(MethodInfo info) => info.GetCustomAttribute(typeof(EventHandlingMethodAttribute)) != null;

            return new ObjectProcessorBuilder().CreateProcessors<IConflictReport>(this, MethodSearch,
                BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        ///     Get the on cannot process processor, this usually simply returns a no-resolving required report
        /// </summary>
        /// <returns></returns>
        protected virtual IObjectProcessor<IConflictReport> GetCannotProcessProcessor() =>
            new ObjectProcessor<object, IConflictReport>(ConflictReport.CreateNoResolveRequiredReport);

        /// <summary>
        ///     Default dummy reaction for model events
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        //ModelProject.MessageSystem.SendMessage(new InfoMessage(this, $"{obj} received on {ToString()}"));
        protected virtual IConflictReport DummyHandleEvent(object obj) => new ConflictReport();
    }
}
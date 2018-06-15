using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;

using ICon.Framework.Reflection;
using ICon.Framework.Processing;
using ICon.Framework.Operations;

using ICon.Model.ProjectServices;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Abstract base class for all model data event handler that handle event based updating through processing pipelines
    /// </summary>
    internal abstract class ModelEventHandler<T1, T2, T3> where T1 : IModelEventPort where T2 : ModelData where T3 : ModelEventManager
    {
        /// <summary>
        /// Flag that indicates if the event handler is connected to a port
        /// </summary>
        public bool IsConnected => EventSubscription != null;

        /// <summary>
        /// Disposable that contains the unsubscription information for the connected event
        /// </summary>
        protected IDisposable EventSubscription { get; set; }

        /// <summary>
        /// Pipeline that processes the event arguments and invokes afffiliated reactions. Returns resolver report about the handling
        /// </summary>
        public BreakPipeline<IConflictReport> HandlerPipeline { get; set; }

        /// <summary>
        /// Contains the last report generated due to a pipeline invokation
        /// </summary>
        public IConflictReport LastReport { get; set; }

        /// <summary>
        /// Access to the manager collections project services
        /// </summary>
        public IProjectServices ProjectServices { get; protected set; }

        /// <summary>
        /// Accessor provider for the full data access
        /// </summary>
        protected DataAccessProvider<T2> DataAccessorProvider { get; set; }

        /// <summary>
        /// Access to the event manager used for 
        /// </summary>
        protected T3 EventManager { get; set; }

        /// <summary>
        /// Creates new update event handler that uses the provided projects services data accessor provider and event manager
        /// </summary>
        /// <param name="projectServices"></param>
        /// <param name="dataAccessorProvider"></param>
        /// <param name="eventManager"></param>
        public ModelEventHandler(IProjectServices projectServices, DataAccessProvider<T2> dataAccessorProvider, T3 eventManager)
        {
            ProjectServices = projectServices ?? throw new ArgumentNullException(nameof(projectServices));
            DataAccessorProvider = dataAccessorProvider ?? throw new ArgumentNullException(nameof(dataAccessorProvider));
            EventManager = eventManager ?? throw new ArgumentNullException(nameof(eventManager));
            HandlerPipeline = CreatePipeline();
        }

        /// <summary>
        /// Connects to a model event port and subscribes the updating functions the the appropriate events. Returns a disposable to terminate the connection
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        [EventPortConnector]
        public IDisposable Connect(T1 eventPort)
        {
            if (IsConnected)
            {
                return null;
            }
            EventSubscription = SubscribeToEvent(eventPort);
            return Disposable.Create(() => Disconnect());
        }

        /// <summary>
        /// Disconnects from the event port by disposing and nulling the subscription collection
        /// </summary>
        public void Disconnect()
        {
            EventSubscription.Dispose();
            EventSubscription = null;
        }

        /// <summary>
        /// Abstract method that defines which event the handler piepline connects to and what should be done
        /// </summary>
        /// <param name="eventPort"></param>
        /// <returns></returns>
        public abstract IDisposable SubscribeToEvent(T1 eventPort);

        /// <summary>
        /// Defines the basic process event reaction that invokes the pipleine and collects the resolver report
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected virtual void ProcessEvent(EventArgs args)
        {
            LastReport = HandlerPipeline.Process(args);
        }

        /// <summary>
        /// Builds the handler pipeline by searching the handler implementation for handling methods
        /// </summary>
        /// <returns></returns>
        protected BreakPipeline<IConflictReport> CreatePipeline()
        {
            return new BreakPipeline<IConflictReport>(GetCannotProcessProcessor(), GetObjectProcessors().ToList());
        }

        /// <summary>
        /// Get the object processor list by searching the handler for marked methods and creating a set of object processors
        /// </summary>
        /// <returns></returns>
        protected IEnumerable<IObjectProcessor<IConflictReport>> GetObjectProcessors()
        {
            bool MethodSearch(MethodInfo info)
            {
                return info.GetCustomAttribute(typeof(EventHandlingMethodAttribute)) != null;
            }
            return new ObjectProcessorCreator().CreateProcessors<IConflictReport>(this, MethodSearch, BindingFlags.NonPublic | BindingFlags.Instance);
        }

        /// <summary>
        /// Get the on cannot process processor, this usually simply returns a no-resolving required report
        /// </summary>
        /// <returns></returns>
        protected virtual IObjectProcessor<IConflictReport> GetCannotProcessProcessor()
        {
            return new ObjectProcessor<object, IConflictReport>(value => ConflictReport.CreateNoResolveRequiredReport(value));
        }

        /// <summary>
        /// Placeholder debug dummy reaction that writes the passed inormation to console and returns an empry resolver report
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        protected virtual IConflictReport EventTestReaction(object obj)
        {
            Console.WriteLine($"{obj.ToString()} received on {ToString()}");
            return new ConflictReport();
        }
    }
}

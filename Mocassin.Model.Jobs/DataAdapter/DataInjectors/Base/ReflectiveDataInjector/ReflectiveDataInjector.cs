using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mocassin.Framework.Operations;
using Mocassin.Framework.Reflection;

namespace Mocassin.Model.Mml
{
    /// <summary>
    ///     Abstract base class for reflective data extractors that automatically build their extraction pipeline trough
    ///     reflection on construction
    /// </summary>
    /// <typeparam name="THandler"></typeparam>
    /// <typeparam name="TData"></typeparam>
    public abstract class ReflectiveDataInjector<TData, THandler> : IDataInjector<TData, THandler>
        where TData : new()
    {
        /// <summary>
        ///     The collection of operations that is performed during injection
        /// </summary>
        private readonly ICollection<Action<TData, THandler, IOperationReport>> _injectionOperations;

        /// <summary>
        /// Creates new data injector and automatically build the extraction pipeline from marked methods 
        /// </summary>
        protected ReflectiveDataInjector()
        {
            _injectionOperations = GetInjectionOperations(this);
        }
        
        /// <inheritdoc />
        public IOperationReport InjectData(TData data, THandler handler)
        {
            var report = new OperationReport();
            try
            {
                foreach (var action in _injectionOperations)
                {
                    action(data, handler, report);
                }
            }
            catch (Exception e)
            {
                report.AddException(e);
            }

            return report;
        }

        /// <summary>
        ///     Static method to search an object for marked injection operations and return the collection of found delegates
        ///     bound to the passed instance
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public static ICollection<Action<TData, THandler, IOperationReport>> GetInjectionOperations(object instance)
        {
            int SortComparison(Delegate lhs, Delegate rhs)
            {
                var lhsOrder = lhs.GetMethodInfo().GetCustomAttribute<InjectionMethodAttribute>().Order;
                var rhsOrder = rhs.GetMethodInfo().GetCustomAttribute<InjectionMethodAttribute>().Order;
                return lhsOrder.CompareTo(rhsOrder);
            }

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var creator = new DelegateCreator();
            var delegates = creator.CreateWhere(instance, info => info.GetCustomAttribute<InjectionMethodAttribute>() != null, flags)
                .Cast<Action<TData, THandler, IOperationReport>>()
                .ToList();

            delegates.Sort(SortComparison);
            return delegates;
        }
    }
}
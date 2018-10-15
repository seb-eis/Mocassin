using System;
using System.Reactive.Linq;

namespace ICon.Framework.Extensions
{
    /// <summary>
    /// Defines extension methods for the IObservable interface of the reactive extensions
    /// </summary>
    public static class MocassinObservableExtensions
    {
        /// <summary>
        /// Creates an intercepted observable that spies upon pushed values to subscribers before sending it to the actual subscriber (Does not manipulate pushed data)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="spyNext"></param>
        /// <returns></returns>
        public static IObservable<T1> Spy<T1>(this IObservable<T1> source, Action<T1> spyNext)
        {
            return Observable.Create<T1>(obs => source.Subscribe(x => { spyNext(x); obs.OnNext(x); }, obs.OnError, obs.OnCompleted));
        }

        /// <summary>
        /// Creates an intercepted observable that spies upon pushed values to subscribers before sending it to the actual subscriber (Does not manipulate pushed data)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="spyNext"></param>
        /// <param name="spyError"></param>
        /// <returns></returns>
        public static IObservable<T1> Spy<T1>(this IObservable<T1> source, Action<T1> spyNext, Action<Exception> spyError)
        {
            return Observable.Create<T1>(obs => source.Subscribe(x => { spyNext(x); obs.OnNext(x); }, ex => { spyError(ex); obs.OnError(ex); }, obs.OnCompleted));
        }

        /// <summary>
        /// Creates an intercepted observable that spies upon pushed values to subscribers before sending it to the actual subscriber (Does not manipulate pushed data)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="spyNext"></param>
        /// <param name="spyError"></param>
        /// <param name="spyCompleted"></param>
        /// <returns></returns>
        public static IObservable<T1> Spy<T1>(this IObservable<T1> source, Action<T1> spyNext, Action<Exception> spyError, Action spyCompleted)
        {
            return Observable.Create<T1>(obs => source.Subscribe(x => { spyNext(x); obs.OnNext(x); }, ex => { spyError(ex); obs.OnError(ex); }, () => { spyCompleted(); obs.OnCompleted(); }));
        }

        /// <summary>
        /// Creates an intercepted observable that spies upon pushed values to subscribers before sending it to the actual subscriber (Does not manipulate pushed data)
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="observer"></param>
        /// <returns></returns>
        public static IObservable<T1> Spy<T1>(this IObservable<T1> source, IObserver<T1> observer)
        {
            return Observable.Create<T1>(obs => source.Subscribe(observer));
        }

        /// <summary>
        /// Creates an intercepted observable that manipulates pushed values to subscribers before sending it to the actual subscriber
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="forger"></param>
        /// <returns></returns>
        public static IObservable<T1> Forge<T1>(this IObservable<T1> source, Func<T1, T1> forger)
        {
            return Observable.Create<T1>(obs => source.Subscribe(x => obs.OnNext(forger(x)), obs.OnError, obs.OnCompleted));
        }

        /// <summary>
        /// Creates an intercepted observable that manipulates pushed values to subscribers before sending it to the actual subscriber
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="forger"></param>
        /// <param name="exceptionForger"></param>
        /// <returns></returns>
        public static IObservable<T1> Forge<T1>(this IObservable<T1> source, Func<T1, T1> forger, Func<Exception, Exception> exceptionForger)
        {
            return Observable.Create<T1>(obs => source.Subscribe(x => obs.OnNext(forger(x)), ex => obs.OnError(exceptionForger(ex)), obs.OnCompleted));
        }

        /// <summary>
        /// Creates an intercepted observable that filters pushed values to subscribers and sends only values that are evaluated to true by a filter function
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="passCondition"></param>
        /// <returns></returns>
        public static IObservable<T1> AllowIf<T1>(this IObservable<T1> source, Func<T1, bool> passCondition)
        {
            return Observable.Create<T1>(obs => source.Subscribe(x => { if (passCondition(x)) obs.OnNext(x); }, obs.OnError, obs.OnCompleted));
        }

        /// <summary>
        /// Creates an intercepted observable that filters pushed values to subscribers and blocks all values that 
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <param name="source"></param>
        /// <param name="blockCondition"></param>
        /// <returns></returns>
        public static IObservable<T1> BlockIf<T1>(this IObservable<T1> source, Func<T1, bool> blockCondition)
        {
            return Observable.Create<T1>(obs => source.Subscribe(x => { if (!blockCondition(x)) obs.OnNext(x); }, obs.OnError, obs.OnCompleted));
        }

    }
}

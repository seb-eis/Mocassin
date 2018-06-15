using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Generic cached data class that encapsulates data and recalculation instruction into an 'on-demand' data supplier
    /// </summary>
    public class AutoCachedData<TData> : ICachedData
    {
        /// <summary>
        /// The access lock object to protect from multiple induced data recalculation
        /// </summary>
        private object AccessLock { get; } = new object();

        /// <summary>
        /// Flag that indicates if data is deprecated
        /// </summary>
        public bool IsDeprecated { get; protected set; }

        /// <summary>
        /// The data creation delegate that is triggered each time data is deprecated or null
        /// </summary>
        protected Func<TData> DataCreator { get; set; }

        /// <summary>
        /// The cached data object
        /// </summary>
        protected TData Data { get; set; }

        /// <summary>
        /// Data creation delegate to compare cached data objects for equality
        /// </summary>
        public Delegate DataCreationDelegate => DataCreator;

        /// <summary>
        /// Create new auto cached data object that is by default deprecated
        /// </summary>
        protected AutoCachedData()
        {
            lock (AccessLock)
            {
                IsDeprecated = true;
            }
        }

        /// <summary>
        /// Creates new auto cached data object that uses the provided creation function
        /// </summary>
        /// <param name="dataCreator"></param>
        public AutoCachedData(Func<TData> dataCreator) : this()
        {
            DataCreator = dataCreator ?? throw new ArgumentNullException(nameof(dataCreator));
        }

        /// <summary>
        /// Get the data object, triggers update if data is deprecated/null
        /// </summary>
        /// <returns></returns>
        public TData GetData()
        {
            lock (AccessLock)
            {
                if (IsDeprecated)
                {
                    Recalculate();
                }
                return Data;
            }
        }

        /// <summary>
        /// Start data object retrieval as a new task, triggers data update if deprecated flag is set
        /// </summary>
        /// <returns></returns>
        public Task<TData> GetDataAsync()
        {
            return Task.Run(() => GetData());
        }

        /// <summary>
        /// Overwrites internal data by default value and sets the cache status to deprecated
        /// </summary>
        public void Clear()
        {
            lock (AccessLock)
            {
                Data = default(TData);
                IsDeprecated = true;
            }
        }

        /// <summary>
        /// Clears the data only if it is deprecated, else does nothing
        /// </summary>
        public void ClearIfDeprecated()
        {
            lock (AccessLock)
            {
                if (IsDeprecated)
                {
                    Clear();
                }
            }
        }

        /// <summary>
        /// Get the data as an object
        /// </summary>
        /// <returns></returns>
        public object GetValue()
        {
            return GetData();
        }

        /// <summary>
        /// Get the data as object through a new task
        /// </summary>
        /// <returns></returns>
        public Task<object> GetValueAsync()
        {
            return Task.Run(() => GetValue());
        }

        /// <summary>
        /// Compares the data creation delegates for equality
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(ICachedData other)
        {
            return DataCreationDelegate.Equals(other.DataCreationDelegate);
        }

        /// <summary>
        /// Marks the data as deprecated, this will cause recalculation on the next retrieval
        /// </summary>
        public void MarkAsDeprecated()
        {
            lock (AccessLock)
            {
                IsDeprecated = true;
            }
        }

        /// <summary>
        /// Triggers update function and sets the deprecated flag to false
        /// </summary>
        protected void Recalculate()
        {
            Data = (TData)DataCreator();
            IsDeprecated = false;
        }
    }
}

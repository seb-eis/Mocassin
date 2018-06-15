﻿using System;
using System.Collections.Generic;
using System.Text;

using ICon.Model.Basic;

namespace ICon.Model.Transitions
{
    /// <summary>
    /// Basic implementation of the transition query manager that handles safe data queries and service requests to the transition manager from outside sources
    /// </summary>
    internal class TransitionQueryManager : ModelQueryManager<TransitionModelData, ITransitionDataPort, TransitionDataCache, ITransitionCachePort>, ITransitionQueryPort
    {
        /// <summary>
        /// Creates new transition query manager from passed base data, cached data and data access locker
        /// </summary>
        /// <param name="baseData"></param>
        /// <param name="extendedData"></param>
        /// <param name="dataLocker"></param>
        public TransitionQueryManager(TransitionModelData baseData, TransitionDataCache extendedData, DataAccessLocker dataLocker)
            : base(baseData, extendedData, dataLocker)
        {

        }
    }
}

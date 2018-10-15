﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Model.Basic
{
    /// <summary>
    /// Base interface for all model objects or structs that support deprecation operations and are always indexed by a manager
    /// </summary>
    public interface IModelObject
    {
        /// <summary>
        /// Flag that indicates if the object is deprecated
        /// </summary>
        bool IsDeprecated { get; }

        /// <summary>
        /// Get the index of the model object
        /// </summary>
        int Index { get; }

        /// <summary>
        /// Deprecates object
        /// </summary>
        void Deprecate();

        /// <summary>
        /// Returns a string that represents the model object type name
        /// </summary>
        /// <returns></returns>
        string GetObjectName();
    }
}

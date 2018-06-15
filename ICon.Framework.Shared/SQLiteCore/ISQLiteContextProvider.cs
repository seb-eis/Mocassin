using System;
using System.Collections.Generic;
using System.Text;

namespace ICon.Framework.SQLiteCore
{
    /// <summary>
    /// The context provider interafce that ensures that the provider has a factory metod for creating new context
    /// </summary>
    public interface ISQLiteContextProvider<T1> where T1 : SQLiteContext<T1>, new()
    {
        /// <summary>
        /// Factory method that creates new context with the stored context parameter information
        /// </summary>
        /// <returns></returns>
        T1 CreateContext();
    }
}

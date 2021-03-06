﻿using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    ///     Information message class that carries information massages or warnings from the framework or model libraries (non
    ///     critical messages)
    /// </summary>
    public class InfoMessage : PushMessage
    {
        /// <summary>
        ///     Contains additional information and explanations
        /// </summary>
        public IList<string> Details { get; set; }

        /// <inheritdoc />
        public override IEnumerable<string> DetailSequence => Details?.AsEnumerable();

        /// <inheritdoc />
        public InfoMessage(object sender, string shortInfo)
            : base(sender, shortInfo)
        {
        }
    }
}
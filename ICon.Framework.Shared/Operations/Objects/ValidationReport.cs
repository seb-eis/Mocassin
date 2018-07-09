using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reactive.Linq;
using Newtonsoft.Json;

using ICon.Framework.Extensions;
using ICon.Framework.Messaging;

namespace ICon.Framework.Operations
{
    /// <summary>
    /// Validation result that carries information and messages produced during a validation
    /// </summary>
    public class ValidationReport : IValidationReport
    {
        /// <summary>
        /// Flag if the validation was successful (No critical warnings occured)
        /// </summary>
        public bool IsGood { get; protected set; }

        /// <summary>
        /// List of warning messages produces during the validation
        /// </summary>
        public List<WarningMessage> Warnings { get; protected set; }

        /// <summary>
        /// Creates a new validation result that is by default a success
        /// </summary>
        public ValidationReport()
        {
            Warnings = new List<WarningMessage>();
            IsGood = true;
        }

        /// <summary>
        /// Adds a warning message and sets validation to failed if the warning is critical
        /// </summary>
        /// <param name="message"></param>
        public void AddWarning(WarningMessage message)
        {
            Warnings.Add(message);
            if (message.IsCritical)
            {
                IsGood = false;
            }
        }

        /// <summary>
        /// Addss multiple warnings messages and sets validation to failed if one is critical
        /// </summary>
        /// <param name="messages"></param>
        public void AddWarnings(IEnumerable<WarningMessage> messages)
        {
            foreach (var warning in messages)
            {
                AddWarning(warning);
            }
        }

        /// <summary>
        /// Get warnings enumerator
        /// </summary>
        /// <returns></returns>
        public IEnumerator<WarningMessage> GetWarningsEnumerator()
        {
            return Warnings.GetEnumerator();
        }

        /// <summary>
        /// Returns json string of the object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented);
        }
    }
}

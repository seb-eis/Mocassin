using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace ICon.Framework.Messaging
{
    /// <summary>
    /// ICon validation message class that is used for all user input validation messages (informing about good or bad user input)
    /// </summary>
    public class WarningMessage : PushMessage
    {
        /// <summary>
        /// Flag that indicates if the warning is critical
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        /// Contains additional information and explanations why the warning occured
        /// </summary>
        public List<string> Details { get; set; }

        /// <summary>
        /// Creates new warning message with the minimal amount of information
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shortInfo"></param>
        public WarningMessage(object sender, string shortInfo) : base(sender, shortInfo)
        {
            Details = new List<string>();
        }

        /// <summary>
        /// Add a sequence of strings as details to the warning
        /// </summary>
        /// <param name="details"></param>
        public void AddDetails(IEnumerable<string> details)
        {
            foreach (var item in details)
            {
                Details.Add(item);
            }
        }

        /// <summary>
        /// Add an arbitrary number of strings as details to the warning
        /// </summary>
        /// <param name="details"></param>
        public void AddDetails(params string[] details)
        {
            AddDetails(details.AsEnumerable());
        }

        /// <summary>
        /// Creates a new warning message that is directly marked with the critical flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shortInfo"></param>
        /// <returns></returns>
        public static WarningMessage CreateCritical(object sender, string shortInfo)
        {
            return new WarningMessage(sender, shortInfo) { IsCritical = true };
        }
    }
}

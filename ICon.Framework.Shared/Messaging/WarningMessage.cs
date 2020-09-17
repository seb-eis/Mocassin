using System.Collections.Generic;
using System.Linq;

namespace Mocassin.Framework.Messaging
{
    /// <summary>
    ///     ICon validation message class that is used for all user input validation messages (informing about good or bad user
    ///     input)
    /// </summary>
    public class WarningMessage : PushMessage
    {
        /// <summary>
        ///     Flag that indicates if the warning is critical
        /// </summary>
        public bool IsCritical { get; set; }

        /// <summary>
        ///     Contains additional information and explanations why the warning occured
        /// </summary>
        public IList<string> Details { get; set; }

        /// <inheritdoc />
        public override IEnumerable<string> DetailSequence => Details?.AsEnumerable();

        /// <inheritdoc />
        public WarningMessage(object sender, string shortInfo)
            : base(sender, shortInfo)
        {
            Details = new List<string>();
        }

        /// <summary>
        ///     Add a sequence of strings as details to the warning
        /// </summary>
        /// <param name="details"></param>
        public void AddDetails(IEnumerable<string> details)
        {
            foreach (var item in details) Details.Add(item);
        }

        /// <summary>
        ///     Add an arbitrary number of strings as details to the warning
        /// </summary>
        /// <param name="details"></param>
        public void AddDetails(params string[] details)
        {
            AddDetails(details.AsEnumerable());
        }

        /// <summary>
        ///     Creates a new warning message that is directly marked with the critical flag
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="shortInfo"></param>
        /// <returns></returns>
        public static WarningMessage CreateCritical(object sender, string shortInfo) => new WarningMessage(sender, shortInfo) {IsCritical = true};
    }
}
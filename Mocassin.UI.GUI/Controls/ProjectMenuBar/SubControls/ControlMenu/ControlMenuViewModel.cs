using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.GUI.Controls.Base.Attributes;
using Mocassin.UI.GUI.Controls.Base.Commands;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.GUI.Helper;

namespace Mocassin.UI.GUI.Controls.ProjectMenuBar.SubControls.ControlMenu
{
    /// <summary>
    ///     The <see cref="PrimaryControlViewModel" /> for the <see cref="ControlMenuView" /> that provides tab creation
    ///     control
    /// </summary>
    public class ControlMenuViewModel : PrimaryControlViewModel
    {
        /// <summary>
        ///     Get or set the <see cref="IReadOnlyDictionary{TKey,TValue}" /> of provided names and affiliated commands
        /// </summary>
        public IReadOnlyDictionary<string, ICommand> NameCommandDictionary { get; }

        /// <inheritdoc />
        public ControlMenuViewModel(IMocassinProjectControl projectControl)
            : base(projectControl)
        {
            NameCommandDictionary = CreateNameCommandDictionary();
        }

        /// <summary>
        ///     Searches the <see cref="Assembly" /> for marked commands and creates the
        ///     <see cref="IReadOnlyDictionary{TKey,TValue}" /> of commands and names
        /// </summary>
        /// <returns></returns>
        private IReadOnlyDictionary<string, ICommand> CreateNameCommandDictionary()
        {
            var dictionary = Assembly.GetAssembly(typeof(Command))
                .MakeAttributedInstances<ProjectControlCommand, ControlMenuCommandAttribute>(ProjectControl)
                .ToDictionary(x => x.Value.DisplayName, y => (ICommand) y.Key);

            return dictionary;
        }
    }
}
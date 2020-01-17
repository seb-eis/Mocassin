using System;
using System.Windows.Controls;
using System.Windows.Input;
using Mocassin.UI.Base.Commands;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Extends the <see cref="ControlTabItem" /> to provide functionality and commands to perform removal form or
    ///     switching between <see cref="IControlTabHost" /> containers
    /// </summary>
    public class DynamicControlTabItem : ControlTabItem
    {
        /// <summary>
        ///     Get the hosting <see cref="IControlTabHost" />
        /// </summary>
        private IControlTabHost TabHost { get; set; }

        /// <summary>
        ///     Get a command that removes the item from its <see cref="IControlTabHost" /> and disposes its contents
        /// </summary>
        public ICommand CloseTabCommand { get; }

        /// <summary>
        ///     Creates new <see cref="DynamicControlTabItem" /> that belongs to the provided
        ///     <see cref="IControlTabHost" />
        /// </summary>
        /// <param name="tabName"></param>
        /// <param name="contentViewModel"></param>
        /// <param name="content"></param>
        /// <param name=""></param>
        /// <param name="tabHost"></param>
        public DynamicControlTabItem(string tabName, ViewModelBase contentViewModel, Control content, IControlTabHost tabHost)
            : base(tabName, contentViewModel, content)
        {
            TabHost = tabHost ?? throw new ArgumentNullException(nameof(tabHost));
            CloseTabCommand = CreateCloseAndDisposeCommand();
        }

        /// <summary>
        ///     Creates an <see cref="ICommand" /> that removes the tab from its current <see cref="IControlTabHost" /> and
        ///     disposes the content
        /// </summary>
        /// <returns></returns>
        private ICommand CreateCloseAndDisposeCommand()
        {
            void CloseTab() => TabHost.RemoveAndDispose(this);
            bool CanCloseTab() => TabHost.Contains(this);
            return new RelayCommand(CloseTab, CanCloseTab);
        }

        /// <summary>
        ///     Detaches the <see cref="DynamicControlTabItem" /> from its current <see cref="IControlTabHost" /> and reattaches it to a new one
        /// </summary>
        /// <param name="newTabHost"></param>
        private void SwitchHost(IControlTabHost newTabHost)
        {
            if (Equals(TabHost, newTabHost)) return;
            TabHost.RemoveItem(this);
            newTabHost.AddTab(this);
            TabHost = newTabHost;
        }
    }
}
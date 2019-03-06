using System;
using Mocassin.UI.GUI.Base.ViewModels.Tabs;
using Mocassin.UI.GUI.Controls.ProjectTabControl.SubControls.XmlControl;

namespace Mocassin.UI.GUI.Controls.ProjectTabControl
{
    public class ProjectTabControlViewModel : UserControlTabControlViewModel
    {
        /// <summary>
        ///     Get the <see cref="XmlInstructionControlViewModel" /> that enables project control by xml files
        /// </summary>
        private XmlInstructionControlViewModel XmlInstructionControlViewModel { get; }

        /// <summary>
        ///     Create new <see cref="ProjectTabControlViewModel" /> with the provided
        ///     <see cref="XmlInstructionControlViewModel" />
        /// </summary>
        /// <param name="xmlInstructionControlViewModel"></param>
        public ProjectTabControlViewModel(XmlInstructionControlViewModel xmlInstructionControlViewModel)
        {
            XmlInstructionControlViewModel =
                xmlInstructionControlViewModel ?? throw new ArgumentNullException(nameof(xmlInstructionControlViewModel));
            InitializeDefaultTabs();
        }

        /// <inheritdoc />
        protected sealed override void InitializeDefaultTabs()
        {
            base.InitializeDefaultTabs();
            AddNonClosableTab("Xml Interface", XmlInstructionControlViewModel, new XmlInstructionControlView());
        }
    }
}
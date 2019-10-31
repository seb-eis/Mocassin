using System;
using System.ComponentModel;
using Mocassin.Framework.Events;
using Mocassin.Model.Basic;
using Mocassin.UI.Xml.Main;

namespace Mocassin.UI.Xml.Helper
{
    /// <summary>
    ///     Change observer that can be attached to a <see cref="MocassinProjectGraph"/> relay property changes
    /// </summary>
    public class ProjectTreeChangeObserver : IDisposable
    {
        private DisposableCollection ProjectSubscriptions { get; }

        private MocassinProjectGraph ObservedProjectGraph { get; set; }

        public void AttachToProject(MocassinProjectGraph projectGraph)
        {
            if (ReferenceEquals(ObservedProjectGraph, projectGraph)) return;
            if (ObservedProjectGraph != null) throw new InvalidOperationException("Change subscriber already attached to another project.");
        }

        public void DetachFromProject()
        {

        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetachFromProject();
        }
    }
}
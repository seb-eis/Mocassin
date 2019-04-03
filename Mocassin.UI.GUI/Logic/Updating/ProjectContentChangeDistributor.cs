using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Mocassin.UI.GUI.Base.DataContext;
using Mocassin.UI.Xml.ProjectLibrary;

namespace Mocassin.UI.GUI.Logic.Updating
{
    public class ProjectContentChangeDistributor
    {
        private IMocassinProjectControl projectControl;

        public ProjectContentChangeDistributor(IMocassinProjectControl projectControl)
        {
            this.projectControl = projectControl ?? throw new ArgumentNullException(nameof(projectControl));
            Task.Run(Run);
        }

        public void Run()
        {
            while (true)
            {
                (projectControl.OpenProjectLibrary as MocassinProjectContext)?.HasUnsavedChanges();
                Thread.Sleep(2000);
            }
        }
    }
}
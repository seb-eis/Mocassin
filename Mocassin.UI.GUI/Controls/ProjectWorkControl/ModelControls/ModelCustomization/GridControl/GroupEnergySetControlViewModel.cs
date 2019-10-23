using System;
using System.Linq;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Xml.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}"/> for <see cref="GroupEnergySetControlView"/> that controls the <see cref="GroupEnergyGraph"/> customization
    /// </summary>
    public sealed class GroupEnergySetControlViewModel : CollectionControlViewModel<GroupEnergyGraph>
    {
        /// <summary>
        ///     Get the <see cref="GroupInteractionGraph"/> that the view model targets
        /// </summary>
        public GroupInteractionGraph GroupInteraction { get; }

        /// <summary>
        ///     Get the <see cref="string"/> description of the base geometry
        /// </summary>
        public string BaseGeometryDescription { get; }

        /// <summary>
        ///     Create new <see cref="GroupEnergySetControlViewModel"/> for the passed <see cref="GroupInteractionGraph"/>
        /// </summary>
        /// <param name="groupInteraction"></param>
        public GroupEnergySetControlViewModel(GroupInteractionGraph groupInteraction)
        {
            GroupInteraction = groupInteraction ?? throw new ArgumentNullException(nameof(groupInteraction));
            SetCollection(GroupInteraction.EnergyEntries);
            SelectedItem = Items?.FirstOrDefault();
            BaseGeometryDescription = GetBaseGeometryDescription(groupInteraction);
        }

        /// <summary>
        ///     Builds a <see cref="string"/> description for the base geometry of the passed <see cref="GroupInteractionGraph"/>
        /// </summary>
        /// <param name="groupInteraction"></param>
        /// <returns></returns>
        public string GetBaseGeometryDescription(GroupInteractionGraph groupInteraction)
        {
            var builder = new StringBuilder(250);
            foreach (var vector in groupInteraction.BaseGeometry)
            {
                builder.Append(vector);
                builder.Append(" ");
            }
            builder.PopBack(1);
            return builder.ToString();
        }
    }
}
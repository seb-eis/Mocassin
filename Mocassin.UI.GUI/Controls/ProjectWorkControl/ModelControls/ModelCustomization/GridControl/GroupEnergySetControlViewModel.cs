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
        ///     Get the <see cref="GroupEnergySetGraph"/> that the view model targets
        /// </summary>
        public GroupEnergySetGraph GroupEnergySet { get; }

        /// <summary>
        ///     Get the <see cref="string"/> description of the base geometry
        /// </summary>
        public string BaseGeometryDescription { get; }

        /// <summary>
        ///     Create new <see cref="GroupEnergySetControlViewModel"/> for the passed <see cref="GroupEnergySetGraph"/>
        /// </summary>
        /// <param name="groupEnergySet"></param>
        public GroupEnergySetControlViewModel(GroupEnergySetGraph groupEnergySet)
        {
            GroupEnergySet = groupEnergySet ?? throw new ArgumentNullException(nameof(groupEnergySet));
            SetCollection(GroupEnergySet.EnergyEntries);
            SelectedItem = Items?.FirstOrDefault();
            BaseGeometryDescription = GetBaseGeometryDescription(groupEnergySet);
        }

        /// <summary>
        ///     Builds a <see cref="string"/> description for the base geometry of the passed <see cref="GroupEnergySetGraph"/>
        /// </summary>
        /// <param name="groupEnergySet"></param>
        /// <returns></returns>
        public string GetBaseGeometryDescription(GroupEnergySetGraph groupEnergySet)
        {
            var builder = new StringBuilder(250);
            foreach (var vector in groupEnergySet.BaseGeometry)
            {
                builder.Append(vector);
                builder.Append(" ");
            }
            builder.PopBack(1);
            return builder.ToString();
        }
    }
}
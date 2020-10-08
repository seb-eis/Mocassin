using System;
using System.Linq;
using System.Text;
using Mocassin.Framework.Extensions;
using Mocassin.UI.GUI.Controls.Base.ViewModels;
using Mocassin.UI.Data.Customization;

namespace Mocassin.UI.GUI.Controls.ProjectWorkControl.ModelControls.ModelCustomization.GridControl
{
    /// <summary>
    ///     The <see cref="CollectionControlViewModel{T}" /> for <see cref="GroupEnergySetControlView" /> that controls the
    ///     <see cref="GroupEnergyData" /> customization
    /// </summary>
    public sealed class GroupEnergySetControlViewModel : CollectionControlViewModel<GroupEnergyData>
    {
        /// <summary>
        ///     Get the <see cref="GroupEnergySetData" /> that the view model targets
        /// </summary>
        public GroupEnergySetData GroupEnergySet { get; }

        /// <summary>
        ///     Get the <see cref="string" /> description of the base geometry
        /// </summary>
        public string BaseGeometryDescription { get; }

        /// <summary>
        ///     Create new <see cref="GroupEnergySetControlViewModel" /> for the passed <see cref="GroupEnergySetData" />
        /// </summary>
        /// <param name="groupEnergySet"></param>
        public GroupEnergySetControlViewModel(GroupEnergySetData groupEnergySet)
        {
            GroupEnergySet = groupEnergySet ?? throw new ArgumentNullException(nameof(groupEnergySet));
            SetCollection(GroupEnergySet.EnergyEntries);
            SelectedItem = Items?.FirstOrDefault();
            BaseGeometryDescription = GetBaseGeometryDescription(groupEnergySet);
        }

        /// <summary>
        ///     Builds a <see cref="string" /> description for the base geometry of the passed <see cref="GroupEnergySetData" />
        /// </summary>
        /// <param name="groupEnergySet"></param>
        /// <returns></returns>
        public string GetBaseGeometryDescription(GroupEnergySetData groupEnergySet)
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
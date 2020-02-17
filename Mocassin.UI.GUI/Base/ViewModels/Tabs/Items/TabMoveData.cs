using System;

namespace Mocassin.UI.GUI.Base.ViewModels.Tabs
{
    /// <summary>
    ///     Provides the data required for moving <see cref="DynamicControlTabItem" /> instances between or within tab hosters
    /// </summary>
    public class TabMoveData
    {
        /// <summary>
        ///     The index at which the item should be inserted
        /// </summary>
        public int InsertIndex { get; }

        /// <summary>
        ///     The <see cref="DynamicControlTabItem" /> that is moved
        /// </summary>
        public DynamicControlTabItem ControlTabItem { get; }

        /// <summary>
        ///     Creates a new <see cref="TabMoveData" /> with a <see cref="DynamicControlTabItem" /> and insert index
        /// </summary>
        /// <param name="controlTabItem"></param>
        /// <param name="insertIndex"></param>
        public TabMoveData(DynamicControlTabItem controlTabItem, int insertIndex = -1)
        {
            InsertIndex = insertIndex;
            ControlTabItem = controlTabItem ?? throw new ArgumentNullException(nameof(controlTabItem));
        }
    }
}
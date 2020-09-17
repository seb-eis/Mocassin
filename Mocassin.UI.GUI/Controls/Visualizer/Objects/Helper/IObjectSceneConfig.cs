using System;
using System.ComponentModel;
using System.Windows.Media;

namespace Mocassin.UI.GUI.Controls.Visualizer.Objects
{
    /// <summary>
    ///     Provides a 3D scene configuration for a rendered object (Scaling, mesh quality, material, ...) with property change
    ///     notifications
    /// </summary>
    public interface IObjectSceneConfig : IEquatable<IObjectSceneConfig>, INotifyPropertyChanged
    {
        /// <summary>
        ///     Get the <see cref="VisualObjectCategory" /> the object belongs to
        /// </summary>
        VisualObjectCategory VisualCategory { get; }

        /// <summary>
        ///     Get the name of the base material for the object
        /// </summary>
        string MaterialName { get; set; }

        /// <summary>
        ///     Get or set the scaling factor for the object
        /// </summary>
        double Scaling { get; set; }

        /// <summary>
        ///     Get or set a quality factor for the object
        /// </summary>
        double Quality { get; set; }

        /// <summary>
        ///     Get or set the base color for the object
        /// </summary>
        Color Color { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the object is visible
        /// </summary>
        bool IsVisible { get; set; }

        /// <summary>
        ///     Checks if the config is applicable to the provided object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        bool IsApplicable(object obj);
    }
}
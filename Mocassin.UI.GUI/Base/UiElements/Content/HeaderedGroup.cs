using System.Windows;
using System.Windows.Controls;

namespace Mocassin.UI.GUI.Base.UiElements.Content
{
    /// <summary>
    ///     Provides an <see cref="HeaderedContentControl" /> implementation that has a slightly different behavior than <see cref="GroupBox"/>
    /// </summary>
    public class HeaderedGroup : HeaderedContentControl
    {
        /// <summary>
        ///     Dependency property for <see cref="HeaderToolTip" />
        /// </summary>
        public static readonly DependencyProperty HeaderToolTipProperty = DependencyProperty.Register(nameof(HeaderToolTip), typeof(object),
            typeof(HeaderedGroup), new PropertyMetadata((object) null));

        /// <summary>
        ///     Dependency property for <see cref="HeaderMargin" />
        /// </summary>
        public static readonly DependencyProperty HeaderMarginProperty = DependencyProperty.Register(nameof(HeaderMargin), typeof(Thickness),
            typeof(HeaderedGroup), new PropertyMetadata(new Thickness()));

        /// <summary>
        ///     Dependency property for <see cref="ContentMargin" />
        /// </summary>
        public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register(nameof(ContentMargin), typeof(Thickness),
            typeof(HeaderedGroup), new PropertyMetadata(new Thickness()));

        /// <summary>
        ///     Defines the tool tip object displayed for the header. This is a dependency property
        /// </summary>
        public object HeaderToolTip
        {
            get => GetValue(HeaderToolTipProperty);
            set => SetValue(HeaderToolTipProperty, value);
        }

        /// <summary>
        ///     Defines the margin of the displayed header. This is a dependency property
        /// </summary>
        public Thickness HeaderMargin
        {
            get => (Thickness) GetValue(HeaderMarginProperty);
            set => SetValue(HeaderMarginProperty, value);
        }

        /// <summary>
        ///     Defines the margin of the displayed content. This is a dependency property
        /// </summary>
        public Thickness ContentMargin
        {
            get =>  (Thickness) GetValue(ContentMarginProperty);
            set => SetValue(ContentMarginProperty, value);
        }

        /// <inheritdoc />
        static HeaderedGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(HeaderedGroup), new FrameworkPropertyMetadata(typeof(HeaderedGroup)));
        }
    }
}
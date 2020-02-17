using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
// ReSharper disable InconsistentNaming

namespace Mocassin.UI.GUI.Base.UiElements.Popup
{
    /// <summary>
    ///     Extends the <see cref="System.Windows.Controls.Primitives.Popup" /> class to enable non-topmost placement and make
    ///     it usable as an overlay control
    /// </summary>
    public class ControlOverlay : System.Windows.Controls.Primitives.Popup
    {
        /// <summary>
        ///     The <see cref="Topmost" /> <see cref="DependencyProperty" />
        /// </summary>
        public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(ControlOverlay), new FrameworkPropertyMetadata(false, OnIsTopmostChanged));

        /// <summary>
        ///     Get or set the parent <see cref="Window" />
        /// </summary>
        private Window ParentWindow { get; set; }

        /// <summary>
        ///     Get or set the parent <see cref="UserControl"/>
        /// </summary>
        private UserControl ParentControl { get; set; }

        /// <summary>
        ///     Get or set a boolean flag if the <see cref="ControlOverlay" /> was previously loaded
        /// </summary>
        private bool AlreadyLoaded { get; set; }

        /// <summary>
        ///     Get set a boolean flag if the popup should be topmost in all situations
        /// </summary>
        public bool Topmost
        {
            get => (bool) GetValue(TopmostProperty);
            set => SetValue(TopmostProperty, value);
        }

        /// <inheritdoc />
        public ControlOverlay()
        {
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }


        /// <summary>
        ///     Callback for the <see cref="Topmost" /> property change
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="args"></param>
        private static void OnIsTopmostChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            (obj as ControlOverlay)?.UpdateOverlayPosition();
        }

        /// <summary>
        ///     Event handler for the unloaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            try
            {
                if (ParentWindow == null) return;
                ParentWindow.Activated -= OnParentWindowActivated;
                ParentWindow.Deactivated -= OnParentWindowDeactivated;
                ParentWindow.LocationChanged -= OnForceResetRequired;
                ParentWindow = null;
                if (ParentControl == null) return;
                ParentControl.SizeChanged -= OnForceResetRequired;
                ParentControl = null;
            }
            finally
            {
                AlreadyLoaded = false;   
            }
        }

        /// <summary>
        ///     Event handler for the loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (AlreadyLoaded) return;
            try
            {
                ForceIsOpenBindingTargetUpdate();
                ParentWindow = Window.GetWindow(this);
                ParentControl = FindParent<UserControl>(this);
                if (ParentWindow != null)
                {
                    ParentWindow.Activated += OnParentWindowActivated;
                    ParentWindow.Deactivated += OnParentWindowDeactivated;
                    ParentWindow.LocationChanged += OnForceResetRequired;
                }

                if (ParentControl != null)
                {
                    ParentControl.SizeChanged += OnForceResetRequired;
                }
            }
            finally
            {
                AlreadyLoaded = true;
            }
        }

        /// <summary>
        ///     Forces the IsOpenProperty to update the binding target if a binding exists
        /// </summary>
        private void ForceIsOpenBindingTargetUpdate()
        {
            BindingOperations.GetBindingExpression(this, IsOpenProperty)?.UpdateTarget();
        }

        /// <summary>
        ///     Event handler for the activation of the parent window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void OnParentWindowActivated(object sender, EventArgs arg)
        {

        }

        /// <summary>
        ///     Event handler for the deactivation of the parent window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="arg"></param>
        private void OnParentWindowDeactivated(object sender, EventArgs arg)
        {

        }

        /// <summary>
        ///     Event handler for changes of the parent <see cref="FrameworkElement"/> or <see cref="Window"/> that require a forced update of the overlay
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        protected virtual void OnForceResetRequired(object sender, EventArgs args)
        {
            if (!IsOpen) return;
            var binding = BindingOperations.GetBinding(this, IsOpenProperty);
            if (binding != null)
            {
                BindingOperations.ClearBinding(this, IsOpenProperty);
                BindingOperations.SetBinding(this, IsOpenProperty, binding);
                return;
            }

            IsOpen = false;
            IsOpen = true;
        }

        /// <inheritdoc />
        protected override void OnOpened(EventArgs args)
        {
            UpdateOverlayPosition();
            base.OnOpened(args);
        }

        /// <summary>
        ///     Finds the parent control that hosts the provided <see cref="DependencyObject"/>
        /// </summary>
        /// <returns></returns>
        protected T FindParent<T>(DependencyObject child = null) where  T : class
        {
            if (child == null) throw new ArgumentNullException(nameof(child));
            while (child != null)
            {
                var parent = VisualTreeHelper.GetParent(child);
                if (parent is T parent1) return parent1;
                child = parent;
            }

            return null;
        }

        /// <summary>
        ///     Updates the overlay position based on the <see cref="Topmost" /> setting
        /// </summary>
        private void UpdateOverlayPosition()
        {
            if (!(PresentationSource.FromVisual(Child) is HwndSource hwndSource)) return;
            var callOk = GetWindowRect(hwndSource.Handle, out var rect);
            Debug.WriteLineIf(!callOk, $"Cannot set window position: {nameof(GetWindowRect)} returned false.");
            if (!callOk) return;

            callOk = SetWindowPos(hwndSource.Handle, Topmost ? HWND_TOPMOST : HWND_NOTOPMOST, rect.left, rect.top, (int) Width, (int) Height, 0);
            Debug.WriteLineIf(!callOk, $"Cannot set window position: {nameof(SetWindowPos)} returned false.");
        }

        #region dllimport

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        [StructLayout(LayoutKind.Sequential)]
        [DebuggerDisplay("L:{left},T:{top},R:{right},B:{bottom}")]
        internal struct RECT
        {
            internal int left;
            internal int top;
            internal int right;
            internal int bottom;
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hwndInsertAfter, int x, int y, int cx, int cy, uint flags);

        #endregion
    }
}
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;

namespace Mocassin.UI.GUI.Base.UiElements.Popup
{
    /// <summary>
    ///     Extends the <see cref="System.Windows.Controls.Primitives.Popup" /> class to enable non-topmost placement and make
    ///     it usable as an overlay control
    /// </summary>
    public class ControlOverlay : System.Windows.Controls.Primitives.Popup
    {
        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private static readonly IntPtr HWND_NOTOPMOST = new IntPtr(-2);
        private static readonly IntPtr HWND_TOP = new IntPtr(0);
        private static readonly IntPtr HWND_BOTTOM = new IntPtr(1);

        /// <summary>
        ///     The <see cref="Topmost" /> <see cref="DependencyProperty" />
        /// </summary>
        public static DependencyProperty TopmostProperty =
            Window.TopmostProperty.AddOwner(typeof(ControlOverlay), new FrameworkPropertyMetadata(false, OnIsTopmostChanged));

        /// <summary>
        ///     Get or set the parent <see cref="Window" />
        /// </summary>
        private Window ParentWindow { get; set; }

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
            if (ParentWindow == null) return;
            ParentWindow.Activated -= OnParentWindowActivated;
            ParentWindow.Deactivated -= OnParentWindowDeactivated;
            AlreadyLoaded = false;
        }

        /// <summary>
        ///     Event handler for the loaded event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            if (!AlreadyLoaded)
            {
                AlreadyLoaded = true;
                ParentWindow = Window.GetWindow(this);
                if (ParentWindow == null) return;
                ParentWindow.Activated += OnParentWindowActivated;
                ParentWindow.Deactivated += OnParentWindowDeactivated;
            }
            ForceIsOpenPropertyBindingUpdate();
        }

        /// <summary>
        ///     Forces the IsOpenProperty to update the binding target if a binding exists
        /// </summary>
        private void ForceIsOpenPropertyBindingUpdate()
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

        /// <inheritdoc />
        protected override void OnOpened(EventArgs args)
        {
            UpdateOverlayPosition();
            base.OnOpened(args);
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
            
            callOk = SetWindowPos(hwndSource.Handle, Topmost ? HWND_TOPMOST : HWND_NOTOPMOST, rect.Left, rect.Top, (int) Width, (int) Height, 0);
            Debug.WriteLineIf(!callOk, $"Cannot set window position: {nameof(SetWindowPos)} returned false.");
        }

        #region dllimport

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
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
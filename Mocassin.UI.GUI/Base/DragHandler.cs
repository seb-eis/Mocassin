using System;
using System.Windows;
using System.Windows.Input;

namespace Mocassin.UI.GUI.Base
{
    /// <summary>
    ///     System to safely detect drag events without interfering with other mouse events
    /// </summary>
    public class DragHandler<TElement> where TElement : FrameworkElement
    {
        /// <summary>
        ///     Get or set the minimal drag length before a mouse move is converted to a drag
        /// </summary>
        public double MinLengthBeforeDrag { get; set; } = 5;

        /// <summary>
        ///     Get or seth the last <see cref="Point" /> of a potential drag drop event
        /// </summary>
        public Point? StartPoint { get; private set; }

        /// <summary>
        ///     Get or set the <see cref="Func{TResult}" /> to package data from the <see cref="TElement" /> into a
        ///     <see cref="IDataObject" />
        /// </summary>
        public Func<TElement, IDataObject> PackerFunction { get; set; }

        /// <summary>
        ///     Get or set the allowed <see cref="DragDropEffects" />, defaults to copy behavior
        /// </summary>
        public DragDropEffects AllowedEffects { get; set; } = DragDropEffects.Copy;

        /// <summary>
        ///     Creates new <see cref="DragHandler{TElement}" /> with the provided packer <see cref="Func{TResult}" /> and
        ///     allowed <see cref="DragDropEffects" />
        /// </summary>
        /// <param name="packerFunction"></param>
        /// <param name="allowedEffects"></param>
        public DragHandler(Func<TElement, IDataObject> packerFunction, DragDropEffects allowedEffects)
            : this(packerFunction)
        {
            AllowedEffects = allowedEffects;
        }

        /// <summary>
        ///     Creates new <see cref="DragHandler{TElement}" /> with the provided packer <see cref="Func{TResult}" />
        /// </summary>
        /// <param name="packerFunction"></param>
        public DragHandler(Func<TElement, IDataObject> packerFunction)
        {
            PackerFunction = packerFunction ?? throw new ArgumentNullException(nameof(packerFunction));
        }

        /// <summary>
        ///     Registers a mouse button event as a potential drag drop start <see cref="Point" />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void RegisterDragStartPoint(TElement sender, MouseButtonEventArgs args)
        {
            if (StartPoint == null) StartPoint = args.GetPosition(sender);
        }

        /// <summary>
        ///     Deletes the last registered starting <see cref="Point" />
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DeleteDragStartPoint(TElement sender, MouseButtonEventArgs args)
        {
            StartPoint = null;
        }

        /// <summary>
        ///     Tries to start a drag drop using the provided <see cref="Func{TResult}" /> for data packing if the internal
        ///     conditions are met
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// <param name="effects"></param>
        public void TryDoDragDrop(TElement sender, MouseEventArgs args)
        {
            if (StartPoint == null) return;

            var dragVector = args.GetPosition(sender) - StartPoint.Value;
            if (dragVector.Length < MinLengthBeforeDrag) return;

            DragDrop.DoDragDrop(sender, PackerFunction(sender), AllowedEffects);
            StartPoint = null;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using HelixToolkit.Wpf;
using Mocassin.Framework.Extensions;
using Mocassin.UI.Base.Commands;
using Mocassin.UI.GUI.Base.ViewModels;
using Mocassin.UI.GUI.Controls.Visualizer.Objects;

namespace Mocassin.UI.GUI.Controls.Visualizer.DataControl
{
    /// <summary>
    ///     A <see cref="ViewModelBase" /> instance that manages a <see cref="Visual3D" /> collection an affiliated data for a
    ///     <see cref="HelixViewport3D" />
    /// </summary>
    public class Viewport3DViewModel : ViewModelBase
    {
        private LightSetup lightSetup;
        private Visual3D selectedVisual;
        private bool isAutoUpdating;

        /// <summary>
        ///     Get the default light setup for the visual collection
        /// </summary>
        public LightSetup DefaultLightSetup { get; }

        /// <summary>
        ///     Get the <see cref="ObservableCollection{T}" /> for the <see cref="Visual3D" /> data
        /// </summary>
        public ObservableCollection<Visual3D> Visuals { get; }

        /// <summary>
        ///     Get the set of <see cref="IVisualGroupViewModel" /> instances that define the content
        /// </summary>
        public ObservableCollection<IVisualGroupViewModel> VisualGroups { get; }

        /// <summary>
        ///     Get or set the <see cref="LightSetup" /> of the collection
        /// </summary>
        public LightSetup LightSetup
        {
            get => lightSetup;
            set
            {
                ReplaceUniqueVisual(lightSetup, value);
                SetProperty(ref lightSetup, value);
            }
        }

        /// <summary>
        ///     Get or set the currently selected <see cref="Visual3D" />
        /// </summary>
        public Visual3D SelectedVisual
        {
            get => selectedVisual;
            set => SetProperty(ref selectedVisual, value);
        }

        /// <summary>
        ///     Get or set a boolean flag if the system should auto update changes on the visual group
        /// </summary>
        public bool IsAutoUpdating
        {
            get => isAutoUpdating;
            set => SetProperty(ref isAutoUpdating, value);
        }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to clear the visual data and reset to minimum contents
        /// </summary>
        public ParameterlessCommand ClearVisualCommand { get; }

        /// <summary>
        ///     Get a <see cref="ParameterlessCommand" /> to resynchronize the visual collection with the visual groups
        /// </summary>
        public ParameterlessCommand UpdateVisualCommand { get; }

        /// <summary>
        ///     Create sa new <see cref="Viewport3DViewModel" /> with default settings
        /// </summary>
        public Viewport3DViewModel()
        {
            isAutoUpdating = true;
            Visuals = new ObservableCollection<Visual3D>();
            VisualGroups = new ObservableCollection<IVisualGroupViewModel>();
            ClearVisualCommand = new RelayCommand(ClearVisual);
            UpdateVisualCommand = new RelayCommand(UpdateVisual);
            DefaultLightSetup = new DefaultLights();
            ClearVisual();
        }


        /// <summary>
        ///     Resets the visual collection to contain only the light setup
        /// </summary>
        public void ClearVisual()
        {
            ExecuteOnDispatcher(() =>
            {
                Visuals.Clear();
                Visuals.Add(LightSetup ?? DefaultLightSetup);
            });
        }

        /// <summary>
        ///     Updates the visual collection to the current state of the visual groups
        /// </summary>
        public void UpdateVisual()
        {
            ClearVisual();
            ExecuteOnDispatcher(() =>
            {
                foreach (var visualGroup in VisualGroups.Where(x => x.IsVisible)) Visuals.AddMany(visualGroup.Items);
            });
        }

        /// <summary>
        ///     Clears the visual groups
        /// </summary>
        public void ClearVisualGroups()
        {
            ExecuteOnDispatcher(() => VisualGroups.Clear());
            ClearVisual();
        }

        /// <summary>
        ///     Adds a new <see cref="IVisualGroupViewModel"/> to the view port
        /// </summary>
        /// <param name="visualGroup"></param>
        public void AddVisualGroup(IVisualGroupViewModel visualGroup)
        {
            if (visualGroup == null) throw new ArgumentNullException(nameof(visualGroup));
            if (VisualGroups.Contains(visualGroup)) return;

            visualGroup.PropertyChanged += AutoUpdateVisualInternal;
            ExecuteOnDispatcher(() => VisualGroups.Add(visualGroup));
        }

        /// <summary>
        ///     Adds a sequence of <see cref="Visual3D"/> objects as a new named <see cref="IVisualGroupViewModel"/>
        /// </summary>
        /// <param name="visuals"></param>
        /// <param name="name"></param>
        /// <param name="isVisible"></param>
        public void AddVisualGroup<T>(IEnumerable<T> visuals, string name, bool isVisible = true) where T : Visual3D
        {
            var visualGroup = new VisualGroupViewModel<T>
            {
                IsVisible = isVisible, Name = name ?? "New group", Visuals = visuals.ToList()
            };
            AddVisualGroup(visualGroup);
        }

        /// <summary>
        ///     Replaces a unique visual component of the collection
        /// </summary>
        /// <param name="oldVisual"></param>
        /// <param name="newVisual"></param>
        private void ReplaceUniqueVisual(Visual3D oldVisual, Visual3D newVisual)
        {
            Visuals.Remove(oldVisual);
            Visuals.Add(newVisual);
        }

        /// <summary>
        ///     Creates a <see cref="Visual3D" /> from a data object, and ensures that the API call is made from the dispatcher
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TVisual"></typeparam>
        /// <param name="data"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public TVisual CreateVisual<TData, TVisual>(TData data, Func<TData, TVisual> generator)
        {
            return ExecuteOnDispatcher(() => generator(data));
        }

        /// <summary>
        ///     Creates a read only collection of <see cref="Visual3D" /> from a data object sequence and a generator function, and
        ///     ensures that the API call is made from the dispatcher
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        /// <typeparam name="TVisual"></typeparam>
        /// <param name="data"></param>
        /// <param name="generator"></param>
        /// <returns></returns>
        public ReadOnlyCollection<TVisual> CreateVisual<TData, TVisual>(ICollection<TData> data, Func<TData, TVisual> generator)
        {
            ReadOnlyCollection<TVisual> CreateInternal()
            {
                var result = new List<TVisual>(data.Count);
                result.AddRange(data.Select(generator));

                return result.AsReadOnly();
            }

            return ExecuteOnDispatcher(CreateInternal);
        }

        /// <summary>
        ///     Get a generator delegate for creating <see cref="SphereVisual3D" /> around a center <see cref="Point3D" />
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="fillBrush"></param>
        /// <returns></returns>
        public Func<Point3D, SphereVisual3D> CreateSphereGenerator(double radius, Brush fillBrush)
        {
            SphereVisual3D GeneratorInternal(Point3D center)
            {
                return new SphereVisual3D
                {
                    Center = center,
                    Fill = fillBrush,
                    Radius = radius
                };
            }
            return GeneratorInternal;
        }

        /// <summary>
        ///     Get a generator delegate for creating <see cref="CubeVisual3D" /> around a center <see cref="Point3D" />
        /// </summary>
        /// <param name="sideLength"></param>
        /// <param name="fillBrush"></param>
        /// <returns></returns>
        public Func<Point3D, CubeVisual3D> CreateCubeGenerator(double sideLength, Brush fillBrush)
        {
            CubeVisual3D GeneratorInternal(Point3D center)
            {
                return new CubeVisual3D
                {
                    Center = center,
                    Fill = fillBrush,
                    SideLength = sideLength
                };
            }
            return GeneratorInternal;
        }

        /// <summary>
        ///     Get a generator delegate for creating <see cref="ArrowVisual3D" /> from start <see cref="Point3D" /> to end <see cref="Point3D" />
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="fillBrush"></param>
        /// <returns></returns>
        public Func<(Point3D Start, Point3D End), ArrowVisual3D> CreatePointToPointArrowGenerator(double diameter, Brush fillBrush)
        {
            ArrowVisual3D GeneratorInternal((Point3D, Point3D) points)
            {
                return new ArrowVisual3D
                {
                    Point1 = points.Item1,
                    Point2 = points.Item2,
                    Fill = fillBrush,
                    Diameter = diameter
                };
            }
            return GeneratorInternal;
        }

        /// <summary>
        ///     Get a generator delegate for creating <see cref="ArrowVisual3D" /> from start <see cref="Point3D" /> and direction <see cref="Vector3D" />
        /// </summary>
        /// <param name="diameter"></param>
        /// <param name="fillBrush"></param>
        /// <returns></returns>
        public Func<(Point3D Start, Vector3D Dir), ArrowVisual3D> CreateDirectionArrowGenerator(double diameter, Brush fillBrush)
        {
            ArrowVisual3D GeneratorInternal((Point3D, Vector3D) data)
            {
                return new ArrowVisual3D
                {
                    Point1 = data.Item1,
                    Direction = data.Item2,
                    Fill = fillBrush,
                    Diameter = diameter
                };
            }
            return GeneratorInternal;
        }

        /// <summary>
        ///     Update visual as an event delegate that triggers only if the system is set to auto updates
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void AutoUpdateVisualInternal(object sender, PropertyChangedEventArgs args)
        {
            if (IsAutoUpdating) UpdateVisual();
        }
    }
}
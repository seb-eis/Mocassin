using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using Mocassin.UI.GUI.Properties;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.DxVisualizer.Viewport.Helper
{
    /// <summary>
    ///     Builder class for synchronous or asynchronous creation of <see cref="SceneNode" /> sets for 3D scenes
    /// </summary>
    public class DxSceneBuilder
    {
        private object NodeCollectionLock { get; } = new object();
        private object BuildTaskCollectionLock { get; } = new object();

        /// <summary>
        ///     Get or set a boolean flag if a model building call is in progress
        /// </summary>
        private bool IsCreatingModel { get; set; }

        /// <summary>
        ///     Get the <see cref="HashSet{T}" /> of active building <see cref="Task" /> instances
        /// </summary>
        private HashSet<Task> ActiveBuildTasks { get; }

        /// <summary>
        ///     Get the <see cref="HashSet{T}" /> of <see cref="SceneNode" /> instances. Calling this
        /// </summary>
        private HashSet<SceneNode> SceneNodes { get; }

        /// <summary>
        ///     Creates a new <see cref="DxSceneBuilder" /> with the provided initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public DxSceneBuilder(int capacity = 10)
        {
            SceneNodes = new HashSet<SceneNode>(capacity);
            ActiveBuildTasks = new HashSet<Task>(capacity);
        }

        /// <summary>
        ///     Adds a <see cref="SceneNode" /> to the <see cref="DxSceneBuilder" />. This call is thread save
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(SceneNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            lock (NodeCollectionLock)
            {
                SceneNodes.Add(node);
            }
        }

        /// <summary>
        ///     Synchronously creates the unified <see cref="SceneNodeGroupModel3D"/>. This method will throw if any build tasks are still pending
        /// </summary>
        /// <param name="clear"></param>
        /// <param name="dispatcher"></param>
        /// <exception cref="InvalidOperationException">If active build tasks collection is not empty or a  build task is added during the operation</exception>
        /// <returns></returns>
        public SceneNodeGroupModel3D ToModel(bool clear = true, Dispatcher dispatcher = null)
        {
            dispatcher ??= Application.Current.Dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            IsCreatingModel = true;
            try
            {
                if (ActiveBuildTasks.Count != 0) throw new InvalidOperationException("Cannot synchronously build the model if pending build tasks exist.");
                var result = dispatcher.CheckAccess() ? CreateModel() : dispatcher.Invoke(CreateModel);
                if (clear) ClearNodes();
                IsCreatingModel = false;
                return result;
            }
            finally
            {
                IsCreatingModel = false;
            }
        }

        /// <summary>
        ///     Asynchronously awaits all active build tasks and builds a unified a <see cref="SceneNodeGroupModel3D"/>
        /// </summary>
        /// <param name="clear"></param>
        /// <param name="dispatcher"></param>
        /// <remarks>Application.Current is used to get a <see cref="Dispatcher"/> if none is specified</remarks>
        /// <returns></returns>
        public Task<SceneNodeGroupModel3D> ToModelAsync(bool clear = true, Dispatcher dispatcher = null)
        {
            return ToModelInternal(clear, dispatcher);
        }

        /// <summary>
        ///     Internal implementation of the <see cref="SceneNodeGroupModel3D"/> build process
        /// </summary>
        /// <param name="clear"></param>
        /// <param name="dispatcher"></param>
        /// <returns></returns>
        private async Task<SceneNodeGroupModel3D> ToModelInternal(bool clear = true, Dispatcher dispatcher = null)
        {
            dispatcher ??= Application.Current.Dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            IsCreatingModel = true;
            if (ActiveBuildTasks.Count != 0) await GetBuildAwaitTask();
            var result = dispatcher.CheckAccess() ? CreateModel() : dispatcher.Invoke(CreateModel);
            if (clear) ClearNodes();
            IsCreatingModel = false;
            return result;
        }

        /// <summary>
        ///     Creates the <see cref="SceneNodeGroupModel3D"/>
        /// </summary>
        /// <returns></returns>
        private SceneNodeGroupModel3D CreateModel()
        {
            var result = new SceneNodeGroupModel3D();
            foreach (var node in SceneNodes) result.AddNode(node);
            return result;
        }

        /// <summary>
        ///     Clears all scene nodes
        /// </summary>
        public void ClearNodes()
        {
            lock (NodeCollectionLock)
            {
                SceneNodes.Clear();
            }
        }

        /// <summary>
        ///     Adds a set of of <see cref="MeshNode" /> transforms sharing a common <see cref="MeshGeometry3D" /> and <see cref="MaterialCore" />
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        /// <param name="callback"></param>
        public void AddMeshTransforms(MeshGeometry3D geometry, MaterialCore material, IList<Matrix> transforms, Action<MeshNode> callback = null)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (material == null) throw new ArgumentNullException(nameof(material));
            if (transforms == null) throw new ArgumentNullException(nameof(transforms));
            if (transforms.Count == 0) throw new ArgumentException(Resources.ErrorMsg_Argument_EmptyCollection, nameof(transforms));

            foreach (var matrix in transforms)
            {
                var node = new MeshNode {Geometry = geometry, Material = material, ModelMatrix = matrix};
                callback?.Invoke(node);
                AddNode(node);
            }
        }

        /// <summary>
        ///     Starts performing the <see cref="AddMeshTransforms" /> as a background task
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Provided material is not frozen</exception>
        public Task BeginAddMeshTransforms(MeshGeometry3D geometry, MaterialCore material, IList<Matrix> transforms, Action<MeshNode> callback = null)
        {
            return RunBuildTask(() => AddMeshTransforms(geometry, material, transforms, callback));
        }

        /// <summary>
        ///     Adds a new <see cref="BatchedMeshNode" /> of mesh transforms sharing a common <see cref="MeshGeometry3D" /> and <see cref="MaterialCore" />
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        /// <param name="callback"></param>
        public void AddBatchedMeshTransforms(MeshGeometry3D geometry, MaterialCore material, IList<Matrix> transforms, Action<BatchedMeshNode> callback = null)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (material == null) throw new ArgumentNullException(nameof(material));
            if (transforms == null) throw new ArgumentNullException(nameof(transforms));
            if (transforms.Count == 0) throw new ArgumentException(Resources.ErrorMsg_Argument_EmptyCollection, nameof(transforms));

            var geometries = new BatchedMeshGeometryConfig[transforms.Count];
            for (var i = 0; i < transforms.Count; i++) geometries[i] = new BatchedMeshGeometryConfig(geometry, transforms[i], 0);
            var batchedNode = new BatchedMeshNode {Material = material, Geometries = geometries};
            callback?.Invoke(batchedNode);
            AddNode(batchedNode);
        }

        /// <summary>
        ///     Begins the <see cref="AddBatchedMeshTransforms" /> as a background task
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task BeginAddBatchedMeshTransforms(MeshGeometry3D geometry, MaterialCore material, IList<Matrix> transforms, Action<BatchedMeshNode> callback = null)
        {
            return RunBuildTask(() => AddBatchedMeshTransforms(geometry, material, transforms, callback));
        }

        /// <summary>
        ///     Adds a <see cref="LineGeometry3D"/> using the provided model <see cref="Matrix"/> and <see cref="LineMaterialCore"/>
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transform"></param>
        /// <param name="callback"></param>
        public void AddLineGeometry(LineGeometry3D geometry, LineMaterialCore material, in Matrix transform, Action<LineNode> callback = null)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (material == null) throw new ArgumentNullException(nameof(material));
            var node = new LineNode {Geometry = geometry, Material = material, ModelMatrix = transform};
            callback?.Invoke(node);
            AddNode(node);
        }

        /// <summary>
        ///     Adds a default styled screen spaced coordinate system with custom vectors
        /// </summary>
        /// <param name="vectorA"></param>
        /// <param name="vectorB"></param>
        /// <param name="vectorC"></param>
        /// <param name="material"></param>
        /// <param name="callback"></param>
        public void AddCoordinateSystem(in Vector3 vectorA, in Vector3 vectorB, in Vector3 vectorC, MaterialCore material, Action<SceneNode> callback = null)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Attaches a custom build <see cref="Task"/> that requires awaiting when creating the model
        /// </summary>
        /// <param name="task"></param>
        /// <remarks>Warning: Never attach a task that itself attaches a build task to the builder instance, this causes an <see cref="InvalidOperationException"/> on model creation. </remarks>
        public void AttachCustomTask(Task task)
        {
            AttachBuildTask(task);
            task.ContinueWith(DetachBuildTask);
        }

        /// <summary>
        ///     Runs an <see cref="Action" /> as a build <see cref="Task" /> that detaches itself on completion. Returned <see cref="Task" /> completes after the build process has detached from the scene builder
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private Task RunBuildTask(Action action)
        {
            var mainTask = Task.Run(action);
            AttachBuildTask(mainTask);
            return mainTask.ContinueWith(DetachBuildTask);
        }

        /// <summary>
        ///     Adds a <see cref="Task" /> to the active builds tasks
        /// </summary>
        /// <param name="task"></param>
        private void AttachBuildTask(Task task)
        {
            lock (BuildTaskCollectionLock)
            {
                if (IsCreatingModel) throw new InvalidOperationException("Cannot add scene node build tasks while the builder is creating a model.");
                ActiveBuildTasks.Add(task);
            }
        }

        /// <summary>
        ///     Removes a <see cref="Task" /> from the active builds tasks
        /// </summary>
        /// <param name="task"></param>
        private void DetachBuildTask(Task task)
        {
            lock (BuildTaskCollectionLock)
            {
                ActiveBuildTasks.Remove(task);
            }
        }

        /// <summary>
        ///     Get a <see cref="Task"/> to await completion fo all active build tasks
        /// </summary>
        /// <returns></returns>
        private Task GetBuildAwaitTask()
        {
            lock (BuildTaskCollectionLock)
            {
                return Task.WhenAll(ActiveBuildTasks.ToList());
            }
        }
    }
}
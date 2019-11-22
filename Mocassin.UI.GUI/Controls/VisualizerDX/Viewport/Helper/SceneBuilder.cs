﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using HelixToolkit.Wpf.SharpDX;
using HelixToolkit.Wpf.SharpDX.Model;
using HelixToolkit.Wpf.SharpDX.Model.Scene;
using SharpDX;

namespace Mocassin.UI.GUI.Controls.VisualizerDX.Viewport.Helper
{
    /// <summary>
    ///     Build class for multi-threaded creation <see cref="SceneNode"/> sets for 3D scenes
    /// </summary>
    public class SceneBuilder
    {
        private object NodeContainerLock { get; } = new object();
        private object BuildTasksLock { get; } = new object();

        /// <summary>
        ///     Get the <see cref="HashSet{T}"/> of active building <see cref="Task"/> instances
        /// </summary>
        private HashSet<Task> ActiveBuildTasks { get; }

        /// <summary>
        ///     Get the <see cref="HashSet{T}"/> of <see cref="SceneNode"/> instances. Calling this
        /// </summary>
        private HashSet<SceneNode> SceneNodes { get; }

        /// <summary>
        ///     Creates a new <see cref="SceneBuilder"/> with the provided initial capacity
        /// </summary>
        /// <param name="capacity"></param>
        public SceneBuilder(int capacity = 100)
        {
            SceneNodes = new HashSet<SceneNode>(capacity);
            ActiveBuildTasks = new HashSet<Task>();
        }

        /// <summary>
        ///     Adds a <see cref="SceneNode"/> to the <see cref="SceneBuilder"/>. This call is thread save
        /// </summary>
        /// <param name="node"></param>
        public void AddNode(SceneNode node)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            lock (NodeContainerLock)
            {
                SceneNodes.Add(node);
            }
        }

        /// <summary>
        ///     Creates the <see cref="SceneNodeGroupModel3D"/> of the scene and cleans the builders node collection if requested
        /// </summary>
        /// <returns></returns>
        public SceneNodeGroupModel3D ToModel(bool clear = true)
        {
            lock (NodeContainerLock)
            {
                var result = new SceneNodeGroupModel3D();
                foreach (var node in SceneNodes) result.AddNode(node);   
                if (clear) SceneNodes.Clear();
                return result;
            }
        }

        /// <summary>
        ///     Adds a new <see cref="GroupNode"/> of <see cref="MeshNode"/> transforms sharing a common <see cref="Geometry3D"/> and <see cref="Material"/>
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        public void AddMeshTransforms(Geometry3D geometry, Material material, IList<Matrix> transforms, Action<GroupNode> callback = null)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (material == null) throw new ArgumentNullException(nameof(material));
            if (transforms == null) throw new ArgumentNullException(nameof(transforms));
            if (transforms.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(transforms));

            var groupNode = new GroupNode();
            foreach (var matrix in transforms)
            {
                var node = new MeshNode {Geometry = geometry, Material = material, ModelMatrix = matrix};
                groupNode.AddChildNode(node);
            }
            callback?.Invoke(groupNode);
            AddNode(groupNode);
        }

        /// <summary>
        ///     Performs the <see cref="AddMeshTransforms"/> action asynchronously.
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">Provided material is not frozen</exception>
        public Task AddMeshTransformsAsync(Geometry3D geometry, Material material, IList<Matrix> transforms, Action<GroupNode> callback = null)
        {
            ThrowIfNotFrozen(material);
            return RunBuildTask(() => AddMeshTransforms(geometry, material, transforms, callback));
        }

        /// <summary>
        ///     Adds a new <see cref="BatchedMeshNode"/> of mesh transforms sharing a common <see cref="Geometry3D"/> and <see cref="Material"/>
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        public void AddBatchedMeshTransforms(Geometry3D geometry, Material material, IList<Matrix> transforms, Action<BatchedMeshNode> callback = null)
        {
            if (geometry == null) throw new ArgumentNullException(nameof(geometry));
            if (material == null) throw new ArgumentNullException(nameof(material));
            if (transforms == null) throw new ArgumentNullException(nameof(transforms));
            if (transforms.Count == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(transforms));

            var geometries = new BatchedMeshGeometryConfig[transforms.Count];
            for (var i = 0; i < transforms.Count; i++) geometries[i] = new BatchedMeshGeometryConfig(geometry, transforms[i], 0);
            var batchedNode = new BatchedMeshNode {Material = material, Geometries = geometries};
            callback?.Invoke(batchedNode);
            AddNode(batchedNode);
        }

        /// <summary>
        ///     Performs the <see cref="AddBatchedMeshTransforms"/> action asynchronously
        /// </summary>
        /// <param name="geometry"></param>
        /// <param name="material"></param>
        /// <param name="transforms"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public Task AddBatchedMeshTransformsAsync(Geometry3D geometry, Material material, IList<Matrix> transforms, Action<BatchedMeshNode> callback = null)
        {
            ThrowIfNotFrozen(material);
            return RunBuildTask(() => AddBatchedMeshTransforms(geometry, material, transforms, callback));
        }

        /// <summary>
        ///     Runs an <see cref="Action"/> as a build <see cref="Task"/>. Returned <see cref="Task"/> completes after the build process is detached from the scene builder
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        private Task RunBuildTask(Action action)
        {
            var mainTask = Task.Run(action);
            AddBuildTask(mainTask);
            return mainTask.ContinueWith(RemoveBuildTask);
        }

        /// <summary>
        ///     Adds a <see cref="Task"/> to the active builds tasks
        /// </summary>
        /// <param name="task"></param>
        private void AddBuildTask(Task task)
        {
            lock (BuildTasksLock)
            {
                ActiveBuildTasks.Add(task);
            }
        }

        /// <summary>
        ///     Removes a <see cref="Task"/> from the active builds tasks
        /// </summary>
        /// <param name="task"></param>
        private void RemoveBuildTask(Task task)
        {
            lock (BuildTasksLock)
            {
                ActiveBuildTasks.Remove(task);
            }
        }

        /// <summary>
        ///     Throws if a <see cref="Freezable"/> object is not frozen
        /// </summary>
        /// <param name="freezable"></param>
        private void ThrowIfNotFrozen(Freezable freezable)
        {
            if (freezable == null) throw new ArgumentNullException(nameof(freezable));
            if (!freezable.IsFrozen) throw new InvalidOperationException("Operation requires involved Freezable objects to be frozen.");
        }
    }
}
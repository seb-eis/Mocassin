using System;
using System.Threading.Tasks;
using System.Windows;

namespace Mocassin.UI.GUI.Base.Loading
{
    /// <summary>
    ///     Wrapper for async execution of tasks while a progress <see cref="Window" /> is shown that blocks the UI thread
    /// </summary>
    public class WindowedBackgroundTask : IDisposable
    {
        /// <summary>
        ///     The <see cref="Task" /> that is executed in the background
        /// </summary>
        private readonly Task task;

        /// <summary>
        ///     The <see cref="Window" /> that is shown as long as the executor is not
        /// </summary>
        private readonly Window window;

        /// <summary>
        ///     Creates a new <see cref="WindowedBackgroundTask" /> with the provided window and task
        /// </summary>
        /// <param name="window"></param>
        /// <param name="task"></param>
        public WindowedBackgroundTask(Window window, Task task)
        {
            this.window = window ?? throw new ArgumentNullException(nameof(window));
            this.task = task ?? throw new ArgumentNullException(nameof(task));
        }

        /// <inheritdoc />
        public virtual void Dispose()
        {
            window.Close();
        }

        /// <summary>
        ///     Internal execution routine that completes once the wrapped work <see cref="Task" /> is finished
        /// </summary>
        /// <returns></returns>
        private async Task ExecuteInternal()
        {
            if (!task.IsCompleted)
            {
                using (this)
                {
                    task.Start();
                    window.Show();
                    await Task.WhenAll(task);
                }
            }
        }

        /// <summary>
        ///     Run the passed <see cref="Action" /> while showing the passed <see cref="Window" />
        /// </summary>
        /// <param name="window"></param>
        /// <param name="action"></param>
        public static async void Run(Window window, Action action)
        {
            using var task = new WindowedBackgroundTask(window, new Task(action));
            await task.ExecuteInternal();
        }

        /// <summary>
        ///     Run the passed <see cref="Action" /> while showing a default <see cref="LoadingWindow" />
        /// </summary>
        /// <param name="action"></param>
        public static void RunWithLoadingWindow(Action action)
        {
            Run(new LoadingWindow(), action);
        }

        /// <summary>
        ///     Run the passed <see cref="Action" /> while showing a default <see cref="LoadingWindow" /> and returns an awaitable
        ///     task
        /// </summary>
        /// <param name="action"></param>
        public static Task StartWithLoadingWindow(Action action)
        {
            return Task.Run(() => RunWithLoadingWindow(action));
        }
    }
}
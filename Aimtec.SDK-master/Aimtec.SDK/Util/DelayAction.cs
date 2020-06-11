namespace Aimtec.SDK.Util
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    /// <summary>
    ///     Runs actions at a later time.
    /// </summary>
    public class DelayAction
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="DelayAction" /> class.
        /// </summary>
        static DelayAction()
        {
            Game.OnUpdate += UpdateDelayedActions;
        }

        #endregion

        #region Properties

        /// <summary>
        ///     Gets or sets the task queue.
        /// </summary>
        /// <value>The task queue.</value>
        private static List<Tuple<int, Action, CancellationToken>> TaskQueue { get; } =
            new List<Tuple<int, Action, CancellationToken>>();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Queues the action to be run after a specified time.
        /// </summary>
        /// <param name="milliseconds">The time time to delay in milliseconds.</param>
        /// <param name="action">The action.</param>
        /// <returns>A <see cref="CancellationToken" /> that can be used to cancel the action.</returns>
        public static CancellationTokenSource Queue(int milliseconds, Action action)
        {
            var tokenSource = new CancellationTokenSource();
            Queue(milliseconds, action, tokenSource.Token);

            return tokenSource;
        }

        /// <summary>
        ///     Queues the specified action after a specified delay.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        /// <param name="action">The action.</param>
        /// <param name="token">The token.</param>
        public static void Queue(int milliseconds, Action action, CancellationToken token)
        {
            if (milliseconds == 0)
            {
                action();
                return;
            }

            TaskQueue.Add(new Tuple<int, Action, CancellationToken>(Game.TickCount + milliseconds, action, token));
            SortList();
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Sorts the list.
        /// </summary>
        private static void SortList()
        {
            // sort list backwards
            TaskQueue.Sort((x, y) => x.Item1 == y.Item1 ? 0 : x.Item1 > y.Item1 ? -1 : 1);
        }

        /// <summary>
        ///     Updates the delayed actions.
        /// </summary>
        private static void UpdateDelayedActions()
        {
            for (var i = TaskQueue.Count - 1; i >= 0; i--)
            {
                var task = TaskQueue[i];

                if (task.Item3.IsCancellationRequested)
                {
                    TaskQueue.RemoveAt(i);
                    continue;
                }

                if (Game.TickCount >= task.Item1)
                {
                    task.Item2();
                    TaskQueue.RemoveAt(i);
                }
                else
                {
                    return;
                }
            }
        }

        #endregion
    }
}
namespace Aimtec.SDK.Events
{
    using System;

    /// <summary>
    ///     Class GameEvents.
    /// </summary>
    public static class GameEvents
    {
        #region Constructors and Destructors

        /// <summary>
        ///     Initializes static members of the <see cref="GameEvents" /> class.
        /// </summary>
        static GameEvents()
        {
            Game.OnStart += GameStartHandler;
            Game.OnUpdate += GameStartHandler;
        }

        #endregion

        #region Delegates

        /// <summary>
        ///     The GameStart Delegate
        /// </summary>
        public delegate void GameStartDelegate();

        #endregion

        #region Public Events

        /// <summary>
        ///     Occurs when the game is started.
        /// </summary>
        public static event GameStartDelegate GameStart;

        #endregion

        #region Methods

        /// <summary>
        ///     Handles the <see cref="GameStart" /> event.
        /// </summary>
        private static void GameStartHandler()
        {
            if (Game.Mode != GameMode.Running)
            {
                return;
            }

            Game.OnStart -= GameStartHandler;
            Game.OnUpdate -= GameStartHandler;

            //Call the Bootstrapper
            Bootstrap.Load();

            var invocationList = GameStart?.GetInvocationList();

            if (invocationList == null)
            {
                return;
            }

            foreach (var del in invocationList)
            {
                try
                {
                    del.DynamicInvoke();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        #endregion
    }
}
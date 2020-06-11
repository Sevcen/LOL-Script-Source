namespace Aimtec.SDK
{
    using System;
    using System.IO;
    using System.Reflection;

    using Aimtec.SDK.Damage;
    using Aimtec.SDK.Menu.Config;

    using NLog;
    using NLog.Config;
    using NLog.Layouts;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    /// <summary>
    ///     Class Bootstrap.
    /// </summary>
    public class Bootstrap // : ILibraryEntryPoint
    {
        #region Static Fields

        /// <summary>
        ///     Gets if the library has already been loaded and initalized
        /// </summary>
        private static bool alreadyLoaded;

        #endregion

        #region Properties

        /// <summary>
        ///     Gets the logger.
        /// </summary>
        /// <value>The logger.</value>
        private static Logger Logger { get; } = LogManager.GetCurrentClassLogger();

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     Initalizes the library.
        /// </summary>
        public static void Load()
        {
            if (alreadyLoaded)
            {
                return;
            }

            SetupLogging();
            LogUnhandledExceptions();

            DamageLibrary.LoadDamages();
            GlobalKeys.Load();
            TargetSelector.TargetSelector.Load();

            AimtecMenu.Instance.Attach();

            Logger.Info($"Aimtec.SDK version {Assembly.GetExecutingAssembly().GetName().Version} loaded.");

            alreadyLoaded = true;
        }

        #endregion

        #region Methods

        /// <summary>
        ///     Adds an event handler for unhandles exceptions and logs them accordingly.
        /// </summary>
        private static void LogUnhandledExceptions()
        {
            // FIXME this requires that the SDK be fully trusted by the AppDomain
            // this event is not raised for exceptions that corrupt the state of the process, such as stack overflows or access violations
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                var exception = args.ExceptionObject as Exception;
                var logLevel = args.IsTerminating ? LogLevel.Fatal : LogLevel.Error;

                if (exception == null)
                {
                    Logger.Log(logLevel, "An unknown unhandled exception occured.");
                    return;
                }

                // Is there a better way to do this? -Pixl
                if (exception.TargetSite.Module.Assembly.Equals(Assembly.GetExecutingAssembly()))
                {
                    Logger.Log(logLevel, exception, "An unhandled exception occured in Aimtec.SDK");
                }
            };
        }

        /// <summary>
        ///     Configures NLog to properly display and record log messages.
        /// </summary>
        private static void SetupLogging()
        {
            // Setup NLog with async console and file logging.
            // Only logs to file if the log level is greater or equal to the warn level.
            var layout = new SimpleLayout(
                "${longdate}|${pad:padding=5:inner=${level:uppercase=true}}|${pad:padding=30:inner=${callsite:includeNamespace=false}}|${message} ${exception:format=tostring}");
            var config = new LoggingConfiguration();

            var consoleTarget = new AsyncTargetWrapper(
                new ColoredConsoleTarget("ColoredConsoleTarget")
                {
                    Layout = layout,
                    DetectConsoleAvailable = false,
                    RowHighlightingRules =
                    {
                        new ConsoleRowHighlightingRule(
                            "level == LogLevel.Debug",
                            ConsoleOutputColor.White,
                            ConsoleOutputColor.NoChange),
                        new ConsoleRowHighlightingRule(
                            "level == LogLevel.Info",
                            ConsoleOutputColor.Cyan,
                            ConsoleOutputColor.NoChange),
                        new ConsoleRowHighlightingRule(
                            "level == LogLevel.Warn",
                            ConsoleOutputColor.Yellow,
                            ConsoleOutputColor.NoChange),
                        new ConsoleRowHighlightingRule(
                            "level == LogLevel.Error",
                            ConsoleOutputColor.Red,
                            ConsoleOutputColor.NoChange),
                        new ConsoleRowHighlightingRule(
                            "level == LogLevel.Fatal",
                            ConsoleOutputColor.Red,
                            ConsoleOutputColor.White)
                    }
                });

            config.AddTarget("AsyncWrapper1", consoleTarget);
            config.AddRule(LogLevel.Trace, LogLevel.Fatal, consoleTarget);

            var asyncFileTarget = new AsyncTargetWrapper(
                new FileTarget("FileTarget")
                {
                    Layout = layout,
                    LineEnding = LineEndingMode.Default,
                    DeleteOldFileOnStartup = true,
                    FileName = new SimpleLayout(
                        Path.Combine(
                            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                            "Aimtec.SDK",
                            "Aimtec.SDK.log")),
                    OpenFileCacheTimeout = 30,
                    KeepFileOpen = true
                });

            config.AddTarget("AsyncWrapper2", asyncFileTarget);
            config.AddRule(LogLevel.Warn, LogLevel.Fatal, asyncFileTarget);

            LogManager.Configuration = config;
        }

        #endregion
    }
}
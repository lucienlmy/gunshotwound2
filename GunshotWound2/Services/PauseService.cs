namespace GunshotWound2.Services {
    using Configs;
    using Utils;

    public sealed class PauseService {
        public delegate void PauseStateChangedDelegate(bool isPaused);

        private static int PAUSE_POST;

        private readonly Notifier notifier;
        private readonly LocaleConfig localeConfig;
        private readonly ILogger logger;
        private bool isPaused;

        public bool State => isPaused;

        public event PauseStateChangedDelegate PauseStateChanged;

        public PauseService(Notifier notifier, LocaleConfig localeConfig, ILogger logger) {
            this.notifier = notifier;
            this.localeConfig = localeConfig;
            this.logger = logger;
        }

        public void TogglePause() {
            isPaused = !isPaused;
            PauseStateChanged?.Invoke(isPaused);

#if DEBUG
            logger.WriteInfo($"Pause state changed to {isPaused}");
#endif

            PAUSE_POST = isPaused
                    ? notifier.ReplaceOne(localeConfig.GswIsPaused, blinking: true, PAUSE_POST, Notifier.Color.YELLOW)
                    : notifier.ReplaceOne(localeConfig.GswIsWorking, blinking: true, PAUSE_POST, Notifier.Color.GREEN);
        }
    }
}
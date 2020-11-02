namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// App.config style settings.
    /// </summary>
    /// <remarks>This is a simplistic way to manage debug/release app config. For a more sophisticated approach see
    /// <see href="https://www.andrewhoefling.com/Blog/Post/xamarin-app-configuration-control-your-app-settings"/>.
    /// </remarks>
    internal static class Constants
    {
        /// <summary>
        /// The number of targets on the play page.
        /// </summary>
        /// <remarks>This app is built with 4 targets but there's no reason why it couldn't be more (or fewer).
        /// In addition to changing this value the targets defined in the XAML and the colours defined in the
        /// GameViewModel would need to be updated.</remarks>
        public const int TargetCount = 4;

        /// <summary>
        /// The number of steps in the countdown after pressing Start and before the play phase begins.
        /// </summary>
        /// <remarks>In normal play each step is 1 second, in test each step is just a counter decrement.</remarks>
        public const int CountdownSteps = 3;

        /// <summary>
        /// The number of seconds in the play phase of a game.
        /// </summary>
#if (DEBUG)
        public const int PlaySeconds = 5;
#else
        public const int PlaySeconds = 30;
#endif

        /// <summary>
        /// The maximum number of results to keep in the 'best' scores.
        /// </summary>
        public const int MaxBestResults = 5;

        /// <summary>
        /// The maximum number of results to keep in the 'latest ' scores.
        /// </summary>
        public const int MaxLatestResults = 3;

        /// <summary>
        /// The public URL for the app's privacy policy.
        /// </summary>
        public const string PrivacyPolicyUrl = "https://www.rmjcs.com/simonish/privacy";

        /// <summary>
        /// The URL for the app's GitHub repo.
        /// </summary>
        public const string GitHubUrl = "https://github.com/rmjcsltd/Simonish";
    }
}

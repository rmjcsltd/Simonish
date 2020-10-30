using System.Threading;
using System.Threading.Tasks;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Pages;
using Rmjcs.Simonish.Services;
using Xamarin.Forms;

namespace Rmjcs.Simonish
{
    /// <summary>
    /// Represents an instance of the Simonish application.
    /// </summary>
    public partial class App : Application
    {
        private static bool _resultsLoadedCalled;

        /// <summary>
        /// Initialises a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            InitializeComponent();

            MainPage = new MainPage();
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        protected override void OnStart()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            if (!_resultsLoadedCalled)
            {
                ResultsService resultsService = ViewModelLocator.ResolveScoresService();
                IFileHelper fileHelper = ViewModelLocator.ResolveIFileHelper();

                Task task = Task.Run(resultsService.LoadResults);

                // If LoadResults errors it should be safe to just log it and continue.
                // As well as logging any task exception this also observes it, avoiding any chance of a TaskScheduler.UnobservedTaskException.
                task.ContinueWith(t => fileHelper.LogException(t.Exception.GetBaseException()),
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.Current);

                _resultsLoadedCalled = true;
            }
        }
    }
}

using System;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Pages;
using Rmjcs.Simonish.Services;
using Rmjcs.Simonish.ViewModels;
using Xamarin.Forms;

namespace Rmjcs.Simonish
{
    /// <summary>
    /// Represents an instance of the Simonish application.
    /// </summary>
    public sealed partial class App : Application
    {
        private bool _isStarted;

        /// <summary>
        /// Initialises a new instance of the <see cref="App"/> class.
        /// </summary>
        public App()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            InitializeComponent();

            MainPage = new MainPage();
        }

        /// <summary>
        /// Called when the application starts.
        /// </summary>
        /// <exception cref="InvalidOperationException">Failed to get GameViewModel from MainPage.InternalChildren.</exception>
        /// <exception cref="InvalidOperationException">Failed to get ResultsViewModel from MainPage.InternalChildren.</exception>
        protected override void OnStart()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // It is possible for OnStart to be called more than once so we need to track whether we have already done this.
            //    see https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/app-lifecycle

            if (!_isStarted)
            {
                GameViewModel gameViewModel = null;
                ResultsViewModel resultsViewModel = null;

                foreach (Element element in MainPage.InternalChildren)
                {
                    if (element is GamePage gamePage) gameViewModel = (GameViewModel)gamePage.BindingContext;
                    if (element is ResultsPage resultsPage) resultsViewModel = (ResultsViewModel)resultsPage.BindingContext;
                }

                if (gameViewModel == null)
                {
                    throw new InvalidOperationException("Failed to get GameViewModel from MainPage.InternalChildren.");
                }

                if (resultsViewModel == null)
                {
                    throw new InvalidOperationException("Failed to get ResultsViewModel from MainPage.InternalChildren.");
                }

                INewResultSource source = gameViewModel.GetNewResultSource();
                INewResultListener listener = resultsViewModel.GetNewResultListener();

                source.NewResult += listener.NewResultSourceNewResult;

                _isStarted = true;
            }
        }
    }
}

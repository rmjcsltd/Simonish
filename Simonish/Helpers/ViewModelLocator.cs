using System;
using System.Threading;
using System.Threading.Tasks;
using Rmjcs.Simonish.Pages;
using Rmjcs.Simonish.Services;
using Rmjcs.Simonish.ViewModels;
using Xamarin.Forms;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// A static class used to assign ViewModels to pages and manage application singletons.
    /// </summary>
    /// <remarks>Originally based on <see href="https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Mobile/eShopOnContainers/eShopOnContainers.Core/ViewModels/Base/ViewModelLocator.cs"/>.
    /// This class originally used TinyIoC as the inversion of control container but this app doesn't warrant one - dependency injection is easily achieved without a 3rd party helper.</remarks>
    internal static class ViewModelLocator
    {
        // These are the objects that there must only be one of in the app.
        // The classes do not use the singleton pattern because it's not very unit testable.
        private static readonly IXamarinWrapper XamarinWrapper;
        private static readonly IFileHelper FileHelper;

        // Here we create a new attached bindable property called AutoWireViewModel.
        // In the XAML of each page we set this property to True which triggers the propertyChanged event (because default(bool) = false). 
        // In the propertyChanged event handler we examine the type of the page and create and assign the appropriate view model.
        public static readonly BindableProperty AutoWireViewModelProperty =
            BindableProperty.CreateAttached("AutoWireViewModel", typeof(bool), typeof(ViewModelLocator), default(bool), propertyChanged: OnAutoWireViewModelChanged);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1810:Initialize reference type static fields inline", Justification = "I want to use WriteDebugEntryMessage to track when this is called.")]
        static ViewModelLocator()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), null);

            XamarinWrapper = new XamarinWrapper();
            FileHelper = new FileHelper(XamarinWrapper);
        }

        // The getter for the AutoWireViewModel property.
        public static bool GetAutoWireViewModel(BindableObject bindable)
        {
            return (bool)bindable.GetValue(AutoWireViewModelProperty);
        }

        // The setter for the AutoWireViewModel.
        public static void SetAutoWireViewModel(BindableObject bindable, bool value)
        {
            bindable.SetValue(AutoWireViewModelProperty, value);
        }

        /// <summary>
        /// Create and assign a view model to the supplied page.
        /// </summary>
        /// <param name="bindable">The page to which to assign </param>
        /// <param name="oldValue">The old property value (not used here).</param>
        /// <param name="newValue">The new property value (not used here).</param>
        private static void OnAutoWireViewModelChanged(BindableObject bindable, object oldValue, object newValue)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), null);

            // The app may (or may not) have been restarted, make sure the statically cached synchronisation context is still correct.
            XamarinWrapper.DebugAssertMainSynchronizationContextIsCorrect();

            object viewModel;

            if (bindable.GetType() == typeof(AboutPage))
            {
                viewModel = new AboutViewModel(XamarinWrapper);
            }
            else if (bindable.GetType() == typeof(GamePage))
            {
                ITimer timer = new OneSecondTimer();
                GameService gameService = new GameService(XamarinWrapper, timer);
                viewModel = new GameViewModel(XamarinWrapper, gameService);
            }
            else if (bindable.GetType() == typeof(ResultsPage))
            {
                ResultsService resultsService = new ResultsService(XamarinWrapper, FileHelper);
                viewModel = new ResultsViewModel(XamarinWrapper, resultsService);

                Task task = Task.Run(resultsService.LoadResults);

                // If LoadResults errors it should be safe to just log it and continue.
                // As well as logging any task exception this also observes it, avoiding any chance of a TaskScheduler.UnobservedTaskException.
                task.ContinueWith(t => FileHelper.LogException(t.Exception.GetBaseException()),
                    CancellationToken.None,
                    TaskContinuationOptions.OnlyOnFaulted,
                    TaskScheduler.Current);
            }
            else
            {
                throw new InvalidOperationException($"Unexpected type of bindable '{bindable.GetType()}'.");
            }

            bindable.BindingContext = viewModel;
        }
    }
}

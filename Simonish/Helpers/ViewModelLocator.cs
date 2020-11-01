using System;
using Rmjcs.Simonish.Pages;
using Rmjcs.Simonish.Services;
using Rmjcs.Simonish.ViewModels;
using Xamarin.Forms;

namespace Rmjcs.Simonish.Helpers
{
    /// <summary>
    /// A class used to assign ViewModels to pages and manage application singletons.
    /// </summary>
    /// <remarks>Originally based on <see href="https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Mobile/eShopOnContainers/eShopOnContainers.Core/ViewModels/Base/ViewModelLocator.cs"/>.
    /// This class originally used TinyIoC as the inversion of control container but this app doesn't warrant one - dependency injection is easily achieved without a 3rd party helper.</remarks>
    internal class ViewModelLocator
    {
        // These are the objects that there must only be one of in the app.
        // The classes do not use the singleton pattern because it's not very unit testable.
        private static readonly ResultsService ResultsService;
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
            ResultsService = new ResultsService(XamarinWrapper, FileHelper);
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

            object viewModel;

            if (bindable.GetType() == typeof(AboutPage))
            {
                viewModel = new AboutViewModel(XamarinWrapper);
            }
            else if (bindable.GetType() == typeof(GamePage))
            {
                ITimer timer = new OneSecondTimer();
                GameService gameService = new GameService(XamarinWrapper, FileHelper, timer, ResultsService);
                viewModel = new GameViewModel(XamarinWrapper, gameService);
            }
            else if (bindable.GetType() == typeof(ResultsPage))
            {
                viewModel = new ResultsViewModel(XamarinWrapper, ResultsService);
            }
            else
            {
                throw new InvalidOperationException($"Unexpected type of bindable '{bindable.GetType()}'.");
            }

            bindable.BindingContext = viewModel;

            if (bindable.GetType() == typeof(ResultsPage))
            {
                ResultsService.InitialiseListeners();
            }
        }

        /// <summary>
        /// Get the single app instance of the <see cref="ResultsService"/>.
        /// </summary>
        /// <returns></returns>
        public static ResultsService ResolveScoresService()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), null);

            return ResultsService;
        }

        /// <summary>
        /// Get the single app instance of the <see cref="IFileHelper"/>.
        /// </summary>
        /// <returns></returns>
        public static IFileHelper ResolveIFileHelper()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), null);

            return FileHelper;
        }
    }
}

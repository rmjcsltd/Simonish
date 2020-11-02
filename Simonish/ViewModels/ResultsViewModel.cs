using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;
using Rmjcs.Simonish.Services;

namespace Rmjcs.Simonish.ViewModels
{
    /// <summary>
    /// A ViewModel for the Scores page.
    /// </summary>
    internal class ResultsViewModel : INotifyPropertyChanged
    {
        private readonly IXamarinWrapper _xamarinWrapper;
        private readonly ResultsService _resultsService;

        private readonly Results _results;

        #region Construction & Initialisation

        public ResultsViewModel(IXamarinWrapper xamarinWrapper, ResultsService resultsService)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _xamarinWrapper = xamarinWrapper;
            _resultsService = resultsService;
            resultsService.ResultsChanged += OnResultsChanged;

            // Define the initial state for bound views.
            // No need to set via the properties because this view model has not been bound to the page yet.

            // ToDo: This isn't right, initial state should come from the ResultsService.

            _results = new Results();
        }

        #endregion

        public INewResultListener GetNewResultListener() => _resultsService;

        #region Binding properties

        public IEnumerable<Result> BestResults
        {
            get => _results.BestResults;
            set
            {
                if (!ReferenceEquals(value, _results.BestResults))
                {
                    _results.BestResults = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public IEnumerable<Result> LatestResults
        {
            get => _results.LatestResults;
            set
            {
                if (!ReferenceEquals(value, _results.LatestResults))
                {
                    _results.LatestResults = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Event Handlers

        private void OnResultsChanged(object sender, ResultsChangedEventArgs e)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Note: This ViewModel assumes events are raised on the UI thread.
            Debug.Assert(_xamarinWrapper.IsMainThread);

            if (e.ResultTypeChanged.HasFlag(ResultTypeChanged.Best))
            {
                BestResults = e.Results.BestResults;
            }

            if (e.ResultTypeChanged.HasFlag(ResultTypeChanged.Latest))
            {
                LatestResults = e.Results.LatestResults;
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

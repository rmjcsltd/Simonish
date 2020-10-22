using System;
using System.Collections.Generic;
using System.Threading;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;

namespace Rmjcs.Simonish.Services
{
    /// <summary>
    /// A class to manage the best and latest game results.
    /// </summary>
    /// <remarks>A single instance of this class is created by the ViewModelLocator.</remarks>
    internal class ResultsService
    {
        private readonly SynchronizationContext _synchronisationContext;
        private readonly IFileHelper _fileHelper;
        private readonly Results _results;

        public ResultsService(IXamarinWrapper xamarinWrapper, IFileHelper fileHelper)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            _synchronisationContext = xamarinWrapper.MainSynchronizationContext;
            _fileHelper = fileHelper;

            _results = new Results();
        }

        #region Public 

        /// <summary>
        /// Load Results from file into the BestResults and LatestResults lists.
        /// </summary>
        public void LoadResults()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            // This is called via Task.Run.

            Results results = _fileHelper.LoadResults();

            // These results have *probably* been loaded before any played games,
            // but just in case we merge them in individually.

            bool bestChanged = false;
            List<Result> bestResults = new List<Result>(_results.BestResults);
            foreach (Result result in results.BestResults)
            {
                bestChanged = MergeBestResult(result, bestResults) || bestChanged;
            }
            if (bestChanged) _results.BestResults = bestResults;

            bool latestChanged = false;
            List<Result> latestResults = new List<Result>(_results.LatestResults);
            foreach (Result result in results.LatestResults)
            {
                latestChanged = MergeLatestResult(result, latestResults, false) || latestChanged;
            }
            if (latestChanged) _results.LatestResults = latestResults;

            OnResultsChanged(bestChanged, latestChanged);
        }

        /// <summary>
        /// Merge a new Result into the BestResults and LatestResults lists.
        /// </summary>
        /// <param name="result">The Result to merge.</param>
        /// <remarks>A 'new' Result is one from game play, not loaded from file.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is <see langword="null"/></exception>
        public void MergeNewGameResult(Result result)
        {
            // This is called via Task.Run.

            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            List<Result> bestResults = new List<Result>(_results.BestResults);
            bool bestChanged = MergeBestResult(result, bestResults);
            if (bestChanged) _results.BestResults = bestResults;

            List<Result> latestResults = new List<Result>(_results.LatestResults);
            bool latestChanged = MergeLatestResult(result, latestResults, true);
            if (latestChanged) _results.LatestResults = latestResults;

            OnResultsChanged(bestChanged, latestChanged);

            SaveResults();
        }

        #endregion

        /// <summary>
        /// Save the current BestResults and LatestResults lists to file.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private void SaveResults()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            Results results = new Results(_results);

            // Note: SaveResults will run on a background thread and enumerate the lists it receives so
            // we can not pass BestResults and LatestResults because they might be modified on the UI thread.

            _fileHelper.SaveResults(results);
        }

        /// <summary>
        /// Merge a Result into the BestResults list in the right place.
        /// </summary>
        /// <param name="newResult">The new Result to merge in.</param>
        /// <param name="bestResults">The current list of Results into which to merge the new Result.</param>
        /// <remarks>Higher scores should appear before lower scores.
        /// Earlier occurrences of a score should appear before later occurrences of the same score.</remarks>
        private static bool MergeBestResult(Result newResult, List<Result> bestResults)
        {
            bool modified = false;
            for (int i = 0; i < bestResults.Count; i++)
            {
                if ((newResult.Score == bestResults[i].Score && newResult.StartTimeUtc < bestResults[i].StartTimeUtc) ||
                    newResult.Score > bestResults[i].Score)
                {
                    bestResults.Insert(i, newResult);
                    modified = true;
                    break;
                }
            }

            if (!modified && bestResults.Count < Constants.MaxBestResults)
            {
                bestResults.Add(newResult);
                modified = true;
            }

            if (bestResults.Count > Constants.MaxBestResults)
            {
                bestResults.RemoveAt(Constants.MaxBestResults);
                modified = true;
            }

            return modified;
        }

        /// <summary>
        /// Merge a Result into the LatestResults list in the right place.
        /// </summary>
        /// <param name="newResult">The new Result to merge in.</param>
        /// <param name="latestResults">The current list of Results into which to merge the new Result.</param>
        /// <param name="isNew">true if this is a new Result from game play, false otherwise.</param>
        /// <remarks>New results are inserted at the start of the list, old (not New) results are added to the end of the list.</remarks>
        private static bool MergeLatestResult(Result newResult, List<Result> latestResults, bool isNew)
        {
            // Note: StartTimeUtc is not considered here, just New or not New.

            bool modified = false;
            if (isNew)
            {
                latestResults.Insert(0, newResult);
                modified = true;
                if (latestResults.Count > Constants.MaxLatestResults) latestResults.RemoveAt(Constants.MaxLatestResults);
            }
            else
            {
                if (latestResults.Count < Constants.MaxLatestResults)
                {
                    latestResults.Add(newResult);
                    modified = true;
                }
            }

            return modified;
        }

        #region Events

        public event EventHandler<ResultsChangedEventArgs> ResultsChanged;

        private void OnResultsChanged(bool bestChanged, bool latestChanged)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            // Don't raise an event if nothing has changed.
            if (!(bestChanged || latestChanged)) return;

            ResultTypeChanged resultTypeChanged = ResultTypeChanged.Void;
            if (bestChanged) resultTypeChanged |= ResultTypeChanged.Best;
            if (latestChanged) resultTypeChanged |= ResultTypeChanged.Latest;

            // Create a copy of the Results instance so we don't expose this private instance.
            Results results = new Results(_results);

            ResultsChangedEventArgs args = new ResultsChangedEventArgs { ResultTypeChanged = resultTypeChanged, Results = results };
            EventHandler<ResultsChangedEventArgs> handler = ResultsChanged;
            if (handler != null)
            {
                // Raise events on the supplied sync context.
                _synchronisationContext.Post(o => handler.Invoke(this, args), null);
            }
        }

        #endregion
    }

    #region Custom EventArgs

    [Flags]
    internal enum ResultTypeChanged { Void = 0, Best = 1, Latest = 2 }

    internal class ResultsChangedEventArgs : EventArgs
    {
        public ResultTypeChanged ResultTypeChanged { get; set; }
        public Results Results { get; set; }
    }

    #endregion
}

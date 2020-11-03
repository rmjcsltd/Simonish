using System;
using System.Collections.Generic;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;

namespace Rmjcs.Simonish.Services
{
    /// <summary>
    /// A class to manage the best and latest game results.
    /// </summary>
    /// <remarks>A single instance of this class is created by the ViewModelLocator.</remarks>
    internal class ResultsService : INewResultListener
    {
        private readonly IFileHelper _fileHelper;
        private readonly Results _results;

        public ResultsService(IFileHelper fileHelper)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _fileHelper = fileHelper;

            _results = new Results();
        }

        #region Public

        /// <summary>
        /// Load Results from file into the BestResults and LatestResults lists.
        /// </summary>
        public void LoadResults()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            Results results;

            try
            {
                string text = _fileHelper.ReadResultsFile();
                results = new Results(text);
            }
            catch (Exception e)
            {
                _fileHelper.LogException(e);
                return;
            }

            // These results have been loaded before any played games, but we merge them in
            // individually to ensure correct order and number of results regardless of what was
            // read from file.

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

        public void NewResultSourceNewResult(object sender, NewResultEventArgs e)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            MergeNewGameResult(e.Result);
        }

        /// <summary>
        /// Merge a new Result into the BestResults and LatestResults lists.
        /// </summary>
        /// <param name="result">The Result to merge.</param>
        /// <remarks>A 'new' Result is one from game play, not loaded from file.</remarks>
        /// <exception cref="ArgumentNullException"><paramref name="result"/> is <see langword="null"/></exception>
        internal void MergeNewGameResult(Result result)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

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
        private void SaveResults()
        {
            try
            {
                string text = _results.ToString();
                _fileHelper.WriteResultsFile(text);
            }
            catch (Exception e)
            {
                _fileHelper.LogException(e);
            }
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
            // Don't raise an event if nothing has changed.
            if (!(bestChanged || latestChanged)) return;

            ResultTypeChanged resultTypeChanged = ResultTypeChanged.Void;
            if (bestChanged) resultTypeChanged |= ResultTypeChanged.Best;
            if (latestChanged) resultTypeChanged |= ResultTypeChanged.Latest;

            // Create a copy of _results so we don't expose this private data.
            ResultsChangedEventArgs args = new ResultsChangedEventArgs { ResultTypeChanged = resultTypeChanged, Results = new Results(_results) };
            EventHandler<ResultsChangedEventArgs> handler = ResultsChanged;
            handler?.Invoke(this, args);
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

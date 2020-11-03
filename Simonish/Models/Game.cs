using System;
using System.ComponentModel;
using Rmjcs.Simonish.Helpers;

namespace Rmjcs.Simonish.Models
{
    /// <summary>
    /// Provides methods to manage the state of the game.
    /// </summary>
    internal class Game
    {
        private readonly Random _randomNumberGenerator;
        private readonly int _durationMilliseconds;
        private DateTime _startTimeUtc;
        private DateTime _endTimeUtc;
        private int _countdown;
        private int _currentTargetIndex;
        private int _correctHits;
        private int _incorrectHits;

        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _randomNumberGenerator = new Random();

            _durationMilliseconds = Constants.PlaySeconds * 1000;
            Phase = GamePhase.Launched;
        }

        /// <summary>
        /// Move the game phase to Countdown.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>The new game phase.</returns>
        public void StartCountdown()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (!(Phase == GamePhase.Launched || Phase == GamePhase.GameOver))
            {
                throw new InvalidOperationException("Unexpected current game phase for this method.");
            }

            Phase = GamePhase.Countdown;
            _countdown = Constants.CountdownSteps;
            _correctHits = 0;
            _incorrectHits = 0;
        }

        /// <summary>
        /// Decrement the countdown counter.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>The remaining countdown steps.</returns>
        public int DecrementCountdown()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (Phase != GamePhase.Countdown)
            {
                throw new InvalidOperationException("Unexpected current game phase for this method.");
            }

            if (_countdown <= 0)
            {
                throw new InvalidOperationException("Countdown has already completed.");
            }

            _countdown--;
            return _countdown;
        }

        /// <summary>
        /// Move the game phase to Playing.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>When this method returns, NewGamePhase is the new game phase, 
        /// NewTargetIndex is the new target index.</returns>
        public int StartPlay()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (Phase != GamePhase.Countdown)
            {
                throw new InvalidOperationException("Unexpected current game phase for this method.");
            }

            if (_countdown != 0)
            {
                throw new InvalidOperationException("Incomplete countdown for this method.");
            }

            Phase = GamePhase.Playing;
            _currentTargetIndex = _randomNumberGenerator.Next(Constants.TargetCount);
            _startTimeUtc = DateTime.UtcNow;
            _endTimeUtc = _startTimeUtc.AddMilliseconds(_durationMilliseconds);

            return _currentTargetIndex;
        }

        /// <summary>
        /// Record a hit on a target.
        /// </summary>
        /// <param name="targetIndex">The index of the target that was hit.</param>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>When this method returns, Success is true if the hit was recorded, false otherwise, 
        /// Score is the game score if the hit was recorded, zero otherwise,
        /// NewTargetIndex is the new target index if the hit was recorded, zero otherwise.</returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="targetIndex"/> is out of range.</exception>
        public (bool Success, int Score, int NewTargetIndex) RecordHit(int targetIndex)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // If this method is called when the phase is GameOver then GameService events are not being processed in the expected order.

            if (Phase != GamePhase.Playing)
            {
                throw new InvalidOperationException("Unexpected current game phase for this method.");
            }

            if (targetIndex < 0 || targetIndex > Constants.TargetCount - 1)
            {
                throw new ArgumentOutOfRangeException(nameof(targetIndex));
            }

            // Any hits outside the game duration should be ignored.
            // This *might* happen on slower/loaded systems when event processing is delayed.


            if (TimeLeft > 0)
            {
                if (targetIndex == _currentTargetIndex)
                {
                    _correctHits++;
                }
                else
                {
                    _incorrectHits++;
                }

                _currentTargetIndex = _randomNumberGenerator.Next(Constants.TargetCount);

                int score = _correctHits - _incorrectHits;
                return (true, score, _currentTargetIndex);
            }
            else
            {
                return (false, 0, 0);
            }
        }

        /// <summary>
        /// Move the game phase to GameOver.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>This method returns the new game phase.</returns>
        public void EndPlay()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (Phase != GamePhase.Playing)
            {
                throw new InvalidOperationException("Unexpected current game phase for this method.");
            }

            Phase = GamePhase.GameOver;
        }

        /// <summary>
        /// Create a new <see cref="Result"/> from the current state of this game.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>A new <see cref="Result"/>.</returns>
        public Result GetResult()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (Phase != GamePhase.GameOver)
            {
                throw new InvalidOperationException("Unexpected current game phase for this method.");
            }

            return new Result(_startTimeUtc, _correctHits, _incorrectHits);
        }

        /// <summary>
        /// Gets the current phase of the game.
        /// </summary>
        public GamePhase Phase { get; private set; }

        /// <summary>
        /// Gets the proportion of the game duration remaining.
        /// </summary>
        public double TimeLeft
        {
            get
            {
                switch (Phase)
                {
                    case GamePhase.Launched:
                    case GamePhase.Countdown:
                        return 1;
                    case GamePhase.Playing:
                        double timeLeft = (_endTimeUtc - DateTime.UtcNow).TotalMilliseconds;
                        return timeLeft > 0 ? timeLeft / _durationMilliseconds : 0;
                    case GamePhase.GameOver:
                        return 0;
                    default:
                        // No other values for the phase are expected (the switch should cover all possibilities).
                        throw new InvalidEnumArgumentException(nameof(Phase), (int)Phase, typeof(GamePhase));
                }
            }
        }
    }

    /// <summary>
    /// Specifies the phase the game is currently in.
    /// </summary>
    public enum GamePhase
    {
        /// <summary>
        /// The app has been launched but no game has been played yet.
        /// </summary>
        Launched = 0,
        /// <summary>
        /// The pre-play countdown is in progress.
        /// </summary>
        Countdown,
        /// <summary>
        /// The game is being played.
        /// </summary>
        Playing,
        /// <summary>
        /// The game is over.
        /// </summary>
        GameOver
    }

}

using System;
using System.ComponentModel;
using System.Threading;
using Rmjcs.Simonish.Helpers;
// ReSharper disable ExceptionNotDocumented

namespace Rmjcs.Simonish.Models
{
    /// <summary>
    /// Provides methods to manage the state of the game.
    /// </summary>
    /// <remarks>During normal game play this object can be accessed from both the main thread and timer threads.
    /// It's unlikely to be accessed by different threads concurrently but if event processing is very delayed
    /// it could happen so all access is controlled by the _readerWriterLocker.</remarks>
    internal class Game : IDisposable
    {
        private readonly ReaderWriterLockSlim _readerWriterLocker;
        private readonly Random _randomNumberGenerator;
        private readonly int _durationMilliseconds;
        private DateTime _startTimeUtc;
        private DateTime _endTimeUtc;
        private int _countdown;
        private int _currentTargetIndex;
        private int _correctHits;
        private int _incorrectHits;
        private GamePhase _phase;

        /// <summary>
        /// Initialises a new instance of the <see cref="Game"/> class.
        /// </summary>
        public Game()
        {
            _readerWriterLocker = new ReaderWriterLockSlim();
            _randomNumberGenerator = new Random();

            _durationMilliseconds = Constants.PlaySeconds * 1000;
            _phase = GamePhase.Launched;
        }

        /// <summary>
        /// Move the game phase to Countdown.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>The new game phase.</returns>
        public void StartCountdown()
        {
            _readerWriterLocker.EnterWriteLock();

            try
            {
                if (!(_phase == GamePhase.Launched || _phase == GamePhase.GameOver))
                {
                    throw new InvalidOperationException("Unexpected current game phase for this method.");
                }

                _phase = GamePhase.Countdown;
                _countdown = Constants.CountdownSteps;
                _correctHits = 0;
                _incorrectHits = 0;
            }
            finally
            {
                _readerWriterLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Decrement the countdown counter.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>The remaining countdown steps.</returns>
        public int DecrementCountdown()
        {
            _readerWriterLocker.EnterWriteLock();

            try
            {
                if (_phase != GamePhase.Countdown)
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
            finally
            {
                _readerWriterLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Move the game phase to Playing.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>When this method returns, NewGamePhase is the new game phase, 
        /// NewTargetIndex is the new target index.</returns>
        public int StartPlay()
        {
            _readerWriterLocker.EnterWriteLock();

            try
            {
                if (_phase != GamePhase.Countdown)
                {
                    throw new InvalidOperationException("Unexpected current game phase for this method.");
                }

                if (_countdown != 0)
                {
                    throw new InvalidOperationException("Incomplete countdown for this method.");
                }

                _phase = GamePhase.Playing;
                _currentTargetIndex = _randomNumberGenerator.Next(Constants.TargetCount);
                _startTimeUtc = DateTime.UtcNow;
                _endTimeUtc = _startTimeUtc.AddMilliseconds(_durationMilliseconds);

                return _currentTargetIndex;
            }
            finally
            {
                _readerWriterLocker.ExitWriteLock();
            }
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
            _readerWriterLocker.EnterWriteLock();

            try
            {
                // If this method is called when the phase is GameOver then GameService events are not being processed in the expected order.

                if (_phase != GamePhase.Playing)
                {
                    throw new InvalidOperationException("Unexpected current game phase for this method.");
                }

                if (targetIndex < 0 || targetIndex > Constants.TargetCount - 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(targetIndex));
                }

                // Any hits outside the game duration should be ignored.
                // This *might* happen on slower/loaded systems when event processing is delayed.


                if (GetTimeLeft() > 0)
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
            finally
            {
                _readerWriterLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Move the game phase to GameOver.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>This method returns the new game phase.</returns>
        public void EndPlay()
        {
            _readerWriterLocker.EnterWriteLock();

            try
            {
                if (_phase != GamePhase.Playing)
                {
                    throw new InvalidOperationException("Unexpected current game phase for this method.");
                }

                _phase = GamePhase.GameOver;
            }
            finally
            {
                _readerWriterLocker.ExitWriteLock();
            }
        }

        /// <summary>
        /// Create a new <see cref="Result"/> from the current state of this game.
        /// </summary>
        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        /// <returns>A new <see cref="Result"/>.</returns>
        public Result GetResult()
        {
            _readerWriterLocker.EnterReadLock();

            try
            {
                if (_phase != GamePhase.GameOver)
                {
                    throw new InvalidOperationException("Unexpected current game phase for this method.");
                }

                return new Result(_startTimeUtc, _correctHits, _incorrectHits);
            }
            finally
            {
                _readerWriterLocker.ExitReadLock();
            }
        }

        /// <summary>
        /// Gets the current phase of the game.
        /// </summary>
        public GamePhase Phase
        {
            get
            {
                _readerWriterLocker.EnterReadLock();
                try
                {
                    return _phase;
                }
                finally
                {
                    _readerWriterLocker.ExitReadLock();
                }
            }
        }

        /// <summary>
        /// Gets the proportion of the game duration remaining.
        /// </summary>
        public double TimeLeft
        {
            get
            {
                _readerWriterLocker.EnterReadLock();
                try
                {
                    return GetTimeLeft();
                }
                finally
                {
                    _readerWriterLocker.ExitReadLock();
                }
            }
        }

        /// <exception cref="InvalidOperationException">The current state of this <see cref="Game"/> is not valid for this method.</exception>
        private double GetTimeLeft()
        {
            // ToDo: Should this be a property or a method?

            // This private method should only be used inside a read or write lock.
            if (!(_readerWriterLocker.IsReadLockHeld || _readerWriterLocker.IsWriteLockHeld))
            {
                throw new InvalidOperationException("No read or write lock found.");
            }

            switch (_phase)
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
                    throw new InvalidEnumArgumentException(nameof(_phase), (int)_phase, typeof(GamePhase));
            }
        }

        #region IDisposable

        /// <summary>
        /// Releases all resources used by the current <see cref="Game"/>.
        /// </summary>
        public void Dispose()
        {
            _readerWriterLocker?.Dispose();
        }

        #endregion

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

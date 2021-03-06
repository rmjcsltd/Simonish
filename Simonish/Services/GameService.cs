﻿using System;
using System.ComponentModel;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;

namespace Rmjcs.Simonish.Services
{
    /// <summary>
    /// A class to manage playing the game.
    /// </summary>
    internal class GameService : INewResultSource
    {
        private readonly IXamarinWrapper _xamarinWrapper;
        private readonly ITimer _timer;
        private readonly Game _game;

        public GameService(IXamarinWrapper xamarinWrapper, ITimer timer)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _xamarinWrapper = xamarinWrapper;
            _game = new Game();
            _timer = timer;
            _timer.SetAction(OnTimer);
        }

        #region Public methods

        public void StartCountdown()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _game.StartCountdown();

            _timer.Start();
        }

        public int StartPlaying()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            int newTargetIndex = _game.StartPlay();
            return newTargetIndex;
        }

        public (bool Success, int Score, int NewTargetIndex) Hit(int targetIndex)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            return _game.RecordHit(targetIndex);
        }

        public void EndPlay()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _game.EndPlay();
            Result result = _game.GetResult();

            OnNewResult(result);
        }

        #endregion

        #region Events

        public event EventHandler<PhaseChangeRequiredEventArgs> PhaseChangeRequired;

        public event EventHandler<CountdownEventArgs> CountdownTimer;

        public event EventHandler<PlayEventArgs> PlayTimer;

        public event EventHandler<NewResultEventArgs> NewResult;

        private void OnNewResult(Result result)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            NewResultEventArgs args = new NewResultEventArgs { Result = result };
            EventHandler<NewResultEventArgs> handler = NewResult;
            handler?.Invoke(this, args);
        }

        #endregion

        #region Event handler

        private void OnTimer()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (!_xamarinWrapper.IsMainThread)
            {
                throw new InvalidOperationException("This method expects to be on the main UI thread.");
            }

            // The timer has AutoReset = false so it must be re-started each time as necessary. 
            // This is done to prevent timer events backing up on very slow/busy machines.
            // The downside of this is that the 1 second timer interval might be longer than 1 second
            // once event queuing and processing have occurred. For the countdown it is not a significant issue.
            // In the RecordHit method there is a check that the hit occurred within the allowed game time window.

            GamePhase phase = _game.Phase;

            switch (phase)
            {
                case GamePhase.Countdown:
                    int countdownRemaining = _game.DecrementCountdown();
                    if (countdownRemaining > 0)
                    {
                        OnCountdownTimer(countdownRemaining);
                    }
                    else
                    {
                        OnPhaseChangeRequired(GamePhase.Playing);
                    }
                    _timer.Start();

                    break;
                case GamePhase.Playing:
                    double gameProportionLeft = _game.TimeLeft;
                    if (gameProportionLeft > 0)
                    {
                        OnPlayTimer(gameProportionLeft);
                        _timer.Start();
                    }
                    else
                    {
                        OnPhaseChangeRequired(GamePhase.GameOver);
                    }

                    break;
                default:
                    // No other values for the phase are expected.
                    throw new InvalidEnumArgumentException(nameof(_game.Phase), (int)phase, typeof(GamePhase));
            }

            void OnPhaseChangeRequired(GamePhase newGamePhase)
            {
                PhaseChangeRequiredEventArgs args = new PhaseChangeRequiredEventArgs { NewGamePhase = newGamePhase };
                EventHandler<PhaseChangeRequiredEventArgs> handler = PhaseChangeRequired;
                handler?.Invoke(this, args);
            }

            void OnCountdownTimer(int countdown)
            {
                CountdownEventArgs args = new CountdownEventArgs { Countdown = countdown };
                EventHandler<CountdownEventArgs> handler = CountdownTimer;
                handler?.Invoke(this, args);
            }

            void OnPlayTimer(double proportionLeft)
            {
                PlayEventArgs args = new PlayEventArgs { TimeLeft = proportionLeft };
                EventHandler<PlayEventArgs> handler = PlayTimer;
                handler?.Invoke(this, args);
            }

        }

        #endregion
    }

    #region Custom EventArgs

    internal class PhaseChangeRequiredEventArgs : EventArgs
    {
        public GamePhase NewGamePhase { get; set; }
    }

    internal class CountdownEventArgs : EventArgs
    {
        public int Countdown { get; set; }
    }

    internal class PlayEventArgs : EventArgs
    {
        // ToDo: TimeLeft ought to be a TimeSpan, it's only a XAML requirement that it be a fraction.
        public double TimeLeft { get; set; }
    }

    #endregion
}

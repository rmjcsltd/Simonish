using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Rmjcs.Simonish.Helpers;
using Rmjcs.Simonish.Models;
using Rmjcs.Simonish.Services;
using Xamarin.Forms;

namespace Rmjcs.Simonish.ViewModels
{
    /// <summary>
    /// A ViewModel for the Play page.
    /// </summary>
    internal class GameViewModel : INotifyPropertyChanged, IDisposable
    {
        // Create a constant colour array for each button's unlit/lit colour [button index, Unlit|Lit].
        // An array initialiser is not a compile time constant so we can't use the const keyword.
#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional
        private static readonly Color[,] Colours = {
            {Color.FromRgb(0, 128, 0), Color.FromRgb(0, 255, 0)},
            {Color.FromRgb(128, 0, 0), Color.FromRgb(255, 0, 0)},
            {Color.FromRgb(128, 128, 0), Color.FromRgb(255, 255, 0)},
            {Color.FromRgb(0, 0, 128), Color.FromRgb(0, 0, 255)},
        };
#pragma warning restore CA1814 // Prefer jagged arrays over multidimensional

        #region Binding property backing fields

        private bool _overlayIsVisible;
        private string _overlayText;
        private int _score;
        private double _timeLeft;

        #endregion

        // ToDo: Should this ViewModel track the canStart & canHit state or should the GameService expose them as properties?
        // With this approach (ViewModel tracks state) the ViewModel has no direct coupling to the GameService, it's just an event listener.
        private bool _canStart;
        private bool _canHit;

        private readonly IXamarinWrapper _xamarinWrapper;
        private readonly GameService _gameService;

        #region Construction & Initialisation

        public GameViewModel(IXamarinWrapper xamarinWrapper, GameService gameService)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            StartCommand = new Command(Start, CanExecuteStart);
            HitCommand = new Command<int>(Hit, CanExecuteHit);

            // Define the initial state for bound views.
            // No need to set via the properties because this view model has not been bound to the page yet.

            // ToDo: This isn't right, initial state should come from the GameService. Raise launched phase changed event?

            _canStart = true;
            _canHit = false;
            _score = 0;
            _timeLeft = 1;
            _overlayText = "Simon\n(ish)";
            _overlayIsVisible = true;

            ButtonColours = new[] { Colours[0, 0], Colours[1, 0], Colours[2, 0], Colours[3, 0] };

            _xamarinWrapper = xamarinWrapper;
            _gameService = gameService;
            _gameService.PhaseChangeRequired += GameServicePhaseChangeRequired;
            _gameService.CountdownTimer += GameServiceCountdownTimer;
            _gameService.PlayTimer += GameServicePlayTimer;
        }

        #endregion

        public INewResultSource GetNewResultSource() => _gameService;

        #region Commands

        public ICommand StartCommand { get; }

        private void Start()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Previous phase was GamePhase.Launched or GamePhase.GameOver.
            _canStart = false;
            ((Command)StartCommand).ChangeCanExecute();
            TimeLeft = 1;
            Score = 0;
            OverlayText = Constants.CountdownSteps.ToString("0", CultureInfo.CurrentCulture);
            OverlayIsVisible = true;

            _gameService.StartCountdown();
        }

        private bool CanExecuteStart()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            return _canStart;
        }

        public ICommand HitCommand { get; }

        private void Hit(int index)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            (bool Success, int Score, int NewTargetIndex) result = _gameService.Hit(index);

            if (result.Success)
            {
                Score = result.Score;
                LightNewTarget(result.NewTargetIndex);
            }
        }

        private bool CanExecuteHit(int _)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            return _canHit;
        }

        #endregion

        #region Binding properties

        public Color[] ButtonColours { get; }

        public bool OverlayIsVisible
        {
            get => _overlayIsVisible;
            private set
            {
                if (value != _overlayIsVisible)
                {
                    _overlayIsVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string OverlayText
        {
            get => _overlayText;
            private set
            {
                if (value != _overlayText)
                {
                    _overlayText = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public int Score
        {
            get => _score;
            private set
            {
                if (value != _score)
                {
                    _score = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double TimeLeft
        {
            get => _timeLeft;
            private set
            {
                // There are known issues when comparing floating point values but for our purposes this inequality check is fine.
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (value != _timeLeft)
                {
                    _timeLeft = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion

        #region Event Handlers

        private void GameServicePhaseChangeRequired(object sender, PhaseChangeRequiredEventArgs args)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Note: This ViewModel assumes events are raised on the UI thread.
            Debug.Assert(_xamarinWrapper.IsMainThread);

            switch (args.NewGamePhase)
            {
                case GamePhase.Playing:
                    // Previous phase was GamePhase.Countdown.
                    _canHit = true;
                    ((Command)HitCommand).ChangeCanExecute();
                    OverlayIsVisible = false;
                    int newTargetIndex = _gameService.StartPlaying();
                    LightNewTarget(newTargetIndex);
                    break;
                case GamePhase.GameOver:
                    // Previous phase was GamePhase.Playing.
                    _canHit = false;
                    ((Command)HitCommand).ChangeCanExecute();
                    TimeLeft = 0; // There is not a PlayTimer for time up so zero the TimeLeft here.
                    OverlayText = "Game\nOver";
                    OverlayIsVisible = true;
                    _gameService.EndPlay();
                    LightNewTarget(-1); // Un-light all buttons for neatness.
                    _canStart = true;
                    ((Command)StartCommand).ChangeCanExecute();
                    break;
                default:
                    // No other values for the phase are expected here.
                    throw new InvalidEnumArgumentException(nameof(args.NewGamePhase), (int)args.NewGamePhase, typeof(GamePhase));
            }
        }

        private void GameServiceCountdownTimer(object sender, CountdownEventArgs e)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Note: This ViewModel assumes events are raised on the UI thread.
            Debug.Assert(_xamarinWrapper.IsMainThread);

            OverlayText = e.Countdown.ToString("0", CultureInfo.CurrentCulture);
        }

        private void GameServicePlayTimer(object sender, PlayEventArgs e)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            // Note: This ViewModel assumes events are raised on the UI thread.
            Debug.Assert(_xamarinWrapper.IsMainThread);

            TimeLeft = e.TimeLeft;
        }

        #endregion

        private void LightNewTarget(int newTargetIndex)
        {
            for (int i = 0; i < ButtonColours.Length; i++)
            {
                ButtonColours[i] = i == newTargetIndex ? Colours[i, 1] : Colours[i, 0];
            }
            NotifyPropertyChanged(nameof(ButtonColours));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region IDisposable

        /// <summary>
        /// Releases all resources used by the current <see cref="GameViewModel"/>.
        /// </summary>
        public void Dispose()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            _gameService?.Dispose();
        }

        #endregion
    }
}

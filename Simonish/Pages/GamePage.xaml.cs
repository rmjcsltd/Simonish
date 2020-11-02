using System;
using Rmjcs.Simonish.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rmjcs.Simonish.Pages
{
    /// <summary>
    /// Represents the Play tab for the application.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public sealed partial class GamePage : ContentPage, IDisposable
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GamePage"/> class.
        /// </summary>
        public GamePage()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            InitializeComponent();

            // XAML does not support expressions so this is done in code behind.
            OverlayLabel.FontSize = TimeLabel.FontSize * 7;
        }

        #region IDisposable

        /// <summary>
        /// Releases all resources used by the current <see cref="OneSecondTimer"/>.
        /// </summary>
        public void Dispose()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            if (BindingContext is IDisposable bindingContext) bindingContext.Dispose();
        }

        #endregion
    }
}
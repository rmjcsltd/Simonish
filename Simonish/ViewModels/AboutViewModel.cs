﻿using System.Globalization;
using System.Windows.Input;
using Rmjcs.Simonish.Helpers;
using Xamarin.Forms;

namespace Rmjcs.Simonish.ViewModels
{
    /// <summary>
    /// A ViewModel for the About page.
    /// </summary>
    internal class AboutViewModel
    {
        private readonly IXamarinWrapper _xamarinWrapper;

        public AboutViewModel(IXamarinWrapper xamarinWrapper)
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());

            _xamarinWrapper = xamarinWrapper;
        }

        public ICommand ShowWebPageCommand => new Command(ShowWebPage);

        private void ShowWebPage()
        {
            _xamarinWrapper.ShowWebPage(Constants.PrivacyPolicyUrl);
        }

        // Although these are effectively static properties if they are defined as static then binding will not work.

#pragma warning disable CA1822 // Mark members as static

        public string GameDurationText => Constants.PlaySeconds.ToString("0", CultureInfo.CurrentCulture);

        public string BuildVersionText => typeof(AboutViewModel).Assembly.GetName().Version.ToString();

        // Because of the way the build date is stored it will be in the build locale, it is not localised.
        // ToDo: Localise the build date format.
        public string BuildDateText => Properties.Resources.BuildDate.Trim();

#pragma warning restore CA1822 // Mark members as static
    }
}

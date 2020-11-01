using Rmjcs.Simonish.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rmjcs.Simonish.Pages
{
    /// <summary>
    /// Represents the About tab for the application.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AboutPage : ContentPage
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="AboutPage"/> class.
        /// </summary>
        public AboutPage()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod(), this);

            InitializeComponent();
        }
    }
}
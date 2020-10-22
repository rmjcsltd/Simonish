using Rmjcs.Simonish.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rmjcs.Simonish.Pages
{
    /// <summary>
    /// Represents the Play tab for the application.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GamePage : ContentPage
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="GamePage"/> class.
        /// </summary>
        public GamePage()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());
            
            InitializeComponent();

            // XAML does not support expressions so this is done in code behind.
            OverlayLabel.FontSize = TimeLabel.FontSize * 7;
        }
    }
}
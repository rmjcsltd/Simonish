using Rmjcs.Simonish.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rmjcs.Simonish.Pages
{
    /// <summary>
    /// Represents the main page for the application.
    /// </summary>
    /// <remarks>This page acts as the tab container for the other pages.</remarks>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : TabbedPage
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());
            
            InitializeComponent();
        }
    }
}
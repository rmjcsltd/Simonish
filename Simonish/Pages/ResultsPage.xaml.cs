using Rmjcs.Simonish.Helpers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Rmjcs.Simonish.Pages
{
    /// <summary>
    /// Represents the Scores tab for the application.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultsPage : ContentPage
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ResultsPage"/> class.
        /// </summary>
        public ResultsPage()
        {
            Utility.WriteDebugEntryMessage(System.Reflection.MethodBase.GetCurrentMethod());
            
            InitializeComponent();
        }
    }
}
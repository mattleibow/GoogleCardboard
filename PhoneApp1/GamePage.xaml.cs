using System.Windows.Navigation;

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using MonoGame.Framework.WindowsPhone;

using VirtualReality;

namespace PhoneApp1
{
    public partial class GamePage : PhoneApplicationPage
    {
		private CardboardMonkeyGame game;

        public GamePage()
        {
            InitializeComponent();

            game = XamlGame<CardboardMonkeyGame>.Create("", this);
        }

	    protected override void OnNavigatedTo(NavigationEventArgs e)
	    {
			PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

		    base.OnNavigatedTo(e);
	    }
    }
}
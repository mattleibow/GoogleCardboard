using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WindowsFormsApplication;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework;
using MonoGame.Framework.WindowsPhone;
using PhoneApp1.Resources;
using PhoneClassLibrary1;

namespace PhoneApp1
{
    public partial class GamePage : PhoneApplicationPage
    {
        private Game1 _game;
	    private MotionSensor motion;

	    // Constructor
        public GamePage()
        {
            InitializeComponent();

            _game = XamlGame<Game1>.Create("", this);

            // Sample code to localize the ApplicationBar
            //BuildLocalizedApplicationBar();

        }

	    protected override void OnNavigatedTo(NavigationEventArgs e)
	    {
			PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;

			// If the Motion object is null, initialize it and add a CurrentValueChanged
			// event handler.
			if (motion == null)
			{
				motion = new MotionSensor();
				motion.CurrentValueChanged += (sender, args) =>
				{
					if (_game.camera != null)
					{
						var m = args.RotationMatrix;
						Matrix matrix = new Matrix(
							m[0], m[1], m[2], m[3],
							m[4], m[5], m[6], m[7],
							m[8], m[9], m[10], m[11],
							m[12], m[13], m[14], m[15]);

						_game.camera.AttitudeMatrix = Matrix.CreateRotationX(MathHelper.PiOver2)*matrix;
					}
				};
			}

		    base.OnNavigatedTo(e);

			UpdateOrientation();
	    }

	    protected override void OnOrientationChanged(OrientationChangedEventArgs e)
	    {
		    base.OnOrientationChanged(e);

		    UpdateOrientation();
	    }

		PageOrientation orient = PageOrientation.Portrait;
		private void UpdateOrientation()
		{
			if (Orientation == PageOrientation.Landscape || Orientation == PageOrientation.LandscapeLeft || Orientation == PageOrientation.LandscapeRight)
			{
				orient = PageOrientation.Landscape;
			}
			else
			{
				orient = PageOrientation.Portrait;
			}
	    }

	    // Sample code for building a localized ApplicationBar
        //private void BuildLocalizedApplicationBar()
        //{
        //    // Set the page's ApplicationBar to a new instance of ApplicationBar.
        //    ApplicationBar = new ApplicationBar();

        //    // Create a new button and set the text value to the localized string from AppResources.
        //    ApplicationBarIconButton appBarButton = new ApplicationBarIconButton(new Uri("/Assets/AppBar/appbar.add.rest.png", UriKind.Relative));
        //    appBarButton.Text = AppResources.AppBarButtonText;
        //    ApplicationBar.Buttons.Add(appBarButton);

        //    // Create a new menu item with the localized string from AppResources.
        //    ApplicationBarMenuItem appBarMenuItem = new ApplicationBarMenuItem(AppResources.AppBarMenuItemText);
        //    ApplicationBar.MenuItems.Add(appBarMenuItem);
        //}
    }
}
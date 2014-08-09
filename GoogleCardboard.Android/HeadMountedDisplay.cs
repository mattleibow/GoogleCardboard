using Android.Views;

namespace Google.Cardboard
{
	public class HeadMountedDisplay
	{
		private ScreenParams mScreen;
		private CardboardDeviceParams mCardboard;

		public HeadMountedDisplay(Display display)
		{
			mScreen = new ScreenParams(display);
			mCardboard = new CardboardDeviceParams();
		}

		public HeadMountedDisplay(HeadMountedDisplay hmd)
		{
			mScreen = new ScreenParams(hmd.mScreen);
			mCardboard = new CardboardDeviceParams(hmd.mCardboard);
		}

		public virtual ScreenParams Screen
		{
			set { mScreen = new ScreenParams(value); }
			get { return mScreen; }
		}

		public virtual CardboardDeviceParams Cardboard
		{
			set { mCardboard = new CardboardDeviceParams(value); }
			get { return mCardboard; }
		}

		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other == this)
			{
				return true;
			}
			if (!(other is HeadMountedDisplay))
			{
				return false;
			}

			HeadMountedDisplay o = (HeadMountedDisplay) other;
			return (mScreen.Equals(o.mScreen)) && (mCardboard.Equals(o.mCardboard));
		}
	}
}
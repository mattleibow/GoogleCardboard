using Android.App;
using Android.OS;
using Android.Views;
using Java.Lang;
using Google.Cardboard.Sensors;
using Android.Content;

namespace Google.Cardboard
{
	public class CardboardActivity : Activity, MagnetSensor.IOnCardboardTriggerListener, NfcSensor.IOnCardboardNfcListener
	{
		private static int NAVIGATION_BAR_TIMEOUT_MS = 2000;
		private CardboardView mCardboardView;
		private MagnetSensor mMagnetSensor;
		private NfcSensor mNfcSensor;

		public CardboardView CardboardView
		{
			get { return mCardboardView; }
			set
			{
				mCardboardView = value;
				if (value != null)
				{
					CardboardDeviceParams cardboardDeviceParams = mNfcSensor.CardboardDeviceParams;
					if (cardboardDeviceParams == null)
					{
						cardboardDeviceParams = new CardboardDeviceParams();
					}
					value.CardboardDeviceParams = cardboardDeviceParams;
				}
			}
		}

		public int VolumeKeysMode { get; set; }

		public bool AreVolumeKeysDisabled()
		{
			switch (VolumeKeysMode)
			{
				case 0:
					return false;
				case 2:
					return DeviceInCardboard;
				case 1:
					return true;
			}
			throw new IllegalStateException("Invalid volume keys mode " + VolumeKeysMode);
		}

		public bool DeviceInCardboard
		{
			get { return mNfcSensor.DeviceInCardboard; }
		}

		public virtual void OnInsertedIntoCardboard(CardboardDeviceParams deviceParams)
		{
			if (mCardboardView != null)
			{
				mCardboardView.CardboardDeviceParams = deviceParams;
			}
		}

		public virtual void OnRemovedFromCardboard()
		{
		}

		public virtual void OnCardboardTrigger()
		{
		}

		protected virtual void OnNfcIntent(Intent intent)
		{
			mNfcSensor.OnNfcIntent(intent);
		}

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);


			RequestWindowFeature(WindowFeatures.NoTitle);


			Window.AddFlags(WindowManagerFlags.KeepScreenOn);


			mMagnetSensor = new MagnetSensor(this);
			mMagnetSensor.OnCardboardTriggerListener = this;


			mNfcSensor = NfcSensor.GetInstance(this);
			mNfcSensor.AddOnCardboardNfcListener(this);


			OnNfcIntent(Intent);


			VolumeKeysMode = 2;
			if ((int) Build.VERSION.SdkInt < 19)
			{
				Handler handler = new Handler();
				Window.DecorView.SystemUiVisibilityChange += (sender, e) =>
				{
					if (((int) e.Visibility & 0x2) == 0)
					{
						handler.PostDelayed(new Runnable(() => { SetFullscreenMode(); }), 2000L);
					}
				};
			}
		}

		protected override void OnResume()
		{
			base.OnResume();
			if (mCardboardView != null)
			{
				mCardboardView.OnResume();
			}
			mMagnetSensor.Start();
			mNfcSensor.OnResume(this);
		}

		protected override void OnPause()
		{
			base.OnPause();
			if (mCardboardView != null)
			{
				mCardboardView.OnPause();
			}
			mMagnetSensor.Stop();
			mNfcSensor.OnPause(this);
		}

		protected override void OnDestroy()
		{
			mNfcSensor.RemoveOnCardboardNfcListener(this);
			base.OnDestroy();
		}

		public override void SetContentView(View view)
		{
			if ((view is CardboardView))
			{
				CardboardView = (CardboardView) view;
			}
			base.SetContentView(view);
		}

		public override void SetContentView(View view, ViewGroup.LayoutParams @params)
		{
			if ((view is CardboardView))
			{
				CardboardView = (CardboardView) view;
			}
			base.SetContentView(view, @params);
		}

		public override bool OnKeyDown(Keycode keyCode, KeyEvent e)
		{
			if (((keyCode == Keycode.VolumeUp) || (keyCode == Keycode.VolumeDown)) && (AreVolumeKeysDisabled()))
			{
				return true;
			}
			return base.OnKeyDown(keyCode, e);
		}

		public override bool OnKeyUp(Keycode keyCode, KeyEvent e)
		{
			if (((keyCode == Keycode.VolumeUp) || (keyCode == Keycode.VolumeDown)) && (AreVolumeKeysDisabled()))
			{
				return true;
			}
			return base.OnKeyUp(keyCode, e);
		}

		public override void OnWindowFocusChanged(bool hasFocus)
		{
			base.OnWindowFocusChanged(hasFocus);
			if (hasFocus)
			{
				SetFullscreenMode();
			}
		}

		private void SetFullscreenMode()
		{
			Window.DecorView.SystemUiVisibility = (StatusBarVisibility) 5894;
		}

		public static class VolumeKeys
		{
			public static int NotDisabled = 0;
			public static int Disabled = 1;
			public static int DisabledWhileInCardboard = 2;
		}
	}
}
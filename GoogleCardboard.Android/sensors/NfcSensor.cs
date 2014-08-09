using System;
using System.Collections;
using System.Collections.Generic;
using Java.Util;
using Android.Nfc;
using Android.Content;
using Java.Lang;
using Android.OS;
using Java.IO;
using Android.Util;
using Android.Nfc.Tech;
using Android.App;
using Exception = System.Exception;

namespace Google.Cardboard.Sensors
{
	public class NfcSensor
	{
		public const string NFC_DATA_SCHEME = "cardboard";
		public const string FIRST_TAG_VERSION = "v1.0.0";
		private const string TAG = "NfcSensor";
		private const int MAX_CONNECTION_FAILURES = 1;
		private const long NFC_POLLING_INTERVAL_MS = 250L;
		private static NfcSensor sInstance;
		private readonly Context mContext;
		private readonly NfcAdapter mNfcAdapter;
		private readonly object mTagLock;
		private readonly IList<ListenerHelper> mListeners;
		private IntentFilter[] mNfcIntentFilters;
		private volatile Ndef mCurrentTag;
		private Timer mNfcDisconnectTimer;
		private int mTagConnectionFailures;

		public static NfcSensor GetInstance(Context context)
		{
			if (sInstance == null)
			{
				sInstance = new NfcSensor(context);
			}
			return sInstance;
		}

		private NfcSensor(Context context)
		{
			mContext = context.ApplicationContext;
			mNfcAdapter = NfcAdapter.GetDefaultAdapter(mContext);
			mListeners = new List<ListenerHelper>();
			mTagLock = new object();
			if (mNfcAdapter == null)
			{
				return;
			}
			IntentFilter ndefIntentFilter = new IntentFilter("android.nfc.action.NDEF_DISCOVERED");
			ndefIntentFilter.AddDataScheme("cardboard");
			mNfcIntentFilters = new IntentFilter[] {ndefIntentFilter};

			mContext.RegisterReceiver(new NdefReceiver(this), ndefIntentFilter);
		}

		private class NdefReceiver : BroadcastReceiver
		{
			private readonly NfcSensor parentNfcSensor;

			public NdefReceiver(NfcSensor nfcSensor)
			{
				parentNfcSensor = nfcSensor;
			}

			public override void OnReceive(Context context, Intent intent)
			{
				parentNfcSensor.OnNfcIntent(intent);
			}
		}

		public virtual void AddOnCardboardNfcListener(IOnCardboardNfcListener listener)
		{
			if (listener == null)
			{
				return;
			}
			lock (mListeners)
			{
				foreach (ListenerHelper helper in mListeners)
				{
					if (helper.Listener == listener)
					{
						return;
					}
				}
				mListeners.Add(new ListenerHelper(listener, new Handler()));
			}
		}

		public virtual void RemoveOnCardboardNfcListener(IOnCardboardNfcListener listener)
		{
			if (listener == null)
			{
				return;
			}
			lock (mListeners)
			{
				foreach (ListenerHelper helper in mListeners)
				{
					if (helper.Listener == listener)
					{
						mListeners.Remove(helper);
						return;
					}
				}
			}
		}

		public virtual bool NfcSupported
		{
			get { return mNfcAdapter != null; }
		}

		public virtual bool NfcEnabled
		{
			get { return (NfcSupported) && (mNfcAdapter.IsEnabled); }
		}

		public virtual bool DeviceInCardboard
		{
			get { return mCurrentTag != null; }
		}

		public virtual CardboardDeviceParams CardboardDeviceParams
		{
			get
			{
				NdefMessage tagContents = null;
				lock (mTagLock)
				{
					try
					{
						tagContents = mCurrentTag.CachedNdefMessage;
					}
					catch
					{
						return null;
					}
				}
				if (tagContents == null)
				{
					return null;
				}
				return CardboardDeviceParams.createFromNfcContents(tagContents);
			}
		}

		public virtual void OnResume(Activity activity)
		{
			if (!NfcEnabled)
			{
				return;
			}
			Intent intent = new Intent("android.nfc.action.NDEF_DISCOVERED");
			intent.SetPackage(activity.PackageName);

			PendingIntent pendingIntent = PendingIntent.GetBroadcast(mContext, 0, intent, 0);
			mNfcAdapter.EnableForegroundDispatch(activity, pendingIntent, mNfcIntentFilters, (string[][]) null);
		}

		public virtual void OnPause(Activity activity)
		{
			if (!NfcEnabled)
			{
				return;
			}
			mNfcAdapter.DisableForegroundDispatch(activity);
		}

		public virtual void OnNfcIntent(Intent intent)
		{
			if ((!NfcEnabled) || (intent == null) || (!"android.nfc.action.NDEF_DISCOVERED".Equals(intent.Action)))
			{
				return;
			}
			var uri = intent.Data;
			Tag nfcTag = (Tag) intent.GetParcelableExtra("android.nfc.extra.TAG");
			if ((uri == null) || (nfcTag == null))
			{
				return;
			}
			Ndef ndef = Ndef.Get(nfcTag);
			if ((ndef == null) || (!uri.Scheme.Equals("cardboard")) ||
			    ((!uri.Host.Equals("v1.0.0")) && (uri.PathSegments.Count == 2)))
			{
				return;
			}
			lock (mTagLock)
			{
				bool isSameTag = false;
				if (mCurrentTag != null)
				{
					byte[] tagId1 = nfcTag.GetId();
					byte[] tagId2 = mCurrentTag.Tag.GetId();
					isSameTag = (tagId1 != null) && (tagId2 != null) && (Arrays.Equals(tagId1, tagId2));


					CloseCurrentNfcTag();
					if (!isSameTag)
					{
						SendDisconnectionEvent();
					}
				}
				NdefMessage nfcTagContents;
				try
				{
					ndef.Connect();
					nfcTagContents = ndef.CachedNdefMessage;
				}
				catch (Exception e)
				{
					Log.Error("NfcSensor", "Error reading NFC tag: " + e.ToString());
					if (isSameTag)
					{
						SendDisconnectionEvent();
					}
					return;
				}
				mCurrentTag = ndef;
				if (!isSameTag)
				{
					lock (mListeners)
					{
						foreach (ListenerHelper listener in mListeners)
						{
							listener.OnInsertedIntoCardboard(CardboardDeviceParams.createFromNfcContents(nfcTagContents));
						}
					}
				}
				mTagConnectionFailures = 0;
				mNfcDisconnectTimer = new Timer("NFC disconnect timer");
				mNfcDisconnectTimer.Schedule(new DisconnectionTimer(this), 250L, 250L);
			}
		}

		private class DisconnectionTimer : TimerTask
		{
			private readonly NfcSensor parentNfcSensor;

			public DisconnectionTimer(NfcSensor nfcSensor)
			{
				parentNfcSensor = nfcSensor;
			}

			public override void Run()
			{
				lock (parentNfcSensor.mTagLock)
				{
					if (!parentNfcSensor.mCurrentTag.IsConnected)
					{
						//		 NfcSensor.access$204(parentNfcSensor);
						if (parentNfcSensor.mTagConnectionFailures > 1)
						{
							parentNfcSensor.CloseCurrentNfcTag();
							parentNfcSensor.SendDisconnectionEvent();
						}
					}
				}
			}
		}

		private void CloseCurrentNfcTag()
		{
			if (mNfcDisconnectTimer != null)
			{
				mNfcDisconnectTimer.Cancel();
			}
			try
			{
				mCurrentTag.Close();
			}
			catch (IOException e)
			{
				Log.Warn("NfcSensor", e.ToString());
			}
			mCurrentTag = null;
		}

		private void SendDisconnectionEvent()
		{
			lock (mListeners)
			{
				foreach (ListenerHelper listener in mListeners)
				{
					listener.OnRemovedFromCardboard();
				}
			}
		}

		private class ListenerHelper : IOnCardboardNfcListener
		{
			internal IOnCardboardNfcListener mListener;
			internal Handler mHandler;

			public ListenerHelper(IOnCardboardNfcListener listener, Handler handler)
			{
				mListener = listener;
				mHandler = handler;
			}

			public virtual IOnCardboardNfcListener Listener
			{
				get { return mListener; }
			}

			public virtual void OnInsertedIntoCardboard(CardboardDeviceParams deviceParams)
			{
				mHandler.Post(() => mListener.OnInsertedIntoCardboard(deviceParams));
			}

			public virtual void OnRemovedFromCardboard()
			{
				mHandler.Post(() => mListener.OnRemovedFromCardboard());
			}
		}

		public interface IOnCardboardNfcListener
		{
			void OnInsertedIntoCardboard(CardboardDeviceParams paramCardboardDeviceParams);

			void OnRemovedFromCardboard();
		}
	}
}
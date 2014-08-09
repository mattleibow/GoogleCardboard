using System;
using System.Collections;
using System.Collections.Generic;

 namespace com.google.vrtoolkit.cardboard.sensors
 {
	 
		 using Activity = android.app.Activity;
		 using PendingIntent = android.app.PendingIntent;
		 using BroadcastReceiver = android.content.BroadcastReceiver;
		 using Context = android.content.Context;
		 using Intent = android.content.Intent;
		 using IntentFilter = android.content.IntentFilter;
		 using Uri = android.net.Uri;
		 using NdefMessage = android.nfc.NdefMessage;
		 using NfcAdapter = android.nfc.NfcAdapter;
		 using Tag = android.nfc.Tag;
		 using Ndef = android.nfc.tech.Ndef;
		 using Handler = android.os.Handler;
		 using Log = android.util.Log;
								 
		 public class NfcSensor
	
	 {		   public const string NFC_DATA_SCHEME = "cardboard";
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
	   
		   public static NfcSensor getInstance(Context context)
	
	   {			 if (sInstance == null)
		 {
			   sInstance = new NfcSensor(context);
	
		 }			 return sInstance;
	
	   }	   
		   private NfcSensor(Context context)
	
	   {			 this.mContext = context.ApplicationContext;
			 this.mNfcAdapter = NfcAdapter.getDefaultAdapter(this.mContext);
			 this.mListeners = new ArrayList();
			 this.mTagLock = new object();
			 if (this.mNfcAdapter == null)
		 {
			   return;
	
		 }			 IntentFilter ndefIntentFilter = new IntentFilter("android.nfc.action.NDEF_DISCOVERED");
			 ndefIntentFilter.addDataScheme("cardboard");
			 this.mNfcIntentFilters = new IntentFilter[] {ndefIntentFilter};
	     
	 
			 this.mContext.registerReceiver(new BroadcastReceiverAnonymousInnerClassHelper(this, context), ndefIntentFilter);
	
	   }
	   private class BroadcastReceiverAnonymousInnerClassHelper : BroadcastReceiver
	   {
		   private readonly NfcSensor outerInstance;

		   private Context context;

		   public BroadcastReceiverAnonymousInnerClassHelper(NfcSensor outerInstance, Context context)
		   {
			   this.outerInstance = outerInstance;
			   this.context = context;
		   }

	   	   		   public virtual void onReceive(Context context, Intent intent)
	   
		   {	   			 outerInstance.onNfcIntent(intent);
	   
		   }	   
	   }	   
		   public virtual void addOnCardboardNfcListener(OnCardboardNfcListener listener)
	
	   {			 if (listener == null)
		 {
			   return;
	
		 }			 lock (this.mListeners)
	
		 {			   foreach (ListenerHelper helper in this.mListeners)
		   {
				 if (helper.Listener == listener)
			 {
				   return;
	
			 }	
		   }			   this.mListeners.Add(new ListenerHelper(listener, new Handler()));
	
		 }	
	   }	   
		   public virtual void removeOnCardboardNfcListener(OnCardboardNfcListener listener)
	
	   {			 if (listener == null)
		 {
			   return;
	
		 }			 lock (this.mListeners)
	
		 {			   foreach (ListenerHelper helper in this.mListeners)
		   {
				 if (helper.Listener == listener)
	
			 {				   this.mListeners.Remove(helper);
				   return;
	
			 }	
		   }	
		 }	
	   }	   
	
	   public virtual bool NfcSupported
	   {
		   get
		
		   {					 return this.mNfcAdapter != null;
		
		   }
	   }	   
	
	   public virtual bool NfcEnabled
	   {
		   get
		
		   {					 return (NfcSupported) && (this.mNfcAdapter.Enabled);
		
		   }
	   }	   
	
	   public virtual bool DeviceInCardboard
	   {
		   get
		
		   {					 return this.mCurrentTag != null;
		
		   }
	   }	   
	
	   public virtual CardboardDeviceParams CardboardDeviceParams
	   {
		   get
		
		   {					 NdefMessage tagContents = null;
					 lock (this.mTagLock)
		
			 {					   try
		
			   {						 tagContents = this.mCurrentTag.CachedNdefMessage;
		
			   }					   catch (Exception)
		
			   {						 return null;
		
			   }		
			 }					 if (tagContents == null)
			 {
					   return null;
		
			 }					 return CardboardDeviceParams.createFromNfcContents(tagContents);
		
		   }
	   }	   
		   public virtual void onResume(Activity activity)
	
	   {			 if (!NfcEnabled)
		 {
			   return;
	
		 }			 Intent intent = new Intent("android.nfc.action.NDEF_DISCOVERED");
			 intent.Package = activity.PackageName;
	     
			 PendingIntent pendingIntent = PendingIntent.getBroadcast(this.mContext, 0, intent, 0);
			 this.mNfcAdapter.enableForegroundDispatch(activity, pendingIntent, this.mNfcIntentFilters, (string[][])null);
	
	   }	   
		   public virtual void onPause(Activity activity)
	
	   {			 if (!NfcEnabled)
		 {
			   return;
	
		 }			 this.mNfcAdapter.disableForegroundDispatch(activity);
	
	   }	   
		   public virtual void onNfcIntent(Intent intent)
	
	   {			 if ((!NfcEnabled) || (intent == null) || (!"android.nfc.action.NDEF_DISCOVERED".Equals(intent.Action)))
		 {
			   return;
	
		 }			 Uri uri = intent.Data;
			 Tag nfcTag = (Tag)intent.getParcelableExtra("android.nfc.extra.TAG");
			 if ((uri == null) || (nfcTag == null))
		 {
			   return;
	
		 }			 Ndef ndef = Ndef.get(nfcTag);
			 if ((ndef == null) || (!uri.Scheme.Equals("cardboard")) || ((!uri.Host.Equals("v1.0.0")) && (uri.PathSegments.size() == 2)))
		 {
			   return;
	
		 }			 lock (this.mTagLock)
	
		 {			   bool isSameTag = false;
			   if (this.mCurrentTag != null)
	
		   {				 sbyte[] tagId1 = nfcTag.Id;
				 sbyte[] tagId2 = this.mCurrentTag.Tag.Id;
				 isSameTag = (tagId1 != null) && (tagId2 != null) && (Arrays.Equals(tagId1, tagId2));
	         
	 
				 closeCurrentNfcTag();
				 if (!isSameTag)
			 {
				   sendDisconnectionEvent();
	
			 }	
		   }			   NdefMessage nfcTagContents;
			   try
	
		   {				 ndef.connect();
				 nfcTagContents = ndef.CachedNdefMessage;
	
		   }			   catch (Exception e)
	
		   {				 Log.e("NfcSensor", "Error reading NFC tag: " + e.ToString());
				 if (isSameTag)
			 {
				   sendDisconnectionEvent();
	
			 }				 return;
	
		   }			   this.mCurrentTag = ndef;
			   if (!isSameTag)
		   {
				 lock (this.mListeners)
	
			 {				   foreach (ListenerHelper listener in this.mListeners)
			   {
					 listener.onInsertedIntoCardboard(CardboardDeviceParams.createFromNfcContents(nfcTagContents));
	
			   }	
			 }	
		   }			   this.mTagConnectionFailures = 0;
			   this.mNfcDisconnectTimer = new Timer("NFC disconnect timer");
			   this.mNfcDisconnectTimer.schedule(new TimerTaskAnonymousInnerClassHelper(this), 250L, 250L);
	
		 }	
	   }
	   private class TimerTaskAnonymousInnerClassHelper : TimerTask
	   {
		   private readonly NfcSensor outerInstance;

		   public TimerTaskAnonymousInnerClassHelper(NfcSensor outerInstance)
		   {
			   this.outerInstance = outerInstance;
		   }

	   	   		   public virtual void run()
	   
		   {	   			 lock (outerInstance.mTagLock)
	   
			 {	   			   if (!outerInstance.mCurrentTag.Connected)
	   
			   {	   				 NfcSensor.access$204(outerInstance);
	   				 if (outerInstance.mTagConnectionFailures > 1)
	   
				 {	   				   outerInstance.closeCurrentNfcTag();
	   				   outerInstance.sendDisconnectionEvent();
	   
				 }	   
			   }	   
			 }	   
		   }	   
	   }	   
		   private void closeCurrentNfcTag()
	
	   {			 if (this.mNfcDisconnectTimer != null)
		 {
			   this.mNfcDisconnectTimer.cancel();
	
		 }			 try
	
		 {			   this.mCurrentTag.close();
	
		 }			 catch (IOException e)
	
		 {			   Log.w("NfcSensor", e.ToString());
	
		 }			 this.mCurrentTag = null;
	
	   }	   
		   private void sendDisconnectionEvent()
	
	   {			 lock (this.mListeners)
	
		 {			   foreach (ListenerHelper listener in this.mListeners)
		   {
				 listener.onRemovedFromCardboard();
	
		   }	
		 }	
	   }	   
		   private class ListenerHelper : NfcSensor.OnCardboardNfcListener
	
	
	   {			 internal NfcSensor.OnCardboardNfcListener mListener;
			 internal Handler mHandler;
	     
			 public ListenerHelper(NfcSensor.OnCardboardNfcListener listener, Handler handler)
	
		 {			   this.mListener = listener;
			   this.mHandler = handler;
	
		 }	     
	
		 public virtual NfcSensor.OnCardboardNfcListener Listener
		 {
			 get
		
			 {					   return this.mListener;
		
			 }
		 }	     
	//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void onInsertedIntoCardboard(final com.google.vrtoolkit.cardboard.CardboardDeviceParams deviceParams)
		 public virtual void onInsertedIntoCardboard(CardboardDeviceParams deviceParams)
	
		 {			   this.mHandler.post(new RunnableAnonymousInnerClassHelper(this, deviceParams));
	
		 }
		 private class RunnableAnonymousInnerClassHelper : Runnable
		 {
			 private readonly ListenerHelper outerInstance;

			 private CardboardDeviceParams deviceParams;

			 public RunnableAnonymousInnerClassHelper(ListenerHelper outerInstance, CardboardDeviceParams deviceParams)
			 {
				 this.outerInstance = outerInstance;
				 this.deviceParams = deviceParams;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mListener.onInsertedIntoCardboard(deviceParams);
		 
			 }		 
		 }	     
			 public virtual void onRemovedFromCardboard()
	
		 {			   this.mHandler.post(new RunnableAnonymousInnerClassHelper2(this));
	
		 }
		 private class RunnableAnonymousInnerClassHelper2 : Runnable
		 {
			 private readonly ListenerHelper outerInstance;

			 public RunnableAnonymousInnerClassHelper2(ListenerHelper outerInstance)
			 {
				 this.outerInstance = outerInstance;
			 }

		 		 			 public virtual void run()
		 
			 {		 			   outerInstance.outerInstance.mListener.onRemovedFromCardboard();
		 
			 }		 
		 }	
	   }	   
		   public abstract interface OnCardboardNfcListener
	
	   {			 void onInsertedIntoCardboard(CardboardDeviceParams paramCardboardDeviceParams);
	     
			 void onRemovedFromCardboard();
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.NfcSensor
	 * JD-Core Version:    0.7.0.1
	 */
 }
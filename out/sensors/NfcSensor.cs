using System;
using System.Collections;
using System.Collections.Generic;

/*   1:    */ namespace com.google.vrtoolkit.cardboard.sensors
 {
	/*   2:    */ 
	/*   3:    */	 using Activity = android.app.Activity;
	/*   4:    */	 using PendingIntent = android.app.PendingIntent;
	/*   5:    */	 using BroadcastReceiver = android.content.BroadcastReceiver;
	/*   6:    */	 using Context = android.content.Context;
	/*   7:    */	 using Intent = android.content.Intent;
	/*   8:    */	 using IntentFilter = android.content.IntentFilter;
	/*   9:    */	 using Uri = android.net.Uri;
	/*  10:    */	 using NdefMessage = android.nfc.NdefMessage;
	/*  11:    */	 using NfcAdapter = android.nfc.NfcAdapter;
	/*  12:    */	 using Tag = android.nfc.Tag;
	/*  13:    */	 using Ndef = android.nfc.tech.Ndef;
	/*  14:    */	 using Handler = android.os.Handler;
	/*  15:    */	 using Log = android.util.Log;
	/*  16:    */	/*  17:    */	/*  18:    */	/*  19:    */	/*  20:    */	/*  21:    */	/*  22:    */	/*  23:    */ 
	/*  24:    */	 public class NfcSensor
	/*  25:    */
	 {	/*  26:    */	   public const string NFC_DATA_SCHEME = "cardboard";
	/*  27:    */	   public const string FIRST_TAG_VERSION = "v1.0.0";
	/*  28:    */	   private const string TAG = "NfcSensor";
	/*  29:    */	   private const int MAX_CONNECTION_FAILURES = 1;
	/*  30:    */	   private const long NFC_POLLING_INTERVAL_MS = 250L;
	/*  31:    */	   private static NfcSensor sInstance;
	/*  32:    */	   private readonly Context mContext;
	/*  33:    */	   private readonly NfcAdapter mNfcAdapter;
	/*  34:    */	   private readonly object mTagLock;
	/*  35:    */	   private readonly IList<ListenerHelper> mListeners;
	/*  36:    */	   private IntentFilter[] mNfcIntentFilters;
	/*  37:    */	   private volatile Ndef mCurrentTag;
	/*  38:    */	   private Timer mNfcDisconnectTimer;
	/*  39:    */	   private int mTagConnectionFailures;
	/*  40:    */   
	/*  41:    */	   public static NfcSensor getInstance(Context context)
	/*  42:    */
	   {	/*  43: 93 */		 if (sInstance == null)
		 {
	/*  44: 94 */		   sInstance = new NfcSensor(context);
	/*  45:    */
		 }	/*  46: 97 */		 return sInstance;
	/*  47:    */
	   }	/*  48:    */   
	/*  49:    */	   private NfcSensor(Context context)
	/*  50:    */
	   {	/*  51:101 */		 this.mContext = context.ApplicationContext;
	/*  52:102 */		 this.mNfcAdapter = NfcAdapter.getDefaultAdapter(this.mContext);
	/*  53:103 */		 this.mListeners = new ArrayList();
	/*  54:104 */		 this.mTagLock = new object();
	/*  55:107 */		 if (this.mNfcAdapter == null)
		 {
	/*  56:108 */		   return;
	/*  57:    */
		 }	/*  58:112 */		 IntentFilter ndefIntentFilter = new IntentFilter("android.nfc.action.NDEF_DISCOVERED");
	/*  59:113 */		 ndefIntentFilter.addDataScheme("cardboard");
	/*  60:114 */		 this.mNfcIntentFilters = new IntentFilter[] {ndefIntentFilter};
	/*  61:    */     
	/*  62:    */ 
	/*  63:117 */		 this.mContext.registerReceiver(new BroadcastReceiverAnonymousInnerClassHelper(this, context), ndefIntentFilter);
	/*  70:    */
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

	   /*  64:    */	   /*  65:    */		   public virtual void onReceive(Context context, Intent intent)
	   /*  66:    */
		   {	   /*  67:120 */			 outerInstance.onNfcIntent(intent);
	   /*  68:    */
		   }	   /*  69:120 */
	   }	/*  71:    */   
	/*  72:    */	   public virtual void addOnCardboardNfcListener(OnCardboardNfcListener listener)
	/*  73:    */
	   {	/*  74:133 */		 if (listener == null)
		 {
	/*  75:134 */		   return;
	/*  76:    */
		 }	/*  77:137 */		 lock (this.mListeners)
	/*  78:    */
		 {	/*  79:138 */		   foreach (ListenerHelper helper in this.mListeners)
		   {
	/*  80:139 */			 if (helper.Listener == listener)
			 {
	/*  81:140 */			   return;
	/*  82:    */
			 }	/*  83:    */
		   }	/*  84:144 */		   this.mListeners.Add(new ListenerHelper(listener, new Handler()));
	/*  85:    */
		 }	/*  86:    */
	   }	/*  87:    */   
	/*  88:    */	   public virtual void removeOnCardboardNfcListener(OnCardboardNfcListener listener)
	/*  89:    */
	   {	/*  90:154 */		 if (listener == null)
		 {
	/*  91:155 */		   return;
	/*  92:    */
		 }	/*  93:158 */		 lock (this.mListeners)
	/*  94:    */
		 {	/*  95:159 */		   foreach (ListenerHelper helper in this.mListeners)
		   {
	/*  96:160 */			 if (helper.Listener == listener)
	/*  97:    */
			 {	/*  98:161 */			   this.mListeners.Remove(helper);
	/*  99:162 */			   return;
	/* 100:    */
			 }	/* 101:    */
		   }	/* 102:    */
		 }	/* 103:    */
	   }	/* 104:    */   
	/* 105:    */
	   public virtual bool NfcSupported
	   {
		   get
		/* 106:    */
		   {		/* 107:174 */			 return this.mNfcAdapter != null;
		/* 108:    */
		   }
	   }	/* 109:    */   
	/* 110:    */
	   public virtual bool NfcEnabled
	   {
		   get
		/* 111:    */
		   {		/* 112:183 */			 return (NfcSupported) && (this.mNfcAdapter.Enabled);
		/* 113:    */
		   }
	   }	/* 114:    */   
	/* 115:    */
	   public virtual bool DeviceInCardboard
	   {
		   get
		/* 116:    */
		   {		/* 117:193 */			 return this.mCurrentTag != null;
		/* 118:    */
		   }
	   }	/* 119:    */   
	/* 120:    */
	   public virtual CardboardDeviceParams CardboardDeviceParams
	   {
		   get
		/* 121:    */
		   {		/* 122:202 */			 NdefMessage tagContents = null;
		/* 123:203 */			 lock (this.mTagLock)
		/* 124:    */
			 {		/* 125:    */			   try
		/* 126:    */
			   {		/* 127:205 */				 tagContents = this.mCurrentTag.CachedNdefMessage;
		/* 128:    */
			   }		/* 129:    */			   catch (Exception)
		/* 130:    */
			   {		/* 131:207 */				 return null;
		/* 132:    */
			   }		/* 133:    */
			 }		/* 134:211 */			 if (tagContents == null)
			 {
		/* 135:212 */			   return null;
		/* 136:    */
			 }		/* 137:215 */			 return CardboardDeviceParams.createFromNfcContents(tagContents);
		/* 138:    */
		   }
	   }	/* 139:    */   
	/* 140:    */	   public virtual void onResume(Activity activity)
	/* 141:    */
	   {	/* 142:224 */		 if (!NfcEnabled)
		 {
	/* 143:225 */		   return;
	/* 144:    */
		 }	/* 145:228 */		 Intent intent = new Intent("android.nfc.action.NDEF_DISCOVERED");
	/* 146:229 */		 intent.Package = activity.PackageName;
	/* 147:    */     
	/* 148:231 */		 PendingIntent pendingIntent = PendingIntent.getBroadcast(this.mContext, 0, intent, 0);
	/* 149:232 */		 this.mNfcAdapter.enableForegroundDispatch(activity, pendingIntent, this.mNfcIntentFilters, (string[][])null);
	/* 150:    */
	   }	/* 151:    */   
	/* 152:    */	   public virtual void onPause(Activity activity)
	/* 153:    */
	   {	/* 154:241 */		 if (!NfcEnabled)
		 {
	/* 155:242 */		   return;
	/* 156:    */
		 }	/* 157:245 */		 this.mNfcAdapter.disableForegroundDispatch(activity);
	/* 158:    */
	   }	/* 159:    */   
	/* 160:    */	   public virtual void onNfcIntent(Intent intent)
	/* 161:    */
	   {	/* 162:254 */		 if ((!NfcEnabled) || (intent == null) || (!"android.nfc.action.NDEF_DISCOVERED".Equals(intent.Action)))
		 {
	/* 163:256 */		   return;
	/* 164:    */
		 }	/* 165:259 */		 Uri uri = intent.Data;
	/* 166:260 */		 Tag nfcTag = (Tag)intent.getParcelableExtra("android.nfc.extra.TAG");
	/* 167:261 */		 if ((uri == null) || (nfcTag == null))
		 {
	/* 168:262 */		   return;
	/* 169:    */
		 }	/* 170:266 */		 Ndef ndef = Ndef.get(nfcTag);
	/* 171:267 */		 if ((ndef == null) || (!uri.Scheme.Equals("cardboard")) || ((!uri.Host.Equals("v1.0.0")) && (uri.PathSegments.size() == 2)))
		 {
	/* 172:270 */		   return;
	/* 173:    */
		 }	/* 174:273 */		 lock (this.mTagLock)
	/* 175:    */
		 {	/* 176:274 */		   bool isSameTag = false;
	/* 177:276 */		   if (this.mCurrentTag != null)
	/* 178:    */
		   {	/* 179:279 */			 sbyte[] tagId1 = nfcTag.Id;
	/* 180:280 */			 sbyte[] tagId2 = this.mCurrentTag.Tag.Id;
	/* 181:281 */			 isSameTag = (tagId1 != null) && (tagId2 != null) && (Arrays.Equals(tagId1, tagId2));
	/* 182:    */         
	/* 183:    */ 
	/* 184:284 */			 closeCurrentNfcTag();
	/* 185:285 */			 if (!isSameTag)
			 {
	/* 186:286 */			   sendDisconnectionEvent();
	/* 187:    */
			 }	/* 188:    */
		   }	/* 189:    */		   NdefMessage nfcTagContents;
	/* 190:    */		   try
	/* 191:    */
		   {	/* 192:293 */			 ndef.connect();
	/* 193:294 */			 nfcTagContents = ndef.CachedNdefMessage;
	/* 194:    */
		   }	/* 195:    */		   catch (Exception e)
	/* 196:    */
		   {	/* 197:296 */			 Log.e("NfcSensor", "Error reading NFC tag: " + e.ToString());
	/* 198:299 */			 if (isSameTag)
			 {
	/* 199:300 */			   sendDisconnectionEvent();
	/* 200:    */
			 }	/* 201:303 */			 return;
	/* 202:    */
		   }	/* 203:306 */		   this.mCurrentTag = ndef;
	/* 204:309 */		   if (!isSameTag)
		   {
	/* 205:310 */			 lock (this.mListeners)
	/* 206:    */
			 {	/* 207:311 */			   foreach (ListenerHelper listener in this.mListeners)
			   {
	/* 208:312 */				 listener.onInsertedIntoCardboard(CardboardDeviceParams.createFromNfcContents(nfcTagContents));
	/* 209:    */
			   }	/* 210:    */
			 }	/* 211:    */
		   }	/* 212:319 */		   this.mTagConnectionFailures = 0;
	/* 213:320 */		   this.mNfcDisconnectTimer = new Timer("NFC disconnect timer");
	/* 214:321 */		   this.mNfcDisconnectTimer.schedule(new TimerTaskAnonymousInnerClassHelper(this), 250L, 250L);
	/* 232:    */
		 }	/* 233:    */
	   }
	   private class TimerTaskAnonymousInnerClassHelper : TimerTask
	   {
		   private readonly NfcSensor outerInstance;

		   public TimerTaskAnonymousInnerClassHelper(NfcSensor outerInstance)
		   {
			   this.outerInstance = outerInstance;
		   }

	   /* 215:    */	   /* 216:    */		   public virtual void run()
	   /* 217:    */
		   {	   /* 218:324 */			 lock (outerInstance.mTagLock)
	   /* 219:    */
			 {	   /* 220:325 */			   if (!outerInstance.mCurrentTag.Connected)
	   /* 221:    */
			   {	   /* 222:326 */				 NfcSensor.access$204(outerInstance);
	   /* 223:328 */				 if (outerInstance.mTagConnectionFailures > 1)
	   /* 224:    */
				 {	   /* 225:329 */				   outerInstance.closeCurrentNfcTag();
	   /* 226:330 */				   outerInstance.sendDisconnectionEvent();
	   /* 227:    */
				 }	   /* 228:    */
			   }	   /* 229:    */
			 }	   /* 230:    */
		   }	   /* 231:330 */
	   }	/* 234:    */   
	/* 235:    */	   private void closeCurrentNfcTag()
	/* 236:    */
	   {	/* 237:341 */		 if (this.mNfcDisconnectTimer != null)
		 {
	/* 238:342 */		   this.mNfcDisconnectTimer.cancel();
	/* 239:    */
		 }	/* 240:    */		 try
	/* 241:    */
		 {	/* 242:347 */		   this.mCurrentTag.close();
	/* 243:    */
		 }	/* 244:    */		 catch (IOException e)
	/* 245:    */
		 {	/* 246:349 */		   Log.w("NfcSensor", e.ToString());
	/* 247:    */
		 }	/* 248:352 */		 this.mCurrentTag = null;
	/* 249:    */
	   }	/* 250:    */   
	/* 251:    */	   private void sendDisconnectionEvent()
	/* 252:    */
	   {	/* 253:356 */		 lock (this.mListeners)
	/* 254:    */
		 {	/* 255:357 */		   foreach (ListenerHelper listener in this.mListeners)
		   {
	/* 256:358 */			 listener.onRemovedFromCardboard();
	/* 257:    */
		   }	/* 258:    */
		 }	/* 259:    */
	   }	/* 260:    */   
	/* 261:    */	   private class ListenerHelper : NfcSensor.OnCardboardNfcListener
	/* 262:    */
	/* 263:    */
	   {	/* 264:    */		 internal NfcSensor.OnCardboardNfcListener mListener;
	/* 265:    */		 internal Handler mHandler;
	/* 266:    */     
	/* 267:    */		 public ListenerHelper(NfcSensor.OnCardboardNfcListener listener, Handler handler)
	/* 268:    */
		 {	/* 269:372 */		   this.mListener = listener;
	/* 270:373 */		   this.mHandler = handler;
	/* 271:    */
		 }	/* 272:    */     
	/* 273:    */
		 public virtual NfcSensor.OnCardboardNfcListener Listener
		 {
			 get
		/* 274:    */
			 {		/* 275:377 */			   return this.mListener;
		/* 276:    */
			 }
		 }	/* 277:    */     
	/* 278:    *///JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: public void onInsertedIntoCardboard(final com.google.vrtoolkit.cardboard.CardboardDeviceParams deviceParams)
		 public virtual void onInsertedIntoCardboard(CardboardDeviceParams deviceParams)
	/* 279:    */
		 {	/* 280:382 */		   this.mHandler.post(new RunnableAnonymousInnerClassHelper(this, deviceParams));
	/* 287:    */
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

		 /* 281:    */		 /* 282:    */			 public virtual void run()
		 /* 283:    */
			 {		 /* 284:385 */			   outerInstance.outerInstance.mListener.onInsertedIntoCardboard(deviceParams);
		 /* 285:    */
			 }		 /* 286:    */
		 }	/* 288:    */     
	/* 289:    */		 public virtual void onRemovedFromCardboard()
	/* 290:    */
		 {	/* 291:392 */		   this.mHandler.post(new RunnableAnonymousInnerClassHelper2(this));
	/* 298:    */
		 }
		 private class RunnableAnonymousInnerClassHelper2 : Runnable
		 {
			 private readonly ListenerHelper outerInstance;

			 public RunnableAnonymousInnerClassHelper2(ListenerHelper outerInstance)
			 {
				 this.outerInstance = outerInstance;
			 }

		 /* 292:    */		 /* 293:    */			 public virtual void run()
		 /* 294:    */
			 {		 /* 295:395 */			   outerInstance.outerInstance.mListener.onRemovedFromCardboard();
		 /* 296:    */
			 }		 /* 297:    */
		 }	/* 299:    */
	   }	/* 300:    */   
	/* 301:    */	   public abstract interface OnCardboardNfcListener
	/* 302:    */
	   {	/* 303:    */		 void onInsertedIntoCardboard(CardboardDeviceParams paramCardboardDeviceParams);
	/* 304:    */     
	/* 305:    */		 void onRemovedFromCardboard();
	/* 306:    */
	   }	/* 307:    */
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.NfcSensor
	 * JD-Core Version:    0.7.0.1
	 */
 }
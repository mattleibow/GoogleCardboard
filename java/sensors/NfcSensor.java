/*   1:    */ package com.google.vrtoolkit.cardboard.sensors;
/*   2:    */ 
/*   3:    */ import android.app.Activity;
/*   4:    */ import android.app.PendingIntent;
/*   5:    */ import android.content.BroadcastReceiver;
/*   6:    */ import android.content.Context;
/*   7:    */ import android.content.Intent;
/*   8:    */ import android.content.IntentFilter;
/*   9:    */ import android.net.Uri;
/*  10:    */ import android.nfc.NdefMessage;
/*  11:    */ import android.nfc.NfcAdapter;
/*  12:    */ import android.nfc.Tag;
/*  13:    */ import android.nfc.tech.Ndef;
/*  14:    */ import android.os.Handler;
/*  15:    */ import android.util.Log;
/*  16:    */ import com.google.vrtoolkit.cardboard.CardboardDeviceParams;
/*  17:    */ import java.io.IOException;
/*  18:    */ import java.util.ArrayList;
/*  19:    */ import java.util.Arrays;
/*  20:    */ import java.util.List;
/*  21:    */ import java.util.Timer;
/*  22:    */ import java.util.TimerTask;
/*  23:    */ 
/*  24:    */ public class NfcSensor
/*  25:    */ {
/*  26:    */   public static final String NFC_DATA_SCHEME = "cardboard";
/*  27:    */   public static final String FIRST_TAG_VERSION = "v1.0.0";
/*  28:    */   private static final String TAG = "NfcSensor";
/*  29:    */   private static final int MAX_CONNECTION_FAILURES = 1;
/*  30:    */   private static final long NFC_POLLING_INTERVAL_MS = 250L;
/*  31:    */   private static NfcSensor sInstance;
/*  32:    */   private final Context mContext;
/*  33:    */   private final NfcAdapter mNfcAdapter;
/*  34:    */   private final Object mTagLock;
/*  35:    */   private final List<ListenerHelper> mListeners;
/*  36:    */   private IntentFilter[] mNfcIntentFilters;
/*  37:    */   private volatile Ndef mCurrentTag;
/*  38:    */   private Timer mNfcDisconnectTimer;
/*  39:    */   private int mTagConnectionFailures;
/*  40:    */   
/*  41:    */   public static NfcSensor getInstance(Context context)
/*  42:    */   {
/*  43: 93 */     if (sInstance == null) {
/*  44: 94 */       sInstance = new NfcSensor(context);
/*  45:    */     }
/*  46: 97 */     return sInstance;
/*  47:    */   }
/*  48:    */   
/*  49:    */   private NfcSensor(Context context)
/*  50:    */   {
/*  51:101 */     this.mContext = context.getApplicationContext();
/*  52:102 */     this.mNfcAdapter = NfcAdapter.getDefaultAdapter(this.mContext);
/*  53:103 */     this.mListeners = new ArrayList();
/*  54:104 */     this.mTagLock = new Object();
/*  55:107 */     if (this.mNfcAdapter == null) {
/*  56:108 */       return;
/*  57:    */     }
/*  58:112 */     IntentFilter ndefIntentFilter = new IntentFilter("android.nfc.action.NDEF_DISCOVERED");
/*  59:113 */     ndefIntentFilter.addDataScheme("cardboard");
/*  60:114 */     this.mNfcIntentFilters = new IntentFilter[] { ndefIntentFilter };
/*  61:    */     
/*  62:    */ 
/*  63:117 */     this.mContext.registerReceiver(new BroadcastReceiver()
/*  64:    */     {
/*  65:    */       public void onReceive(Context context, Intent intent)
/*  66:    */       {
/*  67:120 */         NfcSensor.this.onNfcIntent(intent);
/*  68:    */       }
/*  69:120 */     }, ndefIntentFilter);
/*  70:    */   }
/*  71:    */   
/*  72:    */   public void addOnCardboardNfcListener(OnCardboardNfcListener listener)
/*  73:    */   {
/*  74:133 */     if (listener == null) {
/*  75:134 */       return;
/*  76:    */     }
/*  77:137 */     synchronized (this.mListeners)
/*  78:    */     {
/*  79:138 */       for (ListenerHelper helper : this.mListeners) {
/*  80:139 */         if (helper.getListener() == listener) {
/*  81:140 */           return;
/*  82:    */         }
/*  83:    */       }
/*  84:144 */       this.mListeners.add(new ListenerHelper(listener, new Handler()));
/*  85:    */     }
/*  86:    */   }
/*  87:    */   
/*  88:    */   public void removeOnCardboardNfcListener(OnCardboardNfcListener listener)
/*  89:    */   {
/*  90:154 */     if (listener == null) {
/*  91:155 */       return;
/*  92:    */     }
/*  93:158 */     synchronized (this.mListeners)
/*  94:    */     {
/*  95:159 */       for (ListenerHelper helper : this.mListeners) {
/*  96:160 */         if (helper.getListener() == listener)
/*  97:    */         {
/*  98:161 */           this.mListeners.remove(helper);
/*  99:162 */           return;
/* 100:    */         }
/* 101:    */       }
/* 102:    */     }
/* 103:    */   }
/* 104:    */   
/* 105:    */   public boolean isNfcSupported()
/* 106:    */   {
/* 107:174 */     return this.mNfcAdapter != null;
/* 108:    */   }
/* 109:    */   
/* 110:    */   public boolean isNfcEnabled()
/* 111:    */   {
/* 112:183 */     return (isNfcSupported()) && (this.mNfcAdapter.isEnabled());
/* 113:    */   }
/* 114:    */   
/* 115:    */   public boolean isDeviceInCardboard()
/* 116:    */   {
/* 117:193 */     return this.mCurrentTag != null;
/* 118:    */   }
/* 119:    */   
/* 120:    */   public CardboardDeviceParams getCardboardDeviceParams()
/* 121:    */   {
/* 122:202 */     NdefMessage tagContents = null;
/* 123:203 */     synchronized (this.mTagLock)
/* 124:    */     {
/* 125:    */       try
/* 126:    */       {
/* 127:205 */         tagContents = this.mCurrentTag.getCachedNdefMessage();
/* 128:    */       }
/* 129:    */       catch (Exception e)
/* 130:    */       {
/* 131:207 */         return null;
/* 132:    */       }
/* 133:    */     }
/* 134:211 */     if (tagContents == null) {
/* 135:212 */       return null;
/* 136:    */     }
/* 137:215 */     return CardboardDeviceParams.createFromNfcContents(tagContents);
/* 138:    */   }
/* 139:    */   
/* 140:    */   public void onResume(Activity activity)
/* 141:    */   {
/* 142:224 */     if (!isNfcEnabled()) {
/* 143:225 */       return;
/* 144:    */     }
/* 145:228 */     Intent intent = new Intent("android.nfc.action.NDEF_DISCOVERED");
/* 146:229 */     intent.setPackage(activity.getPackageName());
/* 147:    */     
/* 148:231 */     PendingIntent pendingIntent = PendingIntent.getBroadcast(this.mContext, 0, intent, 0);
/* 149:232 */     this.mNfcAdapter.enableForegroundDispatch(activity, pendingIntent, this.mNfcIntentFilters, (String[][])null);
/* 150:    */   }
/* 151:    */   
/* 152:    */   public void onPause(Activity activity)
/* 153:    */   {
/* 154:241 */     if (!isNfcEnabled()) {
/* 155:242 */       return;
/* 156:    */     }
/* 157:245 */     this.mNfcAdapter.disableForegroundDispatch(activity);
/* 158:    */   }
/* 159:    */   
/* 160:    */   public void onNfcIntent(Intent intent)
/* 161:    */   {
/* 162:254 */     if ((!isNfcEnabled()) || (intent == null) || (!"android.nfc.action.NDEF_DISCOVERED".equals(intent.getAction()))) {
/* 163:256 */       return;
/* 164:    */     }
/* 165:259 */     Uri uri = intent.getData();
/* 166:260 */     Tag nfcTag = (Tag)intent.getParcelableExtra("android.nfc.extra.TAG");
/* 167:261 */     if ((uri == null) || (nfcTag == null)) {
/* 168:262 */       return;
/* 169:    */     }
/* 170:266 */     Ndef ndef = Ndef.get(nfcTag);
/* 171:267 */     if ((ndef == null) || (!uri.getScheme().equals("cardboard")) || ((!uri.getHost().equals("v1.0.0")) && (uri.getPathSegments().size() == 2))) {
/* 172:270 */       return;
/* 173:    */     }
/* 174:273 */     synchronized (this.mTagLock)
/* 175:    */     {
/* 176:274 */       boolean isSameTag = false;
/* 177:276 */       if (this.mCurrentTag != null)
/* 178:    */       {
/* 179:279 */         byte[] tagId1 = nfcTag.getId();
/* 180:280 */         byte[] tagId2 = this.mCurrentTag.getTag().getId();
/* 181:281 */         isSameTag = (tagId1 != null) && (tagId2 != null) && (Arrays.equals(tagId1, tagId2));
/* 182:    */         
/* 183:    */ 
/* 184:284 */         closeCurrentNfcTag();
/* 185:285 */         if (!isSameTag) {
/* 186:286 */           sendDisconnectionEvent();
/* 187:    */         }
/* 188:    */       }
/* 189:    */       NdefMessage nfcTagContents;
/* 190:    */       try
/* 191:    */       {
/* 192:293 */         ndef.connect();
/* 193:294 */         nfcTagContents = ndef.getCachedNdefMessage();
/* 194:    */       }
/* 195:    */       catch (Exception e)
/* 196:    */       {
/* 197:296 */         Log.e("NfcSensor", "Error reading NFC tag: " + e.toString());
/* 198:299 */         if (isSameTag) {
/* 199:300 */           sendDisconnectionEvent();
/* 200:    */         }
/* 201:303 */         return;
/* 202:    */       }
/* 203:306 */       this.mCurrentTag = ndef;
/* 204:309 */       if (!isSameTag) {
/* 205:310 */         synchronized (this.mListeners)
/* 206:    */         {
/* 207:311 */           for (ListenerHelper listener : this.mListeners) {
/* 208:312 */             listener.onInsertedIntoCardboard(CardboardDeviceParams.createFromNfcContents(nfcTagContents));
/* 209:    */           }
/* 210:    */         }
/* 211:    */       }
/* 212:319 */       this.mTagConnectionFailures = 0;
/* 213:320 */       this.mNfcDisconnectTimer = new Timer("NFC disconnect timer");
/* 214:321 */       this.mNfcDisconnectTimer.schedule(new TimerTask()
/* 215:    */       {
/* 216:    */         public void run()
/* 217:    */         {
/* 218:324 */           synchronized (NfcSensor.this.mTagLock)
/* 219:    */           {
/* 220:325 */             if (!NfcSensor.this.mCurrentTag.isConnected())
/* 221:    */             {
/* 222:326 */               NfcSensor.access$204(NfcSensor.this);
/* 223:328 */               if (NfcSensor.this.mTagConnectionFailures > 1)
/* 224:    */               {
/* 225:329 */                 NfcSensor.this.closeCurrentNfcTag();
/* 226:330 */                 NfcSensor.this.sendDisconnectionEvent();
/* 227:    */               }
/* 228:    */             }
/* 229:    */           }
/* 230:    */         }
/* 231:330 */       }, 250L, 250L);
/* 232:    */     }
/* 233:    */   }
/* 234:    */   
/* 235:    */   private void closeCurrentNfcTag()
/* 236:    */   {
/* 237:341 */     if (this.mNfcDisconnectTimer != null) {
/* 238:342 */       this.mNfcDisconnectTimer.cancel();
/* 239:    */     }
/* 240:    */     try
/* 241:    */     {
/* 242:347 */       this.mCurrentTag.close();
/* 243:    */     }
/* 244:    */     catch (IOException e)
/* 245:    */     {
/* 246:349 */       Log.w("NfcSensor", e.toString());
/* 247:    */     }
/* 248:352 */     this.mCurrentTag = null;
/* 249:    */   }
/* 250:    */   
/* 251:    */   private void sendDisconnectionEvent()
/* 252:    */   {
/* 253:356 */     synchronized (this.mListeners)
/* 254:    */     {
/* 255:357 */       for (ListenerHelper listener : this.mListeners) {
/* 256:358 */         listener.onRemovedFromCardboard();
/* 257:    */       }
/* 258:    */     }
/* 259:    */   }
/* 260:    */   
/* 261:    */   private static class ListenerHelper
/* 262:    */     implements NfcSensor.OnCardboardNfcListener
/* 263:    */   {
/* 264:    */     private NfcSensor.OnCardboardNfcListener mListener;
/* 265:    */     private Handler mHandler;
/* 266:    */     
/* 267:    */     public ListenerHelper(NfcSensor.OnCardboardNfcListener listener, Handler handler)
/* 268:    */     {
/* 269:372 */       this.mListener = listener;
/* 270:373 */       this.mHandler = handler;
/* 271:    */     }
/* 272:    */     
/* 273:    */     public NfcSensor.OnCardboardNfcListener getListener()
/* 274:    */     {
/* 275:377 */       return this.mListener;
/* 276:    */     }
/* 277:    */     
/* 278:    */     public void onInsertedIntoCardboard(final CardboardDeviceParams deviceParams)
/* 279:    */     {
/* 280:382 */       this.mHandler.post(new Runnable()
/* 281:    */       {
/* 282:    */         public void run()
/* 283:    */         {
/* 284:385 */           NfcSensor.ListenerHelper.this.mListener.onInsertedIntoCardboard(deviceParams);
/* 285:    */         }
/* 286:    */       });
/* 287:    */     }
/* 288:    */     
/* 289:    */     public void onRemovedFromCardboard()
/* 290:    */     {
/* 291:392 */       this.mHandler.post(new Runnable()
/* 292:    */       {
/* 293:    */         public void run()
/* 294:    */         {
/* 295:395 */           NfcSensor.ListenerHelper.this.mListener.onRemovedFromCardboard();
/* 296:    */         }
/* 297:    */       });
/* 298:    */     }
/* 299:    */   }
/* 300:    */   
/* 301:    */   public static abstract interface OnCardboardNfcListener
/* 302:    */   {
/* 303:    */     public abstract void onInsertedIntoCardboard(CardboardDeviceParams paramCardboardDeviceParams);
/* 304:    */     
/* 305:    */     public abstract void onRemovedFromCardboard();
/* 306:    */   }
/* 307:    */ }


/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.sensors.NfcSensor
 * JD-Core Version:    0.7.0.1
 */
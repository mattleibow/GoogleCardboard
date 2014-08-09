/*   1:    */ 
/*   2:    */ 
/*   3:    */ internal android.app.Activity android, NAVIGATION_BAR_TIMEOUT_MS = 2000 mCardboardView mMagnetSensor mNfcSensor mVolumeKeysMode setCardboardView mCardboardView = cardboardView;
/*   4:    */
/*   5:    */
/*   6:    */
/*   7:    */
/*   8:    */
/*   9:    */
/*  10:    */
/*  11:    */
/*  12:    */
/*  13:    */
/*  14:    */
/*  15:    */
/*  16:    */
/*  17:    */ 
/*  18:    */
/*  19:    */   
/*  20:    */
/*  21:    */ 
/*  22:    */
/*  23:    */
/*  24:    */
/*  25:    */
/*  26:    */
/*  27:    */   
/*  28:    */
/*  29:    */   
/*  30: 92 */
/*  31: 95 */	 if (cardboardView != null)
/*  32:    */
	 {/*  33: 96 */	   CardboardDeviceParams cardboardDeviceParams = this.mNfcSensor.CardboardDeviceParams;
/*  34: 97 */	   if (cardboardDeviceParams == null)
	   {
/*  35: 98 */		 cardboardDeviceParams = new CardboardDeviceParams();
/*  36:    */
	   }/*  37:101 */	   cardboardView.updateCardboardDeviceParams(cardboardDeviceParams);
/*  38:    */
	 }/*  39:    */
   }/*  40:    */   
/*  41:    */   public CardboardView CardboardView
/*  42:    */
   {/*  43:112 */	 return this.mCardboardView;
/*  44:    */
   }/*  45:    */   
/*  46:    */   public void setVolumeKeysMode(int mode)
/*  47:    */
   {/*  48:125 */	 this.mVolumeKeysMode = mode;
/*  49:    */
   }/*  50:    */   
/*  51:    */   public int VolumeKeysMode
/*  52:    */
   {/*  53:135 */	 return this.mVolumeKeysMode;
/*  54:    */
   }/*  55:    */   
/*  56:    */   public bool areVolumeKeysDisabled()
/*  57:    */
   {/*  58:149 */	 switch (this.mVolumeKeysMode)
/*  59:    */
	 {/*  60:    */	 case 0:
/*  61:151 */	   return false;
/*  62:    */	 case 2:
/*  63:154 */	   return DeviceInCardboard;
/*  64:    */	 case 1:
/*  65:157 */	   return true;
/*  66:    */
	 }/*  67:160 */	 throw new IllegalStateException("Invalid volume keys mode " + this.mVolumeKeysMode);
/*  68:    */
   }/*  69:    */   
/*  70:    */   public bool DeviceInCardboard
/*  71:    */
   {/*  72:171 */	 return this.mNfcSensor.DeviceInCardboard;
/*  73:    */
   }/*  74:    */   
/*  75:    */   public void onInsertedIntoCardboard(CardboardDeviceParams deviceParams)
/*  76:    */
   {/*  77:182 */	 if (this.mCardboardView != null)
	 {
/*  78:183 */	   this.mCardboardView.updateCardboardDeviceParams(deviceParams);
/*  79:    */
	 }/*  80:    */
   }/*  81:    */   
/*  82:    */   public void onRemovedFromCardboard()
   {
   }
/*  83:    */   
/*  84:    */   public void onCardboardTrigger()
   {
   }
/*  85:    */   
/*  86:    */   protected void onNfcIntent(Intent intent)
/*  87:    */
   {/*  88:208 */	 this.mNfcSensor.onNfcIntent(intent);
/*  89:    */
   }/*  90:    */   
/*  91:    */   protected void onCreate(Bundle savedInstanceState)
/*  92:    */
   {/*  93:214 */	 base.onCreate(savedInstanceState);
/*  94:    */     
/*  95:    */ 
/*  96:217 */	 requestWindowFeature(1);
/*  97:    */     
/*  98:    */ 
/*  99:220 */	 Window.addFlags(128);
/* 100:    */     
/* 101:    */ 
/* 102:223 */	 this.mMagnetSensor = new MagnetSensor(this);
/* 103:224 */	 this.mMagnetSensor.OnCardboardTriggerListener = this;
/* 104:    */     
/* 105:    */ 
/* 106:227 */	 this.mNfcSensor = NfcSensor.getInstance(this);
/* 107:228 */	 this.mNfcSensor.addOnCardboardNfcListener(this);
/* 108:    */     
/* 109:    */ 
/* 110:231 */	 onNfcIntent(Intent);
/* 111:    */     
/* 112:    */ 
/* 113:234 */	 VolumeKeysMode = 2;
/* 114:237 */	 if (Build.VERSION.SDK_INT < 19)
/* 115:    */
	 {/* 116:238 *///JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Handler handler = new Handler();
	   Handler handler = new Handler();
/* 117:239 */	   Window.DecorView.OnSystemUiVisibilityChangeListener = new OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper(this, handler);
/* 132:    */
	 }/* 133:    */
   }/* 134:    */   
/* 135:    */   protected void onResume()
/* 136:    */
   {/* 137:259 */	 base.onResume();
/* 138:261 */	 if (this.mCardboardView != null)
	 {
/* 139:262 */	   this.mCardboardView.onResume();
/* 140:    */
	 }/* 141:266 */	 this.mMagnetSensor.start();
/* 142:267 */	 this.mNfcSensor.onResume(this);
/* 143:    */
   }/* 144:    */   
/* 145:    */   protected void onPause()
/* 146:    */
   {/* 147:273 */	 base.onPause();
/* 148:275 */	 if (this.mCardboardView != null)
	 {
/* 149:276 */	   this.mCardboardView.onPause();
/* 150:    */
	 }/* 151:280 */	 this.mMagnetSensor.stop();
/* 152:281 */	 this.mNfcSensor.onPause(this);
/* 153:    */
   }/* 154:    */   
/* 155:    */   protected void onDestroy()
/* 156:    */
   {/* 157:287 */	 this.mNfcSensor.removeOnCardboardNfcListener(this);
/* 158:288 */	 base.onDestroy();
/* 159:    */
   }/* 160:    */   
/* 161:    */   public void setContentView(View view)
/* 162:    */
   {/* 163:294 */	 if ((view is CardboardView))
	 {
/* 164:295 */	   CardboardView = (CardboardView)view;
/* 165:    */
	 }/* 166:298 */	 base.ContentView = view;
/* 167:    */
   }/* 168:    */   
/* 169:    */   public void setContentView(View view, ViewGroup.LayoutParams @params)
/* 170:    */
   {/* 171:304 */	 if ((view is CardboardView))
	 {
/* 172:305 */	   CardboardView = (CardboardView)view;
/* 173:    */
	 }/* 174:308 */	 base.setContentView(view, @params);
/* 175:    */
   }/* 176:    */   
/* 177:    */   public bool onKeyDown(int keyCode, KeyEvent @event)
/* 178:    */
   {/* 179:315 */	 if (((keyCode == 24) || (keyCode == 25)) && (areVolumeKeysDisabled()))
	 {
/* 180:317 */	   return true;
/* 181:    */
	 }/* 182:320 */	 return base.onKeyDown(keyCode, @event);
/* 183:    */
   }/* 184:    */   
/* 185:    */   public bool onKeyUp(int keyCode, KeyEvent @event)
/* 186:    */
   {/* 187:327 */	 if (((keyCode == 24) || (keyCode == 25)) && (areVolumeKeysDisabled()))
	 {
/* 188:329 */	   return true;
/* 189:    */
	 }/* 190:332 */	 return base.onKeyUp(keyCode, @event);
/* 191:    */
   }/* 192:    */   
/* 193:    */   public void onWindowFocusChanged(bool hasFocus)
/* 194:    */
   {/* 195:338 */	 base.onWindowFocusChanged(hasFocus);
/* 196:340 */	 if (hasFocus)
	 {
/* 197:341 */	   setFullscreenMode();
/* 198:    */
	 }/* 199:    */
   }/* 200:    */   
/* 201:    */   private void setFullscreenMode()
/* 202:    */
   {/* 203:347 */	 Window.DecorView.SystemUiVisibility = 5894;
/* 204:    */
   }/* 205:    */   
/* 206:    */   public static class VolumeKeys
/* 207:    */
   {/* 208:    */	 public static final int NOT_DISABLED = 0;
/* 209:    */	 public static final int DISABLED = 1;
/* 210:    */	 public static final int DISABLED_WHILE_IN_CARDBOARD = 2;
/* 211:    */
   }/* 212:    */
 }

/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
 * Qualified Name:     com.google.vrtoolkit.cardboard.CardboardActivity
 * JD-Core Version:    0.7.0.1
 */

private class OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper : View.OnSystemUiVisibilityChangeListener
{
	private readonly MissingClass outerInstance;

	private Handler handler;

	public OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper(MissingClass outerInstance, Handler handler)
	{
		this.outerInstance = outerInstance;
		this.handler = handler;
	}

/* 118:    *//* 119:    */	public virtual void onSystemUiVisibilityChange(int visibility)
/* 120:    */
	{/* 121:243 */	  if ((visibility & 0x2) == 0)
	  {
/* 122:244 */		handler.postDelayed(new RunnableAnonymousInnerClassHelper(this), 2000L);
/* 129:    */
	  }/* 130:    */
	}
	private class RunnableAnonymousInnerClassHelper : Runnable
	{
		private readonly OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper outerInstance;

		public RunnableAnonymousInnerClassHelper(OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper outerInstance)
		{
			this.outerInstance = outerInstance;
		}

	/* 123:    */	/* 124:    */		public virtual void run()
	/* 125:    */
		{	/* 126:247 */		  CardboardActivity.this.setFullscreenMode();
	/* 127:    */
		}	/* 128:247 */
	}/* 131:    */
}
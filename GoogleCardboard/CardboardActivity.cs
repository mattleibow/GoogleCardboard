public class CardboardActivity : Activity, MagnetSensor.OnCardboardTriggerListener, NfcSensor.OnCardboardNfcListener
{
  private static final int NAVIGATION_BAR_TIMEOUT_MS = 2000;
  private CardboardView mCardboardView;
  private MagnetSensor mMagnetSensor;
  private NfcSensor mNfcSensor;
  private int mVolumeKeysMode;
  
  public void setCardboardView(CardboardView cardboardView)
  {
    this.mCardboardView = cardboardView;

	 if (cardboardView != null)

	 {	   CardboardDeviceParams cardboardDeviceParams = this.mNfcSensor.CardboardDeviceParams;
	   if (cardboardDeviceParams == null)
	   {
		 cardboardDeviceParams = new CardboardDeviceParams();

	   }	   cardboardView.updateCardboardDeviceParams(cardboardDeviceParams);

	 }
   }   
   public CardboardView CardboardView

   {	 return this.mCardboardView;

   }   
   public void setVolumeKeysMode(int mode)

   {	 this.mVolumeKeysMode = mode;

   }   
   public int VolumeKeysMode

   {	 return this.mVolumeKeysMode;

   }   
   public bool areVolumeKeysDisabled()

   {	 switch (this.mVolumeKeysMode)

	 {	 case 0:
	   return false;
	 case 2:
	   return DeviceInCardboard;
	 case 1:
	   return true;

	 }	 throw new IllegalStateException("Invalid volume keys mode " + this.mVolumeKeysMode);

   }   
   public bool DeviceInCardboard

   {	 return this.mNfcSensor.DeviceInCardboard;

   }   
   public void onInsertedIntoCardboard(CardboardDeviceParams deviceParams)

   {	 if (this.mCardboardView != null)
	 {
	   this.mCardboardView.updateCardboardDeviceParams(deviceParams);

	 }
   }   
   public void onRemovedFromCardboard()
   {
   }
   
   public void onCardboardTrigger()
   {
   }
   
   protected void onNfcIntent(Intent intent)

   {	 this.mNfcSensor.onNfcIntent(intent);

   }   
   protected void onCreate(Bundle savedInstanceState)

   {	 base.onCreate(savedInstanceState);
     
 
	 requestWindowFeature(1);
     
 
	 Window.addFlags(128);
     
 
	 this.mMagnetSensor = new MagnetSensor(this);
	 this.mMagnetSensor.OnCardboardTriggerListener = this;
     
 
	 this.mNfcSensor = NfcSensor.getInstance(this);
	 this.mNfcSensor.addOnCardboardNfcListener(this);
     
 
	 onNfcIntent(Intent);
     
 
	 VolumeKeysMode = 2;
	 if (Build.VERSION.SDK_INT < 19)

	 {//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final Handler handler = new Handler();
	   Handler handler = new Handler();
	   Window.DecorView.OnSystemUiVisibilityChangeListener = new OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper(this, handler);

	 }
   }   
   protected void onResume()

   {	 base.onResume();
	 if (this.mCardboardView != null)
	 {
	   this.mCardboardView.onResume();

	 }	 this.mMagnetSensor.start();
	 this.mNfcSensor.onResume(this);

   }   
   protected void onPause()

   {	 base.onPause();
	 if (this.mCardboardView != null)
	 {
	   this.mCardboardView.onPause();

	 }	 this.mMagnetSensor.stop();
	 this.mNfcSensor.onPause(this);

   }   
   protected void onDestroy()

   {	 this.mNfcSensor.removeOnCardboardNfcListener(this);
	 base.onDestroy();

   }   
   public void setContentView(View view)

   {	 if ((view is CardboardView))
	 {
	   CardboardView = (CardboardView)view;

	 }	 base.ContentView = view;

   }   
   public void setContentView(View view, ViewGroup.LayoutParams @params)

   {	 if ((view is CardboardView))
	 {
	   CardboardView = (CardboardView)view;

	 }	 base.setContentView(view, @params);

   }   
   public bool onKeyDown(int keyCode, KeyEvent @event)

   {	 if (((keyCode == 24) || (keyCode == 25)) && (areVolumeKeysDisabled()))
	 {
	   return true;

	 }	 return base.onKeyDown(keyCode, @event);

   }   
   public bool onKeyUp(int keyCode, KeyEvent @event)

   {	 if (((keyCode == 24) || (keyCode == 25)) && (areVolumeKeysDisabled()))
	 {
	   return true;

	 }	 return base.onKeyUp(keyCode, @event);

   }   
   public void onWindowFocusChanged(bool hasFocus)

   {	 base.onWindowFocusChanged(hasFocus);
	 if (hasFocus)
	 {
	   setFullscreenMode();

	 }
   }   
   private void setFullscreenMode()

   {	 Window.DecorView.SystemUiVisibility = 5894;

   }   
   public static class VolumeKeys

   {	 public static final int NOT_DISABLED = 0;
	 public static final int DISABLED = 1;
	 public static final int DISABLED_WHILE_IN_CARDBOARD = 2;

   }
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

	public virtual void onSystemUiVisibilityChange(int visibility)

	{	  if ((visibility & 0x2) == 0)
	  {
		handler.postDelayed(new RunnableAnonymousInnerClassHelper(this), 2000L);

	  }
	}
	private class RunnableAnonymousInnerClassHelper : Runnable
	{
		private readonly OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper outerInstance;

		public RunnableAnonymousInnerClassHelper(OnSystemUiVisibilityChangeListenerAnonymousInnerClassHelper outerInstance)
		{
			this.outerInstance = outerInstance;
		}

				public virtual void run()
	
		{			  CardboardActivity.this.setFullscreenMode();
	
		}	
	}
}
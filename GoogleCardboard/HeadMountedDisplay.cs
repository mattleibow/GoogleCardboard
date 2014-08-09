 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using Display = android.view.Display;
	 
		 public class HeadMountedDisplay
	
	 {		   private ScreenParams mScreen;
		   private CardboardDeviceParams mCardboard;
	   
		   public HeadMountedDisplay(Display display)
	
	   {			 this.mScreen = new ScreenParams(display);
			 this.mCardboard = new CardboardDeviceParams();
	
	   }	   
		   public HeadMountedDisplay(HeadMountedDisplay hmd)
	
	   {			 this.mScreen = new ScreenParams(hmd.mScreen);
			 this.mCardboard = new CardboardDeviceParams(hmd.mCardboard);
	
	   }	   
	
	   public virtual ScreenParams Screen
	   {
		   set
		
		   {					 this.mScreen = new ScreenParams(value);
		
		   }		   get
		
		   {					 return this.mScreen;
		
		   }
	   }	   
	
	   public virtual CardboardDeviceParams Cardboard
	   {
		   set
		
		   {					 this.mCardboard = new CardboardDeviceParams(value);
		
		   }		   get
		
		   {					 return this.mCardboard;
		
		   }
	   }	   
		   public override bool Equals(object other)
	
	   {			 if (other == null)
		 {
			   return false;
	
		 }			 if (other == this)
		 {
			   return true;
	
		 }			 if (!(other is HeadMountedDisplay))
		 {
			   return false;
	
		 }			 HeadMountedDisplay o = (HeadMountedDisplay)other;
	     
			 return (this.mScreen.Equals(o.mScreen)) && (this.mCardboard.Equals(o.mCardboard));
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.HeadMountedDisplay
	 * JD-Core Version:    0.7.0.1
	 */
 }
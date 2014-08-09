 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using DisplayMetrics = android.util.DisplayMetrics;
		 using Display = android.view.Display;
	 
		 public class ScreenParams
	
	 {		   public const float METERS_PER_INCH = 0.0254F;
		   private const float DEFAULT_BORDER_SIZE_METERS = 0.003F;
		   private int mWidth;
		   private int mHeight;
		   private float mXMetersPerPixel;
		   private float mYMetersPerPixel;
		   private float mBorderSizeMeters;
	   
		   public ScreenParams(Display display)
	
	   {			 DisplayMetrics metrics = new DisplayMetrics();
			 try
	
		 {			   display.getRealMetrics(metrics);
	
		 }			 catch (System.MissingMethodException)
	
		 {			   display.getMetrics(metrics);
	
		 }			 this.mXMetersPerPixel = (0.0254F / metrics.xdpi);
			 this.mYMetersPerPixel = (0.0254F / metrics.ydpi);
			 this.mWidth = metrics.widthPixels;
			 this.mHeight = metrics.heightPixels;
			 this.mBorderSizeMeters = 0.003F;
			 if (this.mHeight > this.mWidth)
	
		 {			   int tempPx = this.mWidth;
			   this.mWidth = this.mHeight;
			   this.mHeight = tempPx;
	       
			   float tempMetersPerPixel = this.mXMetersPerPixel;
			   this.mXMetersPerPixel = this.mYMetersPerPixel;
			   this.mYMetersPerPixel = tempMetersPerPixel;
	
		 }	
	   }	   
		   public ScreenParams(ScreenParams @params)
	
	   {			 this.mWidth = @params.mWidth;
			 this.mHeight = @params.mHeight;
			 this.mXMetersPerPixel = @params.mXMetersPerPixel;
			 this.mYMetersPerPixel = @params.mYMetersPerPixel;
			 this.mBorderSizeMeters = @params.mBorderSizeMeters;
	
	   }	   
	
	   public virtual int Width
	   {
		   set
		
		   {					 this.mWidth = value;
		
		   }		   get
		
		   {					 return this.mWidth;
		
		   }
	   }	   
	
	   public virtual int Height
	   {
		   set
		
		   {					 this.mHeight = value;
		
		   }		   get
		
		   {					 return this.mHeight;
		
		   }
	   }	   
	
	   public virtual float WidthMeters
	   {
		   get
		
		   {					 return this.mWidth * this.mXMetersPerPixel;
		
		   }
	   }	   
	
	   public virtual float HeightMeters
	   {
		   get
		
		   {					 return this.mHeight * this.mYMetersPerPixel;
		
		   }
	   }	   
	
	   public virtual float BorderSizeMeters
	   {
		   set
		
		   {					 this.mBorderSizeMeters = value;
		
		   }		   get
		
		   {					 return this.mBorderSizeMeters;
		
		   }
	   }	   
		   public override bool Equals(object other)
	
	   {			 if (other == null)
		 {
			   return false;
	
		 }			 if (other == this)
		 {
			   return true;
	
		 }			 if (!(other is ScreenParams))
		 {
			   return false;
	
		 }			 ScreenParams o = (ScreenParams)other;
	     
			 return (this.mWidth == o.mWidth) && (this.mHeight == o.mHeight) && (this.mXMetersPerPixel == o.mXMetersPerPixel) && (this.mYMetersPerPixel == o.mYMetersPerPixel) && (this.mBorderSizeMeters == o.mBorderSizeMeters);
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.ScreenParams
	 * JD-Core Version:    0.7.0.1
	 */
 }
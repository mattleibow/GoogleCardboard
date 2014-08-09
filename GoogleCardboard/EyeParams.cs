 namespace com.google.vrtoolkit.cardboard
 {
	 
		 public class EyeParams
	
	 {		   private readonly int mEye;
		   private readonly Viewport mViewport;
		   private readonly FieldOfView mFov;
		   private readonly EyeTransform mEyeTransform;
	   
		   public EyeParams(int eye)
	
	   {			 this.mEye = eye;
			 this.mViewport = new Viewport();
			 this.mFov = new FieldOfView();
			 this.mEyeTransform = new EyeTransform(this);
	
	   }	   
	
	   public virtual int Eye
	   {
		   get
		
		   {					 return this.mEye;
		
		   }
	   }	   
	
	   public virtual Viewport Viewport
	   {
		   get
		
		   {					 return this.mViewport;
		
		   }
	   }	   
	
	   public virtual FieldOfView Fov
	   {
		   get
		
		   {					 return this.mFov;
		
		   }
	   }	   
	
	   public virtual EyeTransform Transform
	   {
		   get
		
		   {					 return this.mEyeTransform;
		
		   }
	   }	   
		   public class Eye
	
	   {			 public const int MONOCULAR = 0;
			 public const int LEFT = 1;
			 public const int RIGHT = 2;
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.EyeParams
	 * JD-Core Version:    0.7.0.1
	 */
 }
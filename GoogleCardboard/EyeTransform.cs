 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using Matrix = android.opengl.Matrix;
	 
		 public class EyeTransform
	
	 {		   private readonly EyeParams mEyeParams;
		   private readonly float[] mEyeView;
		   private readonly float[] mPerspective;
	   
		   public EyeTransform(EyeParams @params)
	
	   {			 this.mEyeParams = @params;
			 this.mEyeView = new float[16];
			 this.mPerspective = new float[16];
	     
			 Matrix.setIdentityM(this.mEyeView, 0);
			 Matrix.setIdentityM(this.mPerspective, 0);
	
	   }	   
	
	   public virtual float[] EyeView
	   {
		   get
		
		   {					 return this.mEyeView;
		
		   }
	   }	   
	
	   public virtual float[] Perspective
	   {
		   get
		
		   {					 return this.mPerspective;
		
		   }
	   }	   
	
	   public virtual EyeParams Params
	   {
		   get
		
		   {					 return this.mEyeParams;
		
		   }
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.EyeTransform
	 * JD-Core Version:    0.7.0.1
	 */
 }
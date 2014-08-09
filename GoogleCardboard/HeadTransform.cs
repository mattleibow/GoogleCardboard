using System;

 namespace com.google.vrtoolkit.cardboard
 {
	 
		 using Matrix = android.opengl.Matrix;
		 using FloatMath = android.util.FloatMath;
	 
		 public class HeadTransform
	
	 {		   private const float GIMBAL_LOCK_EPSILON = 0.01F;
		   private const float PI = 3.141593F;
		   private readonly float[] mHeadView;
	   
		   public HeadTransform()
	
	   {			 this.mHeadView = new float[16];
			 Matrix.setIdentityM(this.mHeadView, 0);
	
	   }	   
	
	   internal virtual float[] HeadView
	   {
		   get
		
		   {					 return this.mHeadView;
		
		   }
	   }	   
		   public virtual void getHeadView(float[] headView, int offset)
	
	   {			 if (offset + 16 > headView.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 Array.Copy(this.mHeadView, 0, headView, offset, 16);
	
	   }	   
		   public virtual void getTranslation(float[] translation, int offset)
	
	   {			 if (offset + 3 > translation.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 for (int i = 0; i < 3; i++)
		 {
			   translation[(i + offset)] = this.mHeadView[(12 + i)];
	
		 }	
	   }	   
		   public virtual void getForwardVector(float[] forward, int offset)
	
	   {			 if (offset + 3 > forward.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 for (int i = 0; i < 3; i++)
		 {
			   forward[(i + offset)] = (-this.mHeadView[(8 + i)]);
	
		 }	
	   }	   
		   public virtual void getUpVector(float[] up, int offset)
	
	   {			 if (offset + 3 > up.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 for (int i = 0; i < 3; i++)
		 {
			   up[(i + offset)] = this.mHeadView[(4 + i)];
	
		 }	
	   }	   
		   public virtual void getRightVector(float[] right, int offset)
	
	   {			 if (offset + 3 > right.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 for (int i = 0; i < 3; i++)
		 {
			   right[(i + offset)] = this.mHeadView[i];
	
		 }	
	   }	   
		   public virtual void getQuaternion(float[] quaternion, int offset)
	
	   {			 if (offset + 4 > quaternion.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 float[] m = this.mHeadView;
			 float t = m[0] + m[5] + m[10];
			 float z;
			 float z;
			 float x;
			 float y;
			 float w;
			 if (t >= 0.0F)
	
		 {			   float s = FloatMath.sqrt(t + 1.0F);
			   float w = 0.5F * s;
			   s = 0.5F / s;
			   float x = (m[9] - m[6]) * s;
			   float y = (m[2] - m[8]) * s;
			   z = (m[4] - m[1]) * s;
	
		 }			 else
	
		 {			   float w;
			   if ((m[0] > m[5]) && (m[0] > m[10]))
	
		   {				 float s = FloatMath.sqrt(1.0F + m[0] - m[5] - m[10]);
				 float x = s * 0.5F;
				 s = 0.5F / s;
				 float y = (m[4] + m[1]) * s;
				 float z = (m[2] + m[8]) * s;
				 w = (m[9] - m[6]) * s;
	
		   }			   else
	
		   {				 float w;
				 if (m[5] > m[10])
	
			 {				   float s = FloatMath.sqrt(1.0F + m[5] - m[0] - m[10]);
				   float y = s * 0.5F;
				   s = 0.5F / s;
				   float x = (m[4] + m[1]) * s;
				   float z = (m[9] + m[6]) * s;
				   w = (m[2] - m[8]) * s;
	
			 }				 else
	
			 {				   float s = FloatMath.sqrt(1.0F + m[10] - m[0] - m[5]);
				   z = s * 0.5F;
				   s = 0.5F / s;
				   x = (m[2] + m[8]) * s;
				   y = (m[9] + m[6]) * s;
				   w = (m[4] - m[1]) * s;
	
			 }	
		   }	
		 }			 quaternion[(offset + 0)] = x;
			 quaternion[(offset + 1)] = y;
			 quaternion[(offset + 2)] = z;
			 quaternion[(offset + 3)] = w;
	
	   }	   
		   public virtual void getEulerAngles(float[] eulerAngles, int offset)
	
	   {			 if (offset + 3 > eulerAngles.Length)
		 {
			   throw new System.ArgumentException("Not enough space to write the result");
	
		 }			 float pitch = (float)Math.Asin(this.mHeadView[6]);
			 float roll;
			 float yaw;
			 float roll;
			 if (FloatMath.sqrt(1.0F - this.mHeadView[6] * this.mHeadView[6]) >= 0.01F)
	
		 {			   float yaw = (float)Math.Atan2(-this.mHeadView[2], this.mHeadView[10]);
			   roll = (float)Math.Atan2(-this.mHeadView[4], this.mHeadView[5]);
	
		 }			 else
	
		 {			   yaw = 0.0F;
			   roll = (float)Math.Atan2(this.mHeadView[1], this.mHeadView[0]);
	
		 }			 eulerAngles[(offset + 0)] = (-pitch);
			 eulerAngles[(offset + 1)] = (-yaw);
			 eulerAngles[(offset + 2)] = (-roll);
	
	   }	
	 }

	/* Location:           C:\Users\mattl_000.SURFACE\Downloads\cardboard.jar
	 * Qualified Name:     com.google.vrtoolkit.cardboard.HeadTransform
	 * JD-Core Version:    0.7.0.1
	 */
 }